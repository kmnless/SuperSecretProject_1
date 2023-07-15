namespace Generation
{
    public class RoadGenerator
    {
        private int[,] matrix;

        private int iterationCount;
        private int matrixX;
        private int matrixY;

        private HashSet<Tuple<int, int>> points;
        private Dictionary<Tuple<int,int>,HashSet<Tuple<int,int>>> depend;
        public RoadGenerator(HashSet<Tuple<int, int>> basesPoses, HashSet<Tuple<int, int>> flagsPoses, int matrixSizeX, int matrixSizeY, int[,] matrix, int iterationCount = 2)
        {
            depend = new();
            this.matrix = matrix;
            matrixX = matrixSizeX;
            matrixY = matrixSizeY;
            points = new HashSet<Tuple<int, int>>();
            foreach (Tuple<int, int> pos in flagsPoses)
            {
                depend[pos] = new HashSet<Tuple<int, int>>();
                points.Add(pos);
            }
            foreach (Tuple<int, int> pos in basesPoses)
            {
                depend[pos] = new HashSet<Tuple<int, int>>();
                points.Add(pos);
            }

            this.iterationCount = iterationCount;

        }

        public override string ToString()
        {
            string r = "";
            for (int i = 0; i < matrixY; i++)
            {
                for (int j = 0; j <matrixX; j++)
                {
                    r += $"{matrix[i, j]}";
                }
                r += "\n";
            }
            return r;
        }
        private int calculateDistance(Tuple<int, int> pos1, Tuple<int, int> pos2)
        {
            return Convert.ToInt32(Math.Sqrt((pos2.Item1 - pos1.Item1) * (pos2.Item1 - pos1.Item1) + (pos2.Item2 - pos1.Item2) * (pos2.Item2 - pos1.Item2)));
        }
        private void connectWithNearest(Tuple<int, int> bonePoint)
        {
            Tuple<Tuple<int, int>, int> minDist = new(new(-1, -1), int.MaxValue);
            foreach (Tuple<int, int> pos in points)
            {
                int dist = calculateDistance(pos, bonePoint);
                if (dist < minDist.Item2 && dist != 0 && !depend[bonePoint].Contains(pos))
                {
                    minDist = new(pos, dist);
                }
            }
            ConnectPoints(minDist.Item1, bonePoint);
        }
        public void connectAllObjectives()
        {
            for (int i = 0; i < iterationCount; ++i)
            {
                foreach (Tuple<int, int> pos in points)
                {
                    connectWithNearest(pos);
                }
            }
        }
        private void ConnectPoints(Tuple<int, int> start, Tuple<int, int> end)
        {

            int startX = start.Item1;
            int startY = start.Item2;
            int endX = end.Item1;
            int endY = end.Item2;
            if (startX < 0 || startY < 0 || endX < 0 || endY < 0) { return; }
            List<int[]> path = new List<int[]>();
            int currentX = startX;
            int currentY = startY;

            Random random = new Random();

            while (currentX != endX || currentY != endY)
            {
                path.Add(new int[] { currentX, currentY });

                // Выбор следующей точки на основе соседних клеток
                int nextX = currentX;
                int nextY = currentY;

                // Рандомный выбор следующей точки
                if (currentX < endX)
                {
                    nextX += random.Next(0, 2);
                }
                else if (currentX > endX)
                {
                    nextX -= random.Next(0, 2);
                }

                if (currentY < endY)
                {
                    nextY += random.Next(0, 2);
                }
                else if (currentY > endY)
                {
                    nextY -= random.Next(0, 2);
                }

                // Обновление текущих координат
                currentX = nextX;
                currentY = nextY;
            }

            // Добавление конечной точки в путь
            path.Add(new int[] { currentX, currentY });

            // Создание округленного пути
            List<int[]> roundedPath = new List<int[]>();

            for (int i = 0; i < path.Count - 1; i++)
            {
                int[] currentPoint = path[i];
                int[] nextPoint = path[i + 1];

                int dx = nextPoint[0] - currentPoint[0];
                int dy = nextPoint[1] - currentPoint[1];

                if (dx != 0 && dy != 0) // Если движение по диагонали
                {
                    int randomDirection = random.Next(0, 2); // Рандомный выбор направления округления

                    if (randomDirection == 0)
                    {
                        roundedPath.Add(new int[] { currentPoint[0], nextPoint[1] }); // Добавить горизонтальный переход
                    }
                    else
                    {
                        roundedPath.Add(new int[] { nextPoint[0], currentPoint[1] }); // Добавить вертикальный переход
                    }
                }
                roundedPath.Add(nextPoint); // Добавить точку
            }

            foreach (int[] point in path)
            {
                int x = point[0];
                int y = point[1];
                if (x >= 0 && y >= 0 && x < this.matrixX && y < this.matrixY)
                {
                    if (matrix[x, y] == 0)
                    {
                        matrix[x, y] = 2;
                    }

                }
            }

            depend[start].Add(end);
            depend[end].Add(start);
        }
    }


}

