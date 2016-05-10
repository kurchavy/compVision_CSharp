using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision
{
    public class ImageArray
    {
        int[] _image;

        /// <summary>
        /// Количество строк
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// Количество столбцов
        /// </summary>
        public int Cols { get; private set; }

        /// <summary>
        /// Индексатор доступа к внутреннему массиву
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <returns></returns>
        public int this[int x, int y]
        {
            get
            {
                //return _image[x * Cols + y];
                return _image[x + y * Cols];
            }
            set
            {
                //_image[x * Cols + y] = value;
                _image[x + y * Cols] = value;
            }
        }

        private ImageArray() { }

        /// <summary>
        /// Создание ImageArray
        /// </summary>
        /// <param name="array">Одномерный массив пикселов</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="cols">Количество столбцов</param>
        public ImageArray(int[] array, int rows, int cols)
        {
            //if (array.Length != rows * cols)
            //    throw new ArgumentException("Wrong array size");
            _image = array;
            Rows = rows;
            Cols = cols;
        }

        /// <summary>
        /// Создание ImageArray
        /// </summary>
        /// <param name="bmpPath">Исходный bitmap</param>
        public ImageArray(string bmpPath)
        {
            Bitmap bmp = new Bitmap(bmpPath);
            Rows = bmp.Height;
            Cols = bmp.Width;
            _image = new int[Rows * Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var pix = bmp.GetPixel(j, i);
                    if (pix.R == 255 && pix.G == 255 && pix.B == 255)
                        _image[i * Cols + j] = 0;
                    else
                        _image[i * Cols + j] = 1;
                }
            }
        }

        /// <summary>
        /// Копирование ImageArray
        /// </summary>
        /// <returns></returns>
        public ImageArray Copy()
        {
            ImageArray inv = new ImageArray();
            inv.Cols = Cols;
            inv.Rows = Rows;
            inv._image = new int[_image.Length];
            Array.Copy(_image, inv._image, _image.Length);
            return inv;
        }

        /// <summary>
        /// Инвертирование ImageArray
        /// </summary>
        /// <returns></returns>
        public ImageArray Negate()
        {
            var inv = Copy();
            inv.TransformPixels(i => -i);
            return inv;
        }

        /// <summary>
        /// Возврат строкового представления ImageArray
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    result += _image[j + i * Cols] + " ";
                }
                result += Environment.NewLine;
            }
            return result;
        }

        /// <summary>
        /// Трансформация пикселей массива
        /// </summary>
        /// <param name="tf">Функция трансформирования</param>
        private void TransformPixels(Func<int, int> tf)
        {
            for (int i = 0; i < _image.Length; i++)
            {
                _image[i] = tf(_image[i]);
            }
        }
    }
}
