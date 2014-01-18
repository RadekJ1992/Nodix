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
using System.IO;

namespace Nodix {
    public partial class Nodix : Form {
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
        public IPEndPoint managerEndPoint {get; private set;}

        private NetworkStream networkStream; // stream dla chmury

        private Socket cloudSocket;
        public Socket managerSocket { get; private set; }

        private Thread receiveThread;     //wątek służący do odbierania połączeń
        private Thread sendThread;        // analogicznie - do wysyłania

        public bool isRunning { get; private set; }     //info czy klient chodzi - dla zarządcy

        public bool isConnectedToCloud { get; private set; } // czy połączony z chmurą?
        public bool isConnectedToManager { get; set; } // czy połączony z zarządcą?

        public bool isLoggedToManager { get; set; } // czy zalogowany w zarządcy?

        public bool isDisconnect;

        //agent zarządzania
        private Agentix agent;

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
            isLoggedToManager = false;
            isDisconnect = false;
        }

        private void connectToCloud(object sender, EventArgs e) {
            if (isNodeNumberSet) {
                if (!isConnectedToCloud) {
                    if (IPAddress.TryParse(cloudIPField.Text, out cloudAddress)) {
                        log.AppendText("IP ustawiono jako " + cloudAddress.ToString() + " \n");
                    } else {
                        log.AppendText("Błąd podczas ustawiania IP chmury (zły format?)" + " \n");
                    }
                    if (Int32.TryParse(cloudPortField.Text, out cloudPort)) {
                        log.AppendText("Port chmury ustawiony jako " + cloudPort.ToString() + " \n");
                    } else {
                        log.AppendText("Błąd podczas ustawiania portu chmury (zły format?)" + " \n");
                    }

                    cloudSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    cloudEndPoint = new IPEndPoint(cloudAddress, cloudPort);
                    try {
                        cloudSocket.Connect(cloudEndPoint);
                        isConnectedToCloud = true;
                        receiveThread = new Thread(this.receiver);
                        receiveThread.IsBackground = true;
                        receiveThread.Start();
                    } catch {
                        isConnectedToCloud = false;
                        log.AppendText("Błąd podczas łączenia się z chmurą\n");
                        log.AppendText("Złe IP lub port? Chmura nie działa?\n");
                    }
                } else SetText("Węzeł jest już połączony z chmurą\n");
            } else SetText("Ustal numer węzła!\n");
        }

        private void connectToManager(object sender, EventArgs e) {
            if (isNodeNumberSet) {
                if (!isConnectedToManager) {
                    if (IPAddress.TryParse(managerIPField.Text, out managerAddress)) {
                        log.AppendText("IP zarządcy ustawione jako " + managerAddress.ToString() + " \n");
                    } else {
                        log.AppendText("Błąd podczas ustawiania IP zarządcy\n");
                    }
                    if (Int32.TryParse(managerPortField.Text, out managerPort)) {
                        log.AppendText("Port zarządcy ustawiony jako " + managerPort.ToString() + " \n");
                    } else {
                        log.AppendText("Błąd podczas ustawiania portu zarządcy\n");
                    }

                    managerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    managerEndPoint = new IPEndPoint(managerAddress, managerPort);
                    try {
                        managerSocket.Connect(managerEndPoint);
                        isConnectedToManager = true;
                        agent = new Agentix(this);
                        agent.writeThread.Start();
                        agent.writeThread.IsBackground = true;
                        agent.readThread.Start();
                        agent.readThread.IsBackground = true;
                        agent.sendLoginT = true;
                    } catch (SocketException) {
                        isConnectedToManager = false;
                        log.AppendText("Błąd podczas łączenia się z zarządcą!\n");
                        log.AppendText("Złe IP lub port? Zarządca nie działa?\n");
                    }
                } else SetText("Już jestem połączony z zarządcą!\n");
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
                if (receivedPacket.VPI == -1 && receivedPacket.VCI == -1) {
                    //WYŚLIJ DO LRM





                }
                else queuedReceivedPackets.Enqueue(receivedPacket);

                //to może nie działać. Sprawdzi się jeszcze
                if (!sendThread.IsAlive) {
                    sendThread = new Thread(this.sender);
                    sendThread.IsBackground = true;
                    sendThread.Start();
                    receiver();
                }
            } catch (Exception e) {
                if (isDisconnect) { SetText("Rozłączam się z chmurą!\n"); isDisconnect = false; networkStream = null; }
                else { SetText("Coś poszło nie tak : " + e.Message + "\n"); }
            }
        }

        public void SetText(string text) {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.log.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else {
                try {
                    this.log.AppendText(text);
                } catch { }
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
                    if (processedPacket.VPI == -1 && processedPacket.VCI == -1) {
                        SetText("Wysyłam pakiet sygnalizacyjny na porcie " + processedPacket.port + "\n");
                        BinaryFormatter bformatter = new BinaryFormatter();
                        bformatter.Serialize(networkStream, processedPacket);
                        networkStream.Close();
                    }
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

                            SetText("Przekierowywanie [" + processedPacket.port + ";" + processedPacket.VPI + ";" + processedPacket.VCI + "]->[" + value.port + ";" + value.VPI + ";" + processedPacket.VCI + "]\n");
                            processedPacket.VPI = value.VPI;
                            processedPacket.port = value.port;
                            // VCI bez zmian
                            BinaryFormatter bformatter = new BinaryFormatter();
                            bformatter.Serialize(networkStream, processedPacket);
                            networkStream.Close();
                        } else {
                            SetText("Coś poszło nie tak przy przepisywaniu wartości VPI i VCI z VCArray\n");
                        }
                    } else {
                        SetText("Pakiet stracony - brak odpowiedniego wpisu w tablicy\n");
                    }


                }
            }
            sender();
        }

        //Dodaje pozycję do VCArray, pobiera dwa obiekty PortVPIVCI
        //WAŻNE - dodaje wpis 'w obie strony'
        public void addEntry(PortVPIVCI key, PortVPIVCI value) {
            if (VCArray.ContainsKey(key))
            {
                PortVPIVCI temp;
                SetText("Zmieniam stary klucz VCArray na [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI +"]\n");
                SetText("Zmieniam stary klucz VCArray na [" + value.port + ";" + value.VPI + ";" + value.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    VCArray.Remove(temp);
                }
                VCArray.Remove(key);
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
            else
            {
                SetText("Dodaję wpis [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                SetText("Dodaję wpis [" + value.port + ";" + value.VPI + ";" + value.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
        }

        //Dodaje pozycję do VCArray, pobiera inty jako poszczególne wartości
        //WAŻNE - dodaje wpis 'w obie strony'
        //jeśli taki wpis już jest - usuwa stary wpis (też w obie strony) i go zastępuje
        public void addEntry(int keyPort, int keyVPI, int keyVCI, int valuePort, int valueVPI, int valueVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            PortVPIVCI value = new PortVPIVCI(valuePort, valueVPI, valueVCI);
            if (VCArray.ContainsKey(key)) {
                PortVPIVCI temp;
                SetText("Zmieniam stary klucz VCArray na [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI +"]\n");
                SetText("Zmieniam stary klucz VCArray na [" + value.port + ";" + value.VPI + ";" + value.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    VCArray.Remove(temp);
                }
                VCArray.Remove(key);
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
            else {
                SetText("Dodaję wpis [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI +"]\n");
                SetText("Dodaję wpis [" + value.port + ";" + value.VPI + ";" + value.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                VCArray.Add(key, value);
                VCArray.Add(value, key);
            }
        }
        //dodaje wpis w JEDNĄ stronę
        public void addSingleEntry(PortVPIVCI key, PortVPIVCI value) {
            if (VCArray.ContainsKey(key)) {
                SetText("Zmieniam stary klucz VCArray na [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
            } else {
                SetText("Dodaję wpis [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                VCArray.Add(key, value);
            }
        }

        //Dodaje pozycję do VCArray, pobiera inty jako poszczególne wartości
        //WAŻNE - dodaje wpis 'w jedna strone'
        public void addSingleEntry(int keyPort, int keyVPI, int keyVCI, int valuePort, int valueVPI, int valueVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            PortVPIVCI value = new PortVPIVCI(valuePort, valueVPI, valueVCI);
            if (VCArray.ContainsKey(key)) {
                SetText("Zmieniam stary klucz VCArray na [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                VCArray.Remove(key);
                VCArray.Add(key, value);
            } else {
                SetText("Dodaję klucz VCArray na [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + value.port + ";" + value.VPI + ";" + value.VCI + "]\n");
                VCArray.Add(key, value);
            }
        }

        //usuwa pojedynczy wpis
        public void removeSingleEntry(PortVPIVCI key) {
            if (VCArray.ContainsKey(key)) {
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                SetText("Usuwam klucz w VCArray [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "]\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }
        //usuwa oba wpisy, jak się nie da to usuwa tylko jeden
        public void removeEntry(PortVPIVCI key) {
            if (VCArray.ContainsKey(key) && VCArray.ContainsValue(key)) {
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    SetText("Usuwam klucz w VCArray [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                    VCArray.Remove(temp);
                }
                SetText("Usuwam klucz w VCArray [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "]\n");
                VCArray.Remove(key);
            } else removeSingleEntry(key);
        }

        //usuwa pojedynczy wpis
        public void removeSingleEntry(int keyPort, int keyVPI, int keyVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                SetText("Usuwam klucz w VCArray [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "]\n");
                VCArray.Remove(key);
            }
            else SetText("Nie ma takiego klucza\n");
        }

        //usuwa oba wpisy, jak się nie uda to tylko jeden
        public void removeEntry(int keyPort, int keyVPI, int keyVCI) {
            PortVPIVCI key = new PortVPIVCI(keyPort, keyVPI, keyVCI);
            if (VCArray.ContainsKey(key)) {
                PortVPIVCI temp = null;
                VCArray.TryGetValue(key, out temp);
                if (temp != null) {
                    SetText("Usuwam klucz w VCArray [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "] -> [" + key.port + ";" + key.VPI + ";" + key.VCI + "]\n");
                    VCArray.Remove(temp);
                }
                SetText("Usuwam klucz w VCArray [" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "]\n");
                VCArray.Remove(key);
            } else removeSingleEntry(key);
        }

        public void clearTable() {
            VCArray = new Dictionary<PortVPIVCI, PortVPIVCI>(new PortVPIVCIComparer());
            SetText("Czyszczę tablicę kierowania\n");
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

        private void addEntryButton_Click(object sender, EventArgs e) {
            try {
                PortVPIVCI inValue = new PortVPIVCI(int.Parse(inPortTextBox.Text), int.Parse(inVPITextBox.Text), int.Parse(inVCITextBox.Text));
                PortVPIVCI outValue = new PortVPIVCI(int.Parse(outPortTextBox.Text), int.Parse(outVPITextBox.Text), int.Parse(outVCITextBox.Text));
                addSingleEntry(inValue, outValue);
            } catch {}
            inPortTextBox.Clear();
            inVPITextBox.Clear();
            inVCITextBox.Clear();
            outPortTextBox.Clear();
            outVPITextBox.Clear();
            outVCITextBox.Clear();
        }

        private void chooseTextFile_Click(object sender, EventArgs e) {
            string path;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                path = openFileDialog.FileName;
                readConfig(path);
            }
        }

        public void readConfig(String nNumber) {
            try {
                nodeNumber = int.Parse(nNumber);
                isNodeNumberSet = true;
                NodeNumberField.Text = nodeNumber.ToString();
                SetText("Ustalam numer węzła jako " + nodeNumber + "\n");
                String path = "config" + nNumber + ".txt";
                using (StreamReader sr = new StreamReader(path)) {
                    string[] lines = System.IO.File.ReadAllLines(path);
                    foreach (String line in lines) {
                        String[] command = line.Split(' ');
                        if (command[0] == "ADD") {
                            try {
                                addSingleEntry(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]),
                                    int.Parse(command[4]), int.Parse(command[5]), int.Parse(command[6]));
                            } catch (IndexOutOfRangeException) {
                                SetText("Komenda została niepoprawnie sformułowana (za mało parametrów)\n");
                            }
                        } else if (command[0] == "CLEAR") {
                            clearTable();
                        }
                    }
                }
            } catch (Exception exc) {
                SetText("Błąd podczas konfigurowania pliku konfiguracyjnego\n");
                SetText(exc.Message + "\n");
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e) {
            isDisconnect = true;
            isConnectedToCloud = false;
            isConnectedToManager = false;
            if (cloudSocket != null) cloudSocket.Close();
            if (managerSocket != null) managerSocket.Close();
        }

        private void saveConfigButton_Click(object sender, EventArgs e) {
            saveConfig();
        }

        private void saveConfig() {
            if (nodeNumber != null) {
                List<String> lines = new List<String>();
                foreach (PortVPIVCI key in VCArray.Keys) {
                    PortVPIVCI value;
                    if (VCArray.TryGetValue(key, out value)) lines.Add("ADD " + key.port + " " + key.VPI + " " + key.VCI +
                                                                        " " + value.port + " " + value.VPI + " " + value.VCI);
                }
                System.IO.File.WriteAllLines("config" + nodeNumber + ".txt", lines);
                SetText("Zapisuję ustawienia do pliku config" + nodeNumber + ".txt\n");
            } else SetText("Ustal numer węzła!\n");
        }

        private void printVCArrayButton_Click(object sender, EventArgs e) {
            foreach (PortVPIVCI key in VCArray.Keys) {
                PortVPIVCI temp;
                if (VCArray.TryGetValue(key, out temp))
                SetText("[" + key.port + ";" + key.VPI + ";" + key.VCI + "] -> [" + temp.port + ";" + temp.VPI + ";" + temp.VCI + "]\n");
            }
        }

        private void Nodix_FormClosed(object sender, FormClosedEventArgs e) {
            if (nodeNumber != null) saveConfig();
        }
    }

    class Agentix {
        StreamReader read = null;
        StreamWriter write = null;
        NetworkStream netstream = null;
        Nodix parent;
        public Thread writeThread;
        public Thread readThread;
        public bool sendLoginT;

        public Agentix(Nodix parent) {
            this.parent = parent;
            netstream = new NetworkStream(parent.managerSocket);
            read = new StreamReader(netstream);
            write = new StreamWriter(netstream);
            readThread = new Thread(reader);
            writeThread = new Thread(writer);
        }
        //Funkcja odpowiedzialna za odbieraie danych od serwera
        //wykonywana w osobnym watąku
        private void reader() {

            String odp;
            Char[] delimitter = { ' ' };
            String[] slowa;
            while (parent.isConnectedToManager) {
                try {
                    odp = read.ReadLine();
                    slowa = odp.Split(delimitter, StringSplitOptions.RemoveEmptyEntries);
                    if (slowa[0] == "ADD") {
                        //dodawanie wpisu
                        int p1, vc1, vp1, p2, vc2, vp2;
                        if (slowa.Length != 7) {
                            parent.SetText("Zła liczba parametrów w ADD: " + slowa.Length + "\n");
                        } else {

                            p1 = int.Parse(slowa[1]);
                            vp1 = int.Parse(slowa[2]);
                            vc1 = int.Parse(slowa[3]);
                            p2 = int.Parse(slowa[4]);
                            vp2 = int.Parse(slowa[5]);
                            vc2 = int.Parse(slowa[6]);
                            parent.addSingleEntry(p1, vp1, vc1, p2, vp2, vc2);
                        }
                    } else if (slowa[0] == "DELETE") {
                        //usuwanie jednego wpisu
                        int p1, vc1, vp1, p2, vc2, vp2;
                        if (slowa.Length != 7) {
                            parent.SetText("Zła liczba parametrów w DELETE: " + slowa.Length + "\n");
                        } else {

                            p1 = int.Parse(slowa[1]);
                            vp1 = int.Parse(slowa[2]);
                            vc1 = int.Parse(slowa[3]);
                            p2 = int.Parse(slowa[4]);
                            vp2 = int.Parse(slowa[5]);
                            vc2 = int.Parse(slowa[6]);
                            parent.removeSingleEntry(p1, vp1, vc1);
                           // parent.removeEntry(p2, vp2, vc1);

                        }
                    } else if (slowa[0] == "CLEAR") {
                        //usuwanie wszystkich wpisów
                        parent.clearTable();
                    } else if (slowa[0] == "LOGGED") {
                        //udane logowanie
                        parent.isLoggedToManager = true;
                        //parent.addEntry(slowa[1], new PortVPIVCI( int.Parse(slowa[2]), int.Parse(slowa[3]), int.Parse(slowa[4]));
                    } else if (slowa[0] == "MSG" || slowa[0] == "DONE") {
                        parent.SetText("Wykryto komunikat od zarządcy o treści:\n");
                        parent.SetText(odp + "\n");
                    } else if (slowa[0] == "ERR") {
                        parent.SetText("Wykryto komunikat błędu o treści:");
                        foreach (String s in slowa) {
                            parent.SetText(" " + s + " ");
                        }
                        parent.SetText("\n");
                        parent.isConnectedToManager = false;
                        writeThread.Abort();
                        readThread.Abort();
                        parent.SetText("Połącz się ponownie!\n");
                    }

                } catch {
                    if (parent.isDisconnect) {
                        parent.SetText("Rozłączam się z zarządcą!\n");
                        parent.isConnectedToManager = false;
                        writeThread.Abort();
                        readThread.Abort();
                        parent.isDisconnect = false;
                    } else {
                        parent.SetText("Problem w połączeniu się z zarządcą :<\n");
                        parent.isConnectedToManager = false;
                        writeThread.Abort();
                        readThread.Abort();
                    }
                }
            }
        }

        //Funkcja przesyłająca dane do serwera
        //Wykonywana w osobnym watku
        
        private void writer() {
            while (parent.isConnectedToManager) {
                try {
                    if (sendLoginT) {
                        write.WriteLine("LOGINT\n" + parent.nodeNumber);
                        write.Flush();
                        sendLoginT = false;
                    }
                } catch {
                    parent.isConnectedToManager = false;
                    writeThread.Abort();
                    readThread.Abort();
                }
            }
        }
        
    }
}
