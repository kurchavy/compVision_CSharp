using AR.CompVision;
using AR.CompVision.Binary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedComponents
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] arr = {
                1, 1, 0, 1, 1, 1, 0, 1, 0,
                1, 1, 0, 1, 0, 1, 0, 1, 0,
                1, 1, 1, 1, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 1, 0,
                1, 1, 1, 1, 0, 1, 0, 1, 1,
                0, 0, 0, 1, 0, 1, 0, 1, 0,
                1, 1, 0, 1, 0, 0, 0, 1, 0,
                1, 1, 0, 1, 0, 1, 1, 1, 1,
            };
            var ia = new ImageArray(arr, 8, 9);
            Console.WriteLine("Image 8*9 pix");
            CalcRecursive(ia);
            CalcUnionFind(ia);

            //Console.ReadLine();
            ia = new ImageArray("pics/1.bmp");
            Console.WriteLine("Image 178*124 pix");
            CalcRecursive(ia);
            CalcUnionFind(ia);
            Console.ReadLine();
        }

        static void CalcRecursive(ImageArray ia)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var bip = new BinaryImageProcessor(ia);
            var res = bip.ConnectedComponentsRecursive(PixelNeighborhood.EightConnected);
            sw.Stop();
            Console.WriteLine("Recursive: " + sw.ElapsedMilliseconds);
            //Console.WriteLine(res);
        }

        static void CalcUnionFind(ImageArray ia)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var bip = new BinaryImageProcessor(ia);
            var res = bip.ConnectedComponentsUnionFind(PixelNeighborhood.EightConnected);
            sw.Stop();
            Console.WriteLine("Union-Find: " + sw.ElapsedMilliseconds);
            //Console.WriteLine(res);

        }
    }
}
