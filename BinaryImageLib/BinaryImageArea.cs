using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision.Binary
{
    /// <summary>
    /// Класс для работы с замкнутой областью ImageArray
    /// </summary>
    public class BinaryImageArea : BinaryImage
    {
        List<Point> _points;
        List<Point> _edgePoints;

        public List<Point> EdgePixels
        {
            get
            {
                if (_edgePoints == null)
                    _edgePoints = new List<Point>(GetEdgePixels());
                return _edgePoints;
            }
        }

        public BinaryImageArea(ImageArray img) 
            : base(img)
        {
            _points = new List<Point>(_img.Points.Where(p => _img[p] != 0));
        }

        /// <summary>
        /// Создание области из массива точек
        /// </summary>
        /// <param name="pts"></param>
        public BinaryImageArea(IEnumerable<Point> pts)
        {
            _points = new List<Point>(pts);

            int cols = _points.Max(p => p.X) + 1;
            int rows = _points.Max(p => p.Y) + 1;


            int[] arr = new int[rows * cols];

            _img = new ImageArray(arr, rows, cols);

            foreach (var p in _points)
                _img[p] = 1;
        }

        /// <summary>
        /// Получить "обрезанный" ImageArray (обрезаются пустые строки и столбцы)
        /// </summary>
        /// <param name="minX">Минимальное значение Х (вычитается из всех точек)</param>
        /// <param name="minY">Минимальное значение Y (вычитается из всех точек)</param>
        /// <returns></returns>
        public ImageArray GetReduxImageArray(out int minX, out int minY)
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

        #region Вычисления
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

        /// <summary>
        /// Список пикселей периметра
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Point> GetEdgePixels()
        {
            List<Point> res = new List<Point>();
            for (int y = 0; y < _img.RowCount; y++)
            {
                for (int x = 0; x < _img.ColCount; x++)
                {
                    if (_img[x, y] == 0)
                        continue;
                    if (y == 0 || x == 0 || y == (_img.RowCount - 1) || x == (_img.ColCount - 1))
                    {
                        res.Add(new Point(x, y));
                    }
                    else
                    {
                        var nset = GetNeighboorPixels(x, y, PixelNeighborhood.EightConnected);
                        foreach (var p in nset)
                        {
                            if (_img[p] == 0)
                            {
                                res.Add(new Point(x, y));
                                break;
                            }
                        }
                    }
                    
                }
            }
            return res;
        }

        /// <summary>
        /// Среднее радиальное расстояние точек периметра
        /// </summary>
        /// <returns></returns>
        public double GetMeanRadialDistance()
        {
            var c = GetCentroid();
            double mu = 0;
            foreach (var p in EdgePixels)
            {
                mu += GetDistance(p, c);
            }
            return mu / EdgePixels.Count;
        }
        /// <summary>
        /// Среднеквадратическое отклонение радиального расстояния
        /// </summary>
        /// <returns></returns>
        public double GetMeanRadialDistanceStd(double calcMu = double.MinValue)
        {
            var c = GetCentroid();
            
            var mu = calcMu != double.MinValue ? calcMu : GetMeanRadialDistance();
            double delta = 0;
            foreach (var p in EdgePixels)
            {
                delta += Math.Pow((GetDistance(p, c) - mu), 2);
            }
            return Math.Sqrt(delta / EdgePixels.Count);
        }

        /// <summary>
        /// Округлость обласи (Харалик)
        /// </summary>
        /// <returns></returns>
        public double GetCircularity()
        {
            var mu = GetMeanRadialDistance();
            var std = GetMeanRadialDistanceStd(mu);
            return mu / std;
        }

        /// <summary>
        /// Расстояние между точками
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public double GetDistance(Point p, PointF c)
        {
            return Math.Sqrt((Math.Pow(p.X - c.X, 2) + Math.Pow(p.Y - c.Y, 2)));
        }
        #endregion
    }
}
