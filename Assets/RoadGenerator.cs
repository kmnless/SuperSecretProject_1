using System;
using System.Collections.Generic;
using System.Linq;

namespace Generation
{
    public class RoadGenerator
    {
        public int[,] matrix { get; }

        private int iterationCount;
        private int matrixX;
        private int matrixY;
        private int seed;

        private HashSet<Tuple<int, int>> points;
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> depend;
        public RoadGenerator(HashSet<Tuple<int, int>> basesPoses, HashSet<Tuple<int, int>> flagsPoses, HashSet<Tuple<int, int>> interestPointsPoses, int matrixSizeX, int matrixSizeY, int[,] matrix, int seed, int iterationCount = 2)
        {
            this.seed = seed;
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
            foreach (Tuple<int, int> pos in interestPointsPoses)
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
                for (int j = 0; j < matrixX; j++)
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

        private void makeSureAllPointsAreReacheble() 
        {
            foreach(Tuple<int, int> pos1 in points)
            {
                foreach(Tuple<int,int> pos2 in points)
                {
                    if (!depend[pos1].Contains(pos2) && pos1 != pos2 && !canReach(pos1, pos2))
                    {
                        ConnectPoints(pos1, pos2);
                    }
                }
            }
        }
        
        private bool canReach(Tuple<int,int> start, Tuple<int,int> end) 
        {
            bool[,] visited = new bool[matrixY, matrixX];
            Queue<Tuple<int, int>> possibleMoves = new Queue<Tuple<int, int>>();
            possibleMoves.Enqueue(start);
            visited[start.Item2, end.Item1] = true;
            while (possibleMoves.Count > 0)
            {
                int x = possibleMoves.Peek().Item2;
                int y = possibleMoves.Peek().Item1;
                if (y == end.Item1&&x==end.Item2) 
                {
                    return true;
                }
                possibleMoves.Dequeue();
                for (int i = y - 1; i < y + 2; i++)
                {
                    for (int j = x - 1; j < x + 2; j++)
                    {
                        if (i >= 0 && i < matrixY && j >= 0 && j < matrixX && matrix[i, j] != 0 && !visited[i, j])
                        {
                            possibleMoves.Enqueue(new(i, j)); visited[i, j] = true;
                        }
                    }
                }
            }
            return false;
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
            makeSureAllPointsAreReacheble();
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

            Random random = new Random(seed);

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
                //else{nextX -= random.Next(-6, 7) / 6;}

                if (currentY < endY)
                {
                    nextY += random.Next(0, 2);
                }
                else if (currentY > endY)
                {
                    nextY -= random.Next(0, 2);
                }
                //else { nextY -= random.Next(-6, 7) / 6; }

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
                    int randomDirection = random.Next(0, 4); // Рандомный выбор направления округления

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
                int y = point[0];
                int x = point[1];
                if (x >= 0 && y >= 0 && x < this.matrixX && y < this.matrixY)
                {
                    if (matrix[y, x] == 0)
                    {
                        matrix[y, x] = Convert.ToInt32(Constants.Buildings.Road);
                    }

                }
            }

            depend[start].Add(end);
            depend[end].Add(start);
        }

    }


}

