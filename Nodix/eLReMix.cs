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

namespace Nodix
{
    class eLReMix
    {
        Nodix parent;
        Thread atm,s;
        private ConcurrentQueue<Packet.ATMPacket> kolejkapyt;
        private ConcurrentQueue<Packet.ATMPacket> kolejkaodp;
        private ConcurrentQueue<Packet.SPacket> kolejkaS;

        public eLReMix(Nodix nod)
        {
            parent = nod;
            kolejkapyt = new ConcurrentQueue<Packet.ATMPacket>();
            kolejkaodp = new ConcurrentQueue<Packet.ATMPacket>();
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
                kolejkaodp.Enqueue(pkt);
            }
        }
        public void CzyZyje(Address sprawdzany)//pod dany adres (jaqk istnieje) wysyłamy ATMPacket z wiadomością ZYJESZ
        {
            int port=0;
            
            //szukanie portu dla adresu
            for (int i = 0; i < parent.routeList.Count; i++ )
            {
                if(parent.routeList.ElementAt(i).destAddr == sprawdzany)//jak adres się zgadza to mamy szukany port do wysyłki
                {
                    port = parent.routeList.ElementAt(i).port;
                    break;
                }
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

            //czekanie na odp TODO
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
    }
}
