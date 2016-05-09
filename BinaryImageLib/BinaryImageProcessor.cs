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

            for (int i = 0; i < _img.Rows; i++)
            {
                for (int j = 0; j < _img.Cols; j++)
                {
                    if (img[i, j] == -1)
                    {
                        label += 1;
                        SearchCC(img, label, i, j, connType);
                    }
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
            if (y < (_img.Cols - 1))
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
            if (_img[r, c] == patt[0] &&
                _img[r, c + 1] == patt[1] &&
                _img[r + 1, c] == patt[2] &&
                _img[r + 1, c + 1] == patt[3])
                return true;
            return false;
        } 
        #endregion
    }
}
