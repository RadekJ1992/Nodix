using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Packet;

namespace Nodix {
    public partial class Nodix : Form {
        //fuck delegates!
        delegate void SetTextCallback(string text);

        //otrzymany i wysyłany pakiets
        private Packet.ATMPacket receivedPacket;
        private Packet.ATMPacket processedPacket;

        //kolejka pakietów odebranych z chmury - concurrentQueue jest thread-safe, zwykła queue nie
        private ConcurrentQueue<Packet.ATMPacket> queuedReceivedPackets = new ConcurrentQueue<Packet.ATMPacket>();

        //dane chmury
        private IPAddress cloudAddress;        //Adres na którym chmura nasłuchuje
        private Int32 cloudPort;           //port chmury

        //dane zarządcy
        private IPAddress managerAddress;        //Adres na którym chmura nasłuchuje
        private Int32 managerPort;           //port chmury

        private IPEndPoint cloudEndPoint;
        private IPEndPoint managerEndPoint;

        private Socket cloudSocket;
        private Socket managerSocket;

        private Thread receiveThread;     //wątek służący do odbierania połączeń
        private Thread sendThread;        // analogicznie - do wysyłania

        private NetworkStream netStream;

        public bool isRunning { get; private set; }     //info czy klient chodzi - dla zarządcy

        public bool isConnectedToCloud { get; private set; } // czy połączony z chmurą?
        public bool isConnectedToManager { get; private set; } // czy połączony z zarządcą?

        // tablica kierowania
        // UWAGA!

        // w momencie gdy chcemy ustawić połączenie VP (jak na slajdzie 22 z wykładów TSST to wartość VCI w strukturze 
        // trzeba ustawić na 65536. Musiałem zaimplementować coś w stylu tej kreski co jest w [a,-] -> [b,-], a skoro
        // pole VCI ma 16 bitów to ten int zgodnie z założeniami może przybierać wartośći od 0 do 65535, 65536 jest poza
        //jego zasięgiem

        //
        private Dictionary<VPIVCI, VPIVCI> VCArray = new Dictionary<VPIVCI,VPIVCI>(new VPIVCIComparer());

        public Nodix() {
            InitializeComponent();
            VPIVCI k = new VPIVCI(1, 1);
            VPIVCI v = new VPIVCI(2, 2);
            VCArray.Add(k, v);
        }

        /*
                    log.AppendText("wysyłam pakiet!\n");
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(netStream, packet);
                    netStream.Close();
        */

        private void connectToCloud(object sender, EventArgs e) {
            if (IPAddress.TryParse(cloudIPField.Text, out cloudAddress)) {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Cloud IP set properly as " + cloudAddress.ToString() + " \n");
            }
            else {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading cloud IP" + " \n");
            }
            if (Int32.TryParse(cloudPortField.Text, out cloudPort)) {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Cloud port set properly as " + cloudPort.ToString() + " \n");
            }
            else {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading cloud Port" + " \n");
            }

            cloudSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            cloudEndPoint = new IPEndPoint(cloudAddress, cloudPort);
            try {
                cloudSocket.Connect(cloudEndPoint);
                isConnectedToCloud = true;
                receiveThread = new Thread(this.receiver);
                receiveThread.Start();
            }
            catch (SocketException ex) {
                isConnectedToCloud = false;
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error while connecting to cloud\n");
                log.AppendText("Wrong IP or port?\n");
            }
        }

        private void connectToManager(object sender, EventArgs e) {
            if (IPAddress.TryParse(managerIPField.Text, out managerAddress)) {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Manager IP set properly as " + managerAddress.ToString() + " \n");
            }
            else {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading manager IP" + " \n");
            }
            if (Int32.TryParse(managerPortField.Text, out managerPort)) {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Manager port set properly as " + managerPort.ToString() + " \n");
            }
            else {
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading manager Port" + " \n");
            }

            managerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            managerEndPoint = new IPEndPoint(managerAddress, managerPort);
            try {
                managerSocket.Connect(managerEndPoint);
                isConnectedToManager = true;

                //działanie AGENTA


            }
            catch (SocketException ex) {
                isConnectedToManager = false;
                log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error while connecting to manager\n");
                log.AppendText("Wrong IP or port?\n");
            }
            
        }

        private void receiver() {
            NetworkStream networkStream = new NetworkStream(cloudSocket);
            BinaryFormatter bf = new BinaryFormatter();
            receivedPacket = (Packet.ATMPacket)bf.Deserialize(networkStream);
            queuedReceivedPackets.Enqueue(receivedPacket);
            networkStream.Close();
            sendThread = new Thread(this.sender);
            sendThread.Start();
            receiver();
        }

        private void SetText(string text) {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.log.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else {
                this.log.AppendText(text);
            }
        }

        private void sender() {
            if (!queuedReceivedPackets.IsEmpty) {
                queuedReceivedPackets.TryDequeue(out processedPacket);
                VPIVCI VCConKey = new VPIVCI();
                VPIVCI VPConKey = new VPIVCI();
                VPIVCI value = new VPIVCI();
                VCConKey.VPI = processedPacket.VPI;
                VCConKey.VCI = processedPacket.VCI;
                VPConKey.VPI = processedPacket.VPI;
                VPConKey.VCI = 65536;
                NetworkStream networkStream = new NetworkStream(cloudSocket);
                if (VCArray.ContainsKey(VCConKey)) {
                    if (VCArray.TryGetValue(VCConKey, out value)) {
                        SetText("Przekierowywanie [" + processedPacket.VPI + ";" + processedPacket.VCI + "]->[" + value.VPI + ";" + value.VCI + "]\n");
                        processedPacket.VPI = value.VPI;
                        processedPacket.VCI = value.VCI;
                        //TODO co z portami?
                        BinaryFormatter bformatter = new BinaryFormatter();
                        bformatter.Serialize(networkStream, processedPacket);
                        networkStream.Close();
                    }
                    else {
                        SetText("Coś poszło nie tak przy przepisywaniu wartości VPI i VCI z VCArray\n");
                    }
                }
                else if (VCArray.ContainsKey(VPConKey)) {
                    if (VCArray.TryGetValue(VPConKey, out value)) {

                        SetText("Przekierowywanie [" + processedPacket.VPI + ";" + processedPacket.VCI + "]->[" + value.VPI + ";" + value.VCI + "]\n");
                        processedPacket.VPI = value.VPI;
                        // VCI bez zmian
                        //TODO co z portami?
                        BinaryFormatter bformatter = new BinaryFormatter();
                        bformatter.Serialize(netStream, processedPacket);
                        networkStream.Close();
                    }
                    else {
                        SetText("Coś poszło nie tak przy przepisywaniu wartości VPI i VCI z VCArray\n");
                    }
                }
                else {
                    SetText("Pakiet stracony - brak odpowiedniego wpisu w tablicy\n");
                }
            }
        }

        public void addEntry(VPIVCI key, VPIVCI value) {
            if (VCArray.ContainsKey(key)) {
                SetText("Zmieniam stary klucz VCArray\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
            }
            else VCArray.Add(key, value);
        }

        public void addEntry(int keyVPI, int keyVCI, int valueVPI, int valueVCI) {
            VPIVCI key = new VPIVCI(keyVPI, keyVCI);
            VPIVCI value = new VPIVCI(keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                SetText("Zmieniam stary klucz VCArray\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
            }
            else VCArray.Add(key, value);
        }

        public void removeEntry(VPIVCI key) {
            if (VCArray.ContainsKey(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }

        public void removeEntry(int keyVPI, int keyVCI) {
            VPIVCI key = new VPIVCI(keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }

        public void clearTable() {
            VCArray = new Dictionary<VPIVCI, VPIVCI>(new VPIVCIComparer());
        }
    }
}
