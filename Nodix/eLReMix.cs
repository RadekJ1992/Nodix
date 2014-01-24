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
        Thread atm;
        private ConcurrentQueue<Packet.ATMPacket> kolejkapyt;
        private ConcurrentQueue<Packet.ATMPacket> kolejkaodp;

        public eLReMix(Nodix nod)//ustawienie parenta
        {
            parent = nod;
            kolejkapyt = new ConcurrentQueue<Packet.ATMPacket>();
            kolejkaodp = new ConcurrentQueue<Packet.ATMPacket>();
            atm = new Thread(new ThreadStart(ATMReader));
            atm.IsBackground = true;
            atm.Start();
        }
        
        public void ATMReader()
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
                    ATMPacket pckt;
                    kolejkapyt.TryDequeue(out pckt);
                    //obsługa pakietu od innego LRMa, w tej metodzie interesują nas tylko wiadomosci z zapytaniem, odpowiedzi obsługiwane są metodą CzyZyje
                    //więc jak wykryjemy odp to idzie do drugiej kolejki
                    int port = pkt.port;
                    byte[] payload = pkt.payload;
                    string result = System.Text.Encoding.UTF8.GetString(payload);
                    AddressLibrary.Address adres = AddressLibrary.Address.Parse(result);
                    if (result.Equals("ZYJESZ?"))//zidentyfikowano wartość
                    {

                    }
                    //rozczytanie komendy i dalsze działania, w tym wysyłanie do RC 
                }
                catch
                {

                }
            }
        }

        public void KolejkujPakiet(Packet.ATMPacket pkt)//każdy pakiet, od razu rozpoznaje rodzaj - czy zapytanie czy odpowiedź, innych nie powinno być, najwyzej przepadną
        {
            String tresc = this.FromPayload(pkt.payload);
            if(tresc.Equals("ZYJESZ?"))
                kolejkapyt.Enqueue(pkt);
            else if(tresc.Equals("ZYJE"))
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
            //w Nodixie będzie kolejka public whatToSendQueue, musisz tylko stworzyć pakiet i zrobić parent.whatToSendQueue.Enquque(packet)
        }
        public void OdczytajSPacket(Packet.SPacket cos)//odbieranie z chmury kablowej
        {
            //w nodixie będzie atuomatycznie przekazywać odebrane pakiety tutaj. Obrabiaj je sobie jak Ci pasuje
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
