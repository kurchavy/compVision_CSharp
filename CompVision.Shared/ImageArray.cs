﻿using System;
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
        CoordinateTransformer _transformer;

        /// <summary>
        /// Количество строк
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Количество столбцов
        /// </summary>
        public int ColCount { get; private set; }

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
                return _image[_transformer.FromPoint(x, y)];
            }
            set
            {
                _image[_transformer.FromPoint(x, y)] = value;
            }
        }

        /// <summary>
        /// Индексатор доступа к внутреннему массиву
        /// </summary>
        /// <param name="p">Нужная точка (X, Y)</param>
        /// <returns></returns>
        public int this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
            set
            {
                this[p.X, p.Y] = value;
            }
        }

        public IEnumerable<Point> Points
        {
            get
            {
                for (int i = 0; i < _image.Length; i++)
                {
                    yield return _transformer.FromIndex(i);
                }
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
            if (array.Length != rows * cols)
                throw new ArgumentException("Wrong array size");
            _image = array;
            RowCount = rows;
            ColCount = cols;
            _transformer = new CoordinateTransformer(rows, cols);
        }

        /// <summary>
        /// Создание ImageArray
        /// </summary>
        /// <param name="bmpPath">Исходный bitmap</param>
        public ImageArray(string bmpPath)
        {
            Bitmap bmp = new Bitmap(bmpPath);
            RowCount = bmp.Height;
            ColCount = bmp.Width;
            _image = new int[RowCount * ColCount];
            _transformer = new CoordinateTransformer(RowCount, ColCount);

            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColCount; c++)
                {
                    var pix = bmp.GetPixel(c, r);
                    if (pix.R == 255 && pix.G == 255 && pix.B == 255)
                        _image[_transformer.FromRowColumn(r, c)] = 0;
                    else
                        _image[_transformer.FromRowColumn(r, c)] = 1;
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
            inv.ColCount = ColCount;
            inv.RowCount = RowCount;
            inv._image = new int[_image.Length];
            inv._transformer = new CoordinateTransformer(RowCount, ColCount);
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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    result += _image[j + i * ColCount] + " ";
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
