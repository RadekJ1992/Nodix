using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TestSignalPacket
{
    class Program
    {
        static void Main(string[] args)
        {
            SPacket sp = new SPacket();
            sp.setSrc("LRM1");
            sp.setDest("RC");
            sp.addParam("LINK");
            sp.addParam("1.5.6");
            sp.addParam("23");
            sp.addParam("5.3.3");
            sp.addParam("23");
            sp.addParam("dupa");
            sp.addParam("2dupa");
            Console.Out.WriteLine(sp);
            Console.Out.WriteLine("????????????????????????????????????????????????????????????????");
            IFormatter formatter = new BinaryFormatter();
            SerializeItem("D:\\SPacket.txt", formatter, sp);
            DeserializeItem("D:\\SPacket.txt", formatter, sp);
            Console.Out.WriteLine(sp);
            Console.Out.WriteLine("????????????????????????????????????????????????????????????????");
        }
        public static void SerializeItem(string fileName, IFormatter formatter, SPacket t)
        {
            

            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, t);
            s.Close();
        }


        public static void DeserializeItem(string fileName, IFormatter formatter, SPacket t)
        {
            FileStream s = new FileStream(fileName, FileMode.Open);
            t = (SPacket)formatter.Deserialize(s);
            
        }       
    }
}
