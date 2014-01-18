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

        public eLReMix()
        {
            klienty = new List<int>();
        }
        public void setList(List <int> l)
        {
            klienty = new List<int>(l);
        }
        public void OgarnijPakiet(Packet.ATMPacket pkt)
        {
            //obsługa pakietu
        }
        public void Spamuj()
        {
            //wysyła do wszystkich sąsiadów z listy odpowiednie wiadomosci
        }
    }
}
