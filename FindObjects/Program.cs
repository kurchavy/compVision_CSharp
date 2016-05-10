using AR.CompVision;
using AR.CompVision.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] arr = {
                0, 0, 0, 0, 0,
                0, 1, 0, 1, 0,
                0, 1, 0, 1, 0,
                0, 0, 0, 0, 0,
            };

            Calculate(new ImageArray(arr, 4, 5), "1. ImageArray from int[]:");
            Calculate(new ImageArray(@"pics/1.bmp"), "2. ImageArray from bmp(1):");
            Calculate(new ImageArray(@"pics/2.bmp"), "2. ImageArray from bmp(1):");
        }

        static void Calculate(ImageArray ia, string comment)
        {
            var bip = new BinaryImage(ia);
            Console.WriteLine(comment);
            Console.WriteLine(ia);
            Console.WriteLine("Number of objects - " + bip.CountObjects());
            Console.WriteLine("Press key to continue...");
            Console.ReadLine();
            
        }
    }
}
