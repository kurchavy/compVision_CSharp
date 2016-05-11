using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision.Binary
{
    public class BinaryImage
    {
        protected ImageArray _img;
        
        /// <summary>
        /// Создание процессора из ImageArray
        /// </summary>
        /// <param name="img"></param>
        public BinaryImage(ImageArray img)
        {
            _img = img;
        }

        protected BinaryImage() { }

        public ImageArray ImageArray { get { return _img; } }

        /// <summary>
        /// Подсчет объектов на изображении
        /// </summary>
        /// <returns></returns>
        public int CountObjects()
        {
            int cornExt = 0;
            int cornInt = 0;

            for (int y = 0; y<_img.RowCount - 1; y++)
            {
                for (int x = 0; x < _img.ColCount - 1; x++)
                {
                    if (ExternalCornerMatch(x, y))
                        cornExt++;
                    if (InternalCornerMatch(x, y))
                        cornInt++;
                }
            }
            return (cornExt - cornInt) / 4;
        }

        /// <summary>
        /// Рекурсивный поиск связных компонент
        /// </summary>
        /// <returns></returns>
        public ImageArray ConnectedComponentsRecursive(PixelNeighborhood connType)
        {
            var img = _img.Negate();
            int label = 0;

            for (int y = 0; y < _img.RowCount; y++)
            {
                for (int x = 0; x < _img.ColCount; x++)
                {
                    if (img[x, y] == -1)
                    {
                        label += 1;
                        SearchCC(img, label, new Point(x, y), connType);
                    }
                }
            }

            return img;
        }

        /// <summary>
        /// Поиск связных компонент с помощью Union-Find
        /// </summary>
        /// <param name="connType"></param>
        /// <returns></returns>
        public ImageArray ConnectedComponentsUnionFind(PixelNeighborhood connType)
        {
            ImageArray img = _img.Copy();
            DisjointSetForest dsf = new DisjointSetForest();

            int label = 1;
            for (int y=0; y < _img.RowCount; y++)
            {
                for (int x = 0; x < img.ColCount; x++)
                    img[x, y] = 0;

                for (int x = 0; x < img.ColCount; x++)
                {
                    if (_img[x, y] == 1)
                    {
                        var pn = GetPriorNeighboorPixels(x, y, connType).Where(p => _img[p] != 0);
                        int m = int.MaxValue;
                        if (pn.Count() == 0)
                        {
                            m = label;
                            dsf.AddSet(m);
                            label += 1;
                        }
                        else
                        {
                            foreach (var p in pn)
                            {
                                if (m > img[p])
                                    m = img[p];
                            }
                        }
                        img[x, y] = m;
                        foreach (var p in pn)
                        {
                            if (img[p] != m)
                                dsf.Union(m, img[p]);
                        }
                    }
                }
            }
            for (int y = 0; y < _img.RowCount; y++)
            {
                for (int x = 0; x < img.ColCount; x++)
                {
                    if (_img[x, y] == 1)
                        img[x, y] = dsf.FindSet(img[x, y]);
                }
            }
            return img;
        }

        #region Поиск связных компонент (рекурсия)
        private void SearchCC(ImageArray img, int label, Point pt, PixelNeighborhood connType)
        {
            img[pt] = label;
            var nSet = GetNeighboorPixels(pt.X, pt.Y, connType);
            foreach (var p in nSet)
            {
                if (img[p] == -1)
                    SearchCC(img, label, p, connType);
            }
        }

        /// <summary>
        /// Получить всех соседей пикселя
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="connType">Тип соседства</param>
        /// <returns></returns>
        protected ICollection<Point> GetNeighboorPixels(int x, int y, PixelNeighborhood connType)
        {
            List<Point> result = new List<Point>();
            if (y > 0)
            {
                if (x > 0 && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x - 1, y - 1));
                result.Add(new Point(x, y - 1));
                if (x < (_img.ColCount - 1) && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x + 1, y - 1));
            }
            if (x > 0)
                result.Add(new Point(x - 1, y));
            if (x < (_img.ColCount - 1))
                result.Add(new Point(x + 1, y));
            if (y < (_img.RowCount - 1))
            {
                if (x > 0 && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x - 1, y + 1));
                result.Add(new Point(x, y + 1));
                if (x < (_img.ColCount - 1) && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x + 1, y + 1));
            }
            return result;
        }

        #endregion

        #region Поиск связных компонент (классика)
        /// <summary>
        /// Получить левых и верхних соседей пикселя
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="connType">Тип соседства</param>
        /// <returns></returns>
        private ICollection<Point> GetPriorNeighboorPixels(int x, int y, PixelNeighborhood connType)
        {
            List<Point> result = new List<Point>();
            if (y > 0)
            {
                if (x > 0 && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x - 1, y - 1));
                result.Add(new Point(x, y - 1));
            }
            if (x > 0)
                result.Add(new Point(x - 1, y));
            return result;
        }
        #endregion

        #region Подсчет углов
        private readonly int[][] ExtCorners = new int[][]
        {
            new int[] { 0,0,0,1 },
            new int[] { 0,0,1,0 },
            new int[] { 1,0,0,0 },
            new int[] { 0,1,0,0 },
        };

        private readonly int[][] IntCorners = new int[][]
        {
            new int[] { 1,1,1,0 },
            new int[] { 1,1,0,1 },
            new int[] { 1,0,1,1 },
            new int[] { 0,1,1,1 },
        };

        private bool ExternalCornerMatch(int x, int y)
        {
            foreach (var pat in ExtCorners)
            {
                if (CheckPattern(pat, x, y))
                    return true;
            }
            return false;
        }

        private bool InternalCornerMatch(int x, int y)
        {
            foreach (var pat in IntCorners)
            {
                if (CheckPattern(pat, x, y))
                    return true;
            }
            return false;
        }

        private bool CheckPattern(int[] patt, int x, int y)
        {
            if (_img[x, y] == patt[0] &&
                _img[x, y + 1] == patt[1] &&
                _img[x + 1, y] == patt[2] &&
                _img[x + 1, y + 1] == patt[3])
                return true;
            return false;
        }
        #endregion
    }
}
