﻿using System;
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
            //obsługa pakietu od innego LRMa
            int port = pkt.port;
            byte[] payload = pkt.payload;
        }
        public void Spamuj()
        {
            //wysyła do wszystkich sąsiadów z listy odpowiednie wiadomosci do LRMów
            for (int i = 0; i < klienty.Count;i++ )
            {

                String str = "";
                byte[] payload = new byte[str.Length * sizeof(char)];
                System.Buffer.BlockCopy(str.ToCharArray(), 0, payload, 0, payload.Length);
                Packet.ATMPacket packiet = new Packet.ATMPacket(Packet.ATMPacket.AALType.SSM, payload, 0, 0);
                packiet.port = klienty.ElementAt(i);
                packiet.VCI = -1;
                packiet.VPI = -1;
                parent.queuedReceivedPackets.Enqueue(packiet);
            }
        }
        public void wyslijSPacket(Packet.SPacket cos)
        {
            
        }
        
    }
}