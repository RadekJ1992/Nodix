using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AddressLibrary;
using Packet;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nodix
{
    class eLReMix
    {
        Nodix parent;
        Thread atm,s;
        private ConcurrentQueue<Packet.ATMPacket> kolejkapyt;
        private List<Packet.ATMPacket> kolejkaodp;
        private ConcurrentQueue<Packet.SPacket> kolejkaS;
        String adresLRM, adresRC;

        public eLReMix(Nodix nod)
        {
            parent = nod;
            adresLRM = nod.myAddress.ToString();
            adresRC = null;
            kolejkapyt = new ConcurrentQueue<Packet.ATMPacket>();
            kolejkaodp = new List<Packet.ATMPacket>();
            kolejkaS = new ConcurrentQueue<SPacket>();
            atm = new Thread(new ThreadStart(ATMReader));
            atm.IsBackground = true;
            atm.Start();
            s = new Thread(new ThreadStart(SReader));
            s.IsBackground = true;
            s.Start();

        }
        
        public void ATMReader()//wątek obsługujący odczytywanie pakietów z zapytaniami, te z odpowiedziami będą jakoś(jeszcze nie wiem jak) obsługiwane w CzyZyje
        {
            while (true)
            {
                if(kolejkapyt.Count == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                try
                {
                    ATMPacket zapytanie, odpowiedz;
                    kolejkapyt.TryDequeue(out zapytanie);
                    //obsługa pakietu od innego LRMa, w tej metodzie interesują nas tylko wiadomosci z zapytaniem, odpowiedzi obsługiwane są metodą CzyZyje
                    //więc jak wykryjemy odp to idzie do drugiej kolejki
                    byte[] payload = ToPayload("ZYJE");
                    odpowiedz = new Packet.ATMPacket(Packet.ATMPacket.AALType.SSM, payload, 0, 0);
                    odpowiedz.port = zapytanie.port;
                    odpowiedz.VCI = -1;
                    odpowiedz.VPI = -1;
                    parent.queuedReceivedPackets.Enqueue(odpowiedz);
                }
                catch(Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                }
            }
        }
        public void SReader()
        {
            while (true)
            {
                if (kolejkaS.Count == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                try
                {
                    //przy przyjściu jakiejkolwiek wiadomości od RC wstawić jej adres do pola adresRC
                    //tu zrobić rozpoznawanie wszystkich możliwych wiadomosci i ich obsługę
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                }
            }
        }

        public void OdczytajATM(Packet.ATMPacket pkt)//każdy pakiet, od razu rozpoznaje rodzaj - czy zapytanie czy odpowiedź, innych nie powinno być, najwyzej przepadną, dodawanie do dobrej kolejki
        {
            String tresc = this.FromPayload(pkt.payload);
            if (tresc.Equals("ZYJESZ?"))
            {
                kolejkapyt.Enqueue(pkt);
            }
            else if (tresc.Equals("ZYJE"))
            {
                kolejkaodp.Add(pkt);
            }
           
        }

        public void CzyZyjeRun(Address sprawdzany)//dla każdego zapytania o sprawność łącza od RC odpalany nowy wątek tą metodą
        {
            new Thread(new ParameterizedThreadStart(_CzyZyje));
            s.IsBackground = true;
            s.Start(sprawdzany);
        }

        
        private void _CzyZyje(Object sprawdzanyy)//pod dany adres (jaqk istnieje) wysyłamy ATMPacket z wiadomością ZYJESZ
        {
            int port=0;
            Address sprawdzany = (Address)sprawdzanyy;
            //szukanie portu dla adresu
            for (int i = 0; i < parent.routeList.Count; i++ )
            {
                if(parent.routeList.ElementAt(i).destAddr == sprawdzany)//jak adres się zgadza to mamy szukany port do wysyłki
                {
                    port = parent.routeList.ElementAt(i).port;
                    break;
                }
            }
            if(port==0)
            {
                //Wyslij z automatu no, bo nie ma takiego portu, żeby pakiet doszedł na ten adres
                return;
            }
            //wysyłka pakietu na wyszukanym porcie
            {
                String str = "ZYJESZ?";/////odpowiedzią na taki paylaod będzie ZYJE :)
                byte[] payload = ToPayload(str);
                Packet.ATMPacket packiet = new Packet.ATMPacket(Packet.ATMPacket.AALType.SSM, payload, 0, 0);
                packiet.port = port;
                packiet.VCI = -1;
                packiet.VPI = -1;
                parent.queuedReceivedPackets.Enqueue(packiet);
            }

            //czekanie na odp 
            {
                bool znaleziono=false;
                Stopwatch t = new Stopwatch();
                int maxwaitmilis = 3000; //czekaj do 3 sekund na odpowiedź od sąsiada
                t.Start();
                while (t.ElapsedMilliseconds < maxwaitmilis)
                {
                    //przeszukanie listy pakietów
                    for(int i=0;i<kolejkaodp.Count;i++)
                    {
                        if(kolejkaodp.ElementAt(i).port==port)
                        {
                            znaleziono = true;
                            kolejkaodp.RemoveAt(i);//usuwam bez dalszego zaglądania, bo jak coś siedzi w tej kolejce to ma w payload ZYJE
                            break;//gdy znaleziono to wychodzi z pętli for
                        }

                    }
                    if(znaleziono)
                    {
                        break;//gdy znaleziono to wychodzi z pętli while
                    }
                }
                //obsługa rezultatu, jeśli znaleziono to wyślij YES <adres>, jak nie to NO <adres>
                if(znaleziono)
                {
                    wyslijSPacket(new SPacket(adresLRM,adresRC,"YES "+sprawdzany.ToString()));
                }
                else
                {
                    wyslijSPacket(new SPacket(adresLRM, adresRC, "NO " + sprawdzany.ToString()));
                }

            }
        }
        public void wyslijSPacket(Packet.SPacket cos)//wysyłanie przez chmurę kablową
        {
            parent.whatToSendQueue.Enqueue(cos);
        }
        public void OdczytajS(Packet.SPacket cos)//odbieranie z chmury kablowej
        {
            kolejkaS.Enqueue(cos);
        }
        public byte[] ToPayload(String str)
        {
            return AAL.GetBytesFromString(str);
        }
        public String FromPayload(byte[] payload)
        {
            return AAL.GetStringFromBytes(payload);
        }
        public bool ZwolnijZasob()//zwraca true jak się uda
        {

            return false;
        }
        public bool ZajmijZasob()//zwraca true jak się uda
        {
            return false;
        }
    }
}
