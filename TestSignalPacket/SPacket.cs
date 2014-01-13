using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestSignalPacket 
{
    class SPacket : ISerializable
    {
        private String src;
        private String dest;
        private List<String> parames;

        public SPacket()
        {
            src = "-";
            dest = "-";
            parames = new List<string>();
        }
        public SPacket(String s, String d, List<String> p)
        {
            src = (String)s.Clone();
            dest = (String)d.Clone();
            parames = new List<String>(p);
        }
        public void addParam(String a)
        {
            if (parames == null)
            {
                Console.Out.WriteLine("Wskaźnik na listę parametrów jest null");
                return;
            }
            else
            {
                parames.Add(a);
                Console.Out.WriteLine("Pomyślnie dodało parametr");
            }
        }
        public String ToString()
        {
            String result= "from: " + src + " to: " + dest + "params: ";
            for (int i = 0; i < parames.Count; i++ )
            {
                result += parames.ElementAt(i) + " | ";
            }
                return result;
        }
        public String getSrc()
        {
            return this.src;
        }
        public void setSrc(String s)
        {
            this.src = s;
        }
        public String getDest()
        {
            return this.dest;
        }
        public void setDest(String d)
        {
            this.dest = d;
        }
        public List<String> getParames()
        {
            return this.parames;
        }
        public void setParames(List<String> AL)
        {
            this.parames = AL;
        }
    }
}
