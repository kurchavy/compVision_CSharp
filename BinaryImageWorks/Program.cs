using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision.Binary
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageArray ia = new ImageArray(@"pics/2.bmp");
            Console.WriteLine(ia);
            Console.WriteLine("");
            var bip = new BinaryImageProcessor(ia);
            Console.WriteLine("Number of objects - " + bip.CountObjects());
            Console.ReadLine();
        }
    }
}
