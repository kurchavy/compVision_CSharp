using AR.CompVision;
using AR.CompVision.Binary;
using System;
using System.Collections.Generic;
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
                1, 1, 0, 1, 1, 1, 0, 1,
                1, 1, 0, 1, 0, 1, 0, 1,
                1, 1, 1, 1, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 1,
                1, 1, 1, 1, 0, 1, 0, 1,
                0, 0, 0, 1, 0, 1, 0, 1,
                1, 1, 0, 1, 0, 0, 0, 1,
                1, 1, 0, 1, 0, 1, 1, 1,
            };
            var bip = new BinaryImageProcessor(new ImageArray(arr, 8, 8));
            var ia = bip.ConnectedComponentsRecursive(PixelNeighborhood.EightConnected);
            Console.WriteLine(ia);
            Console.ReadLine();
        }
    }
}
