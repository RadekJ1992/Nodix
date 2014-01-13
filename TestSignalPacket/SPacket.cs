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
        private String src {get; set;}
        private String dest { get; set; }
        private List<String> parames { get; set; }

        SPacket()
        {
            src = null;
            dest = null;
            parames = null;
        }
        SPacket(String s, String d, List<String> p)
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

    }
}
