using AR.CompVision;
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
            List<Point> pts = new List<Point>()
            {
                new Point(8, 6),
                new Point(8, 7),
            };

            ImageArea area = new ImageArea(pts);
            int x, y;
            //var ia = area.ToImageArray(out x, out y);
            var ia = area.ToImageArray();
            Console.WriteLine(ia);
            Console.WriteLine();
            Console.WriteLine("Area: " + area.GetArea());
            Console.WriteLine("Centroid: " + area.GetCentroid());
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
