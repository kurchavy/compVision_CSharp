using AR.CompVision;
using AR.CompVision.Binary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var area = new BinaryImageArea(new ImageArray(@"pics/1.bmp"));
            Console.WriteLine(area.ImageArray);
            Console.WriteLine();
            Console.WriteLine("Area: " + area.GetArea());
            Console.WriteLine("Centroid: " + area.GetCentroid());
            Console.WriteLine("Radial distance: " + area.GetMeanRadialDistance());
            Console.WriteLine("Radial distance StDev: " + area.GetMeanRadialDistanceStd());
            Console.WriteLine("Circularity: " + area.GetCircularity());
            Console.ReadLine();
        }
    }
}
