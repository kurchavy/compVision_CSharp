using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision
{
    public class CoordinateTransformer
    {
        int _rows, _cols;
        public CoordinateTransformer(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
        }

        public int FromPoint(int x, int y)
        {
            return y * _cols + x;
        }

        public int FromPoint(Point p)
        {
            return FromPoint(p.X, p.Y);
        }

        public int FromRowColumn(int row, int column)
        {
            return row * _cols + column;
        }

        public Point FromIndex(int idx)
        {
            int y = idx / _cols;
            return new Point(idx - y * _cols, y);
        }
    }
}
