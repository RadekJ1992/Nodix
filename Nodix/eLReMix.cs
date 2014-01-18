using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodix
{
    class eLReMix
    {
        List<int> klienty;
        Nodix parent;

        public eLReMix(Nodix nod)//ustawienie parenta
        {
            klienty = new List<int>();
            parent = nod;
        }
        public void setList(List <int> l)
        {
            klienty = new List<int>(l);
        }
        public void OgarnijPakiet(Packet.ATMPacket pkt)
        {
            //obsługa pakietu
            int port = pkt.port;
            byte[] payload = pkt.payload;
        }
        public void Spamuj()
        {
            //wysyła do wszystkich sąsiadów z listy odpowiednie wiadomosci
            byte[] payload={new byte()};
            parent.queuedReceivedPackets.Enqueue(new Packet.ATMPacket(Packet.ATMPacket.AALType.SSM, payload, 0,0);
        }
        public void wyslijSPacket(Packet.SPacket cos)
        {
            
        }
        
    }
}
