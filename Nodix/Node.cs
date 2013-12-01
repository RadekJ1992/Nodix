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
        //fuck delegates! :)
        delegate void SetTextCallback(string text);

        //otrzymany i wysyłany pakiets
        private Packet.ATMPacket receivedPacket;
        private Packet.ATMPacket processedPacket;

        //unikalny numer danego węzła
        public int nodeNumber { get; set; }
        private bool isNodeNumberSet;

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

        private NetworkStream networkStream;

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
        // trzeba ustawić na 0. Musiałem zaimplementować coś w stylu tej kreski co jest w [a,-] -> [b,-], a skoro
        // pole VCI ma 16 bitów to ten int zgodnie z założeniami może przybierać wartośći od 1 do 65536, 0 jest poza
        //jego zasięgiem

        //
        private Dictionary<PortVPIVCI, PortVPIVCI> VCArray = new Dictionary<PortVPIVCI,PortVPIVCI>(new PortVPIVCIComparer());

        public Nodix() {
            InitializeComponent();
            isNodeNumberSet = false;
            PortVPIVCI k = new PortVPIVCI(4, 3, 2);
            PortVPIVCI v = new PortVPIVCI(2, 2, 2);
            addEntry(k, v);
        }

        private void connectToCloud(object sender, EventArgs e) {
            if (isNodeNumberSet) {
                if (IPAddress.TryParse(cloudIPField.Text, out cloudAddress)) {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Cloud IP set properly as " + cloudAddress.ToString() + " \n");
                } else {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading cloud IP" + " \n");
                }
                if (Int32.TryParse(cloudPortField.Text, out cloudPort)) {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Cloud port set properly as " + cloudPort.ToString() + " \n");
                } else {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading cloud Port" + " \n");
                }

                cloudSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                cloudEndPoint = new IPEndPoint(cloudAddress, cloudPort);
                try {
                    cloudSocket.Connect(cloudEndPoint);
                    isConnectedToCloud = true;
                    receiveThread = new Thread(this.receiver);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                } catch (SocketException ex) {
                    isConnectedToCloud = false;
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error while connecting to cloud\n");
                    log.AppendText("Wrong IP or port?\n");
                }
            } else SetText("Ustal numer węzła!\n");
        }

        private void connectToManager(object sender, EventArgs e) {
            if (isNodeNumberSet) {
                if (IPAddress.TryParse(managerIPField.Text, out managerAddress)) {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Manager IP set properly as " + managerAddress.ToString() + " \n");
                } else {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading manager IP" + " \n");
                }
                if (Int32.TryParse(managerPortField.Text, out managerPort)) {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Manager port set properly as " + managerPort.ToString() + " \n");
                } else {
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error reading manager Port" + " \n");
                }

                managerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                managerEndPoint = new IPEndPoint(managerAddress, managerPort);
                try {
                    managerSocket.Connect(managerEndPoint);
                    isConnectedToManager = true;

                    //działanie AGENTA


                } catch (SocketException ex) {
                    Console.WriteLine(ex.StackTrace);
                    isConnectedToManager = false;
                    log.AppendText(DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + " Error while connecting to manager\n");
                    log.AppendText("Wrong IP or port?\n");
                }
            } else SetText("Ustal numer węzła!\n");
            
        }

        private void receiver() {
            if (networkStream == null) {
                networkStream = new NetworkStream(cloudSocket);
                
                //tworzy string 'Node ' i tu jego numer
                String welcomeString = "Node " + nodeNumber;
                //tworzy tablicę bajtów z tego stringa
                byte[] welcomeStringBytes = AAL.GetBytesFromString(welcomeString);
                //wysyła tą tablicę bajtów streamem
                networkStream.Write(welcomeStringBytes, 0, welcomeStringBytes.Length);
            }
            BinaryFormatter bf = new BinaryFormatter();
            try {
                receivedPacket = (Packet.ATMPacket)bf.Deserialize(networkStream);
                queuedReceivedPackets.Enqueue(receivedPacket);
            } catch { }
           // networkStream.Close();
            sendThread = new Thread(this.sender);
            sendThread.IsBackground = true;
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
            //if (!queuedReceivedPackets.IsEmpty) {
            if (queuedReceivedPackets.TryDequeue(out processedPacket)) {
                //queuedReceivedPackets.TryDequeue(out processedPacket);
                if (processedPacket != null) {
                    PortVPIVCI VCConKey = new PortVPIVCI();
                    PortVPIVCI VPConKey = new PortVPIVCI();
                    PortVPIVCI value = new PortVPIVCI();
                    VCConKey.port = processedPacket.port;
                    VCConKey.VPI = processedPacket.VPI;
                    VCConKey.VCI = processedPacket.VCI;
                    VPConKey.port = processedPacket.port;
                    VPConKey.VPI = processedPacket.VPI;
                    VPConKey.VCI = 0;
                    NetworkStream networkStream = new NetworkStream(cloudSocket);
                    if (VCArray.ContainsKey(VCConKey)) {
                        if (VCArray.TryGetValue(VCConKey, out value)) {
                            SetText("Przekierowywanie [" + processedPacket.port + ";" + processedPacket.VPI + ";" + processedPacket.VCI + "]->[" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                            processedPacket.VPI = value.VPI;
                            processedPacket.VCI = value.VCI;
                            processedPacket.port = value.port;
                            BinaryFormatter bformatter = new BinaryFormatter();
                            bformatter.Serialize(networkStream, processedPacket);
                            networkStream.Close();
                        } else {
                            SetText("Coś poszło nie tak przy przepisywaniu wartości VPI i VCI z VCArray\n");
                        }
                    } else if (VCArray.ContainsKey(VPConKey)) {
                        if (VCArray.TryGetValue(VPConKey, out value)) {

                            SetText("Przekierowywanie [" + processedPacket.port + ";" + processedPacket.VPI + ";" + processedPacket.VCI + "]->[" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                            processedPacket.VPI = value.VPI;
                            processedPacket.port = value.port;
                            // VCI bez zmian
                            BinaryFormatter bformatter = new BinaryFormatter();
                            bformatter.Serialize(netStream, processedPacket);
                            networkStream.Close();
                        } else {
                            SetText("Coś poszło nie tak przy przepisywaniu wartości VPI i VCI z VCArray\n");
                        }
                    } else {
                        SetText("Pakiet stracony - brak odpowiedniego wpisu w tablicy\n");
                    }
                }
            }
        }

        //Dodaje pozycję do VCArray, pobiera dwa obiekty PortVPIVCI
        //WAŻNE - dodaje wpis 'w obie strony'
        public void addEntry(PortVPIVCI key, PortVPIVCI value) {
            if (VCArray.ContainsKey(key))
            {
                SetText("Zmieniam stary klucz VCArray\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
            else
            {
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
        }

        //Dodaje pozycję do VCArray, pobiera inty jako poszczególne wartości
        //WAŻNE - dodaje wpis 'w obie strony'
        public void addEntry(int keyPort, int keyVPI, int keyVCI, int valuePort, int valueVPI, int valueVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            PortVPIVCI value = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                SetText("Zmieniam stary klucz VCArray\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
            else {
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
        }

        //usuwa pojedynczy wpis
        public void removeSingleEntry(PortVPIVCI key) {
            if (VCArray.ContainsKey(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }
        //usuwa oba wpisy, jak się nie da to usuwa tylko jeden
        public void removeEntry(PortVPIVCI key) {
            if (VCArray.ContainsKey(key) && VCArray.ContainsValue(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    VCArray.Remove(temp);
                }
            } else removeSingleEntry(key);
        }

        //usuwa pojedynczy wpis
        public void removeSingleEntry(int keyPort, int keyVPI, int keyVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }

        //usuwa oba wpisy, jak się nie uda to tylko jeden
        public void removeEntry(int keyPort, int keyVPI, int keyVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            if (VCArray.ContainsKey(key) && VCArray.ContainsValue(key)) {
                SetText("Usuwam klucz w VCArray\n");
                VCArray.Remove(key);
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    VCArray.Remove(temp);
                }
            } else removeSingleEntry(key);
        }

        public void clearTable() {
            VCArray = new Dictionary<PortVPIVCI, PortVPIVCI>(new PortVPIVCIComparer());
        }

        private void setNodeNumber_Click(object sender, EventArgs e) {
            try {
                nodeNumber = int.Parse(NodeNumberField.Text);
                isNodeNumberSet = true;
                SetText("Numer węzła ustawiony jako " + nodeNumber + "\n");
            } catch {
                isNodeNumberSet = false;
                SetText("Numer węzła musi być NUMEREM!\n");
            }
        }
    }
}
