using System;
using System.Collections.Generic;

namespace Finder
{
    public class LeeAlgorithm
    {
        public int[,] ArrayGraph { get; private set; }
        /// <summary>
        /// Начало пути с конца списка
        /// </summary>
        public List<Tuple<int, int>> Path { get; private set; }
        public int Width { get; private set; }
        public int Heidth { get; private set; }
        public bool PathFound { get; private set; }
        public int LengthPath { get { return Path.Count; } }

        private int _step;
        private bool _finishingCellMarked;
        private int _finishPointI;
        private int _finishPointJ;

        /// <summary>
        /// Инициализирует новый экземпляр объекта с полем и указанием начальной точки
        /// </summary>
        public LeeAlgorithm(int startX, int startY, int[,] array)
        {
            ArrayGraph = array;
            Width = ArrayGraph.GetLength(0);
            Heidth = ArrayGraph.GetLength(1);
            SetStarCell(startX, startY);
            PathFound = PathSearch();
        }

        /// <summary>
        /// Инициализирует новый экземпляр объекта с полем. Начальной точка установлена в массиве
        /// </summary>
        public LeeAlgorithm(int[,] array)
        {
            ArrayGraph = array;
            Width = ArrayGraph.GetLength(0);
            Heidth = ArrayGraph.GetLength(1);
            int startX;
            int startY;
            FindStartCell(out startX, out startY);
            SetStarCell(startX, startY);
            PathFound = PathSearch();

        }

        private void FindStartCell(out int startX, out int startY)
        {
            int w = Width;
            int h = Heidth;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (ArrayGraph[i, j] == (int)Figures.StartPosition)
                    {
                        startX = i;
                        startY = j;
                        return;
                    }
                }
            }
            throw new AggregateException("Нет начальной точки");
        }

        private void SetStarCell(int startX, int startY)
        {
            if (startX > this.ArrayGraph.GetLength(0) || startX < 0)
                throw new ArgumentException("Неправильная координата x");
            if (startY > this.ArrayGraph.GetLength(1) || startY < 0)
                throw new ArgumentException("Неправильная координата x");
            //Пометить стартовую ячейку d:= 0
            _step = 0;
            ArrayGraph[startX, startY] = _step;
        }

        private bool PathSearch()
        {
            if (WavePropagation())
                if (RestorePath())
                    return true;

            return false;
        }

        // Есть. Ортогональный путь
        // Todo. Ортогонально-диагональный путь

        /// <summary>
        /// Распространение волны
        /// </summary>
        /// <returns></returns>
        private bool WavePropagation()
        {
            //ЦИКЛ
            //  ДЛЯ каждой ячейки loc, помеченной числом d
            //    пометить все соседние свободные непомеченные ячейки числом d + 1
            //  КЦ
            //  d := d + 1
            //ПОКА(финишная ячейка не помечена) И(есть возможность распространения волны)

            int w = Width;
            int h = Heidth;

            bool finished = false;
            do
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (ArrayGraph[i, j] == _step)
                        {
                            // Пометить все соседние свободные непомеченные ячейки числом d + 1
                            if (i != w - 1)
                                if (ArrayGraph[i + 1, j] == (int)Figures.EmptySpace) ArrayGraph[i + 1, j] = _step + 1;
                            if (j != h - 1)
                                if (ArrayGraph[i, j + 1] == (int)Figures.EmptySpace) ArrayGraph[i, j + 1] = _step + 1;
                            if (i != 0)
                                if (ArrayGraph[i - 1, j] == (int)Figures.EmptySpace) ArrayGraph[i - 1, j] = _step + 1;
                            if (j != 0)
                                if (ArrayGraph[i, j - 1] == (int)Figures.EmptySpace) ArrayGraph[i, j - 1] = _step + 1;

                            // Путь до финиша проложен
                            if (i < w - 1)
                                if (ArrayGraph[i + 1, j] == (int)Figures.Destination)
                                {
                                    _finishPointI = i + 1;
                                    _finishPointJ = j;
                                    finished = true;
                                }
                            if (j < h - 1)
                                if (ArrayGraph[i, j + 1] == (int)Figures.Destination)
                                {
                                    _finishPointI = i;
                                    _finishPointJ = j + 1;
                                    finished = true;
                                }
                            if (i > 0)
                                if (ArrayGraph[i - 1, j] == (int)Figures.Destination)
                                {
                                    _finishPointI = i - 1;
                                    _finishPointJ = j;
                                    finished = true;
                                }
                            if (j > 0)
                                if (ArrayGraph[i, j - 1] == (int)Figures.Destination)
                                {
                                    _finishPointI = i;
                                    _finishPointJ = j - 1;
                                    finished = true;
                                }
                        }

                    }
                }
                _step++;
                //ПОКА(финишная ячейка не помечена) И(есть возможность распространения волны)
            } while (!finished && _step < w * h);
            _finishingCellMarked = finished;
            return finished;
        }

        /// <summary>
        ///  Восстановление пути
        /// </summary>
        /// <returns></returns>
        private bool RestorePath()
        {
            // ЕСЛИ финишная ячейка помечена
            // ТО
            //   перейти в финишную ячейку
            //   ЦИКЛ
            //     выбрать среди соседних ячейку, помеченную числом на 1 меньше числа в текущей ячейке
            //     перейти в выбранную ячейку и добавить её к пути
            //   ПОКА текущая ячейка — не стартовая
            //   ВОЗВРАТ путь найден
            // ИНАЧЕ
            //   ВОЗВРАТ путь не найден
            if (!_finishingCellMarked)
                return false;

            int w = Width;
            int h = Heidth;
            int i = _finishPointI;
            int j = _finishPointJ;
            Path = new List<Tuple<int, int>>();
            AddToPath(i, j);

            do
            {
                if (i < w - 1)
                    if (ArrayGraph[i + 1, j] == _step - 1)
                    {
                        AddToPath(++i, j);
                    }
                if (j < h - 1)
                    if (ArrayGraph[i, j + 1] == _step - 1)
                    {
                        AddToPath(i, ++j);
                    }
                if (i > 0)
                    if (ArrayGraph[i - 1, j] == _step - 1)
                    {
                        AddToPath(--i, j);
                    }
                if (j > 0)
                    if (ArrayGraph[i, j - 1] == _step - 1)
                    {
                        AddToPath(i, --j);
                    }
                _step--;
            } while (_step != 0);
            return true;
        }

        private void AddToPath(int x, int y)
        {
            Path.Add(new Tuple<int, int>(x, y));
        }
    }
}
