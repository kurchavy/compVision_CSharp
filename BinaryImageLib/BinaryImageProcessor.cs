using System;
using System.Collections.Generic;
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
