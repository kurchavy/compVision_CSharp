using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.CompVision
{
    class DstElement
    {
        public int Parent { get; set; }
        public int Rank { get; set; }
        public DstElement(int parent)
        {
            Parent = parent;
            Rank = 0;
        }
    }

    /// <summary>
    /// Структура данных для непересекающихся множеств
    /// </summary>
    public class DisjointSetForest
    {
        private List<DstElement> _forest;
        
        public DisjointSetForest()
        {
            _forest = new List<DstElement>();
            AddSet(1);
        }

        /// <summary>
        /// Добавление нового множества
        /// </summary>
        /// <param name="number"></param>
        public void AddSet(int number)
        {
            while (number >= _forest.Count)
                _forest.Add(new DstElement(_forest.Count));
        }

        /// <summary>
        /// Найти 1 элемент множества, к которому принадлежит аргумент
        /// </summary>
        /// <param name="x">Множество для поиска</param>
        /// <returns></returns>
        public int FindSet(int x)
        {
            if (_forest[x].Parent != x)
                _forest[x].Parent = FindSet(_forest[x].Parent);
            return _forest[x].Parent;
        }

        /// <summary>
        /// Объединить 2 множества, к которым принадлежат аргументы 
        /// </summary>
        /// <param name="x">Элемент 1 множества</param>
        /// <param name="y">Элемент 2 множества</param>
        public void Union(int x, int y)
        {
            LinkTrees(FindSet(x), FindSet(y));
        }

        private void LinkTrees(int x, int y)
        {
            if (_forest[x].Rank > _forest[y].Rank)
                _forest[y].Parent = x;
            else
            {
                _forest[x].Parent = y;
                if (_forest[x].Rank == _forest[y].Rank)
                    _forest[y].Rank += 1;
            }
        }

    }
}
