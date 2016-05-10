using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision
{
    public class ImageArea
    {
        List<Point> _points;
        ImageArray _imgArray;

        public ImageArea(IEnumerable<Point> pts)
        {
            _points = new List<Point>(pts);
        }

        /// <summary>
        /// Площадь области
        /// </summary>
        /// <returns></returns>
        public int GetArea()
        {
            return _points.Count;
        }

        /// <summary>
        /// Центр тяжести
        /// </summary>
        /// <returns></returns>
        public PointF GetCentroid()
        {
            var pt = new PointF();
            pt.X = (float)_points.Average(p => (double)p.X);
            pt.Y = (float)_points.Average(p => (double)p.Y);
            return pt;
        }

        public ImageArray ToImageArray()
        {
            if (_imgArray == null)
            {
                int cols = _points.Max(p => p.X) + 1;
                int rows = _points.Max(p => p.Y) + 1;


                int[] arr = new int[rows * cols];

                _imgArray = new ImageArray(arr, rows, cols);
                foreach (var p in _points)
                    _imgArray[p.X, p.Y] = 1;
            }
            
            return _imgArray;
        }

        public ImageArray ToImageArray(out int minX, out int minY)
        {
            minX = _points.Min(p => p.X);
            minY = _points.Min(p => p.Y);

            int maxX = _points.Max(p => p.X);
            int maxY = _points.Max(p => p.Y);

            int rows = maxY - minY + 1;
            int cols = maxX - minX + 1;

            int[] arr = new int[rows * cols];

            var ia = new ImageArray(arr, rows, cols);
            foreach (var p in _points)
                ia[p.X - minX, p.Y - minY] = 1;
            return ia;
        }
    }
}
