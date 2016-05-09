using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision.Binary
{
    public class BinaryImageProcessor
    {
        private ImageArray _img;
        
        /// <summary>
        /// Создание процессора из ImageArray
        /// </summary>
        /// <param name="img"></param>
        public BinaryImageProcessor(ImageArray img)
        {
            _img = img;
        }

        /// <summary>
        /// Подсчет объектов на изображении
        /// </summary>
        /// <returns></returns>
        public int CountObjects()
        {
            int cornExt = 0;
            int cornInt = 0;

            for (int i = 0; i<_img.Rows - 1; i++)
            {
                for (int j = 0; j < _img.Cols - 1; j++)
                {
                    if (ExternalCornerMatch(i, j))
                        cornExt++;
                    if (InternalCornerMatch(i, j))
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

            for (int y = 0; y < _img.Rows; y++)
            {
                for (int x = 0; x < _img.Cols; x++)
                {
                    if (img[x, y] == -1)
                    {
                        label += 1;
                        SearchCC(img, label, x, y, connType);
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
            for (int y=0; y < _img.Rows; y++)
            {
                for (int x = 0; x < img.Cols; x++)
                    img[x, y] = 0;

                for (int x = 0; x < img.Cols; x++)
                {
                    if (_img[x, y] == 1)
                    {
                        var pn = GetPriorNeighboorPixels(x, y, connType).Where(p => _img[p.X, p.Y] != 0);
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
                                if (m > img[p.X, p.Y])
                                    m = img[p.X, p.Y];
                            }
                        }
                        img[x, y] = m;
                        foreach (var p in pn)
                        {
                            if (img[p.X, p.Y] != m)
                                dsf.Union(m, img[p.X, p.Y]);
                        }
                    }
                }
            }
            for (int y = 0; y < _img.Rows; y++)
            {
                for (int x = 0; x < img.Cols; x++)
                {
                    if (_img[x, y] == 1)
                        img[x, y] = dsf.FindSet(img[x, y]);
                }
            }
            return img;
        }

        #region Поиск связных компонент (рекурсия)
        private void SearchCC(ImageArray img, int label, int x, int y, PixelNeighborhood connType)
        {
            img[x, y] = label;
            var nSet = GetNeighboorPixels(x, y, connType);
            foreach (var p in nSet)
            {
                if (img[p.X, p.Y] == -1)
                    SearchCC(img, label, p.X, p.Y, connType);
            }
        }

        /// <summary>
        /// Получить всех соседей пикселя
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="connType">Тип соседства</param>
        /// <returns></returns>
        private ICollection<Point> GetNeighboorPixels(int x, int y, PixelNeighborhood connType)
        {
            List<Point> result = new List<Point>();
            if (y > 0)
            {
                if (x > 0 && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x - 1, y - 1));
                result.Add(new Point(x, y - 1));
                if (x < (_img.Cols - 1) && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x + 1, y - 1));
            }
            if (x > 0)
                result.Add(new Point(x - 1, y));
            if (x < (_img.Cols - 1))
                result.Add(new Point(x + 1, y));
            if (y < (_img.Rows - 1))
            {
                if (x > 0 && connType == PixelNeighborhood.EightConnected)
                    result.Add(new Point(x - 1, y + 1));
                result.Add(new Point(x, y + 1));
                if (x < (_img.Cols - 1) && connType == PixelNeighborhood.EightConnected)
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

        private bool ExternalCornerMatch(int r, int c)
        {
            foreach (var pat in ExtCorners)
            {
                if (CheckPattern(pat, r, c))
                    return true;
            }
            return false;
        }

        private bool InternalCornerMatch(int r, int c)
        {
            foreach (var pat in IntCorners)
            {
                if (CheckPattern(pat, r, c))
                    return true;
            }
            return false;
        }

        private bool CheckPattern(int[] patt, int r, int c)
        {
            if (_img[c, r] == patt[0] &&
                _img[c, r + 1] == patt[1] &&
                _img[c + 1, r] == patt[2] &&
                _img[c + 1, r + 1] == patt[3])
                return true;
            return false;
        } 
        #endregion
    }
}
