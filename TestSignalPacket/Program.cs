using System;
using System.Collections.Generic;
using System.Linq;
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
        }
    }
}
