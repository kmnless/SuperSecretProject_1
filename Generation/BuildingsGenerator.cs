namespace Generation
{
    //try
    //{
    //    BuildingsGenerator bg = new(40, 40, 5, (int)DateTime.UtcNow.Ticks);
    //bg.test();
    //}
    //catch (ArgumentException ex)
    //{
    //Console.WriteLine(ex.Message);
    //}
    public class BuildingsGenerator
    {
        private readonly int minRadius;
        private readonly int banRadius;
        private readonly int interestPointsCount;
        private Random random;
        private int matrixSizeX;
        private int matrixSizeY;
        private int amountOfBases;
        private const int LOW_PRIORITY_FLAG_INDEX = 3;
        private const int HIGH_PRIORITY_FLAG_INDEX = 7;
        private const int BASE_INDEX = 1;
        private const int MAX_DEPTH = 5;

        public int[,] matrix { get; }
        public HashSet<Tuple<int, int>> placedBases { get; }
        public HashSet<Tuple<int, int>> interestPoints { get; }

        public HashSet<Tuple<Tuple<int, int>, int>> placedFlags;

        public Tuple<int, int> middle { get; }
        public BuildingsGenerator(int sizeX, int sizeY, int amountOfBases, int seed, int interestPointsCount = 1)
        {
            matrixSizeX = sizeX;
            matrixSizeY = sizeY;
            this.amountOfBases = amountOfBases;
            middle = new(matrixSizeY / 2, matrixSizeX / 2);
            matrix = new int[matrixSizeY, matrixSizeX];
            minRadius = 3;
            banRadius = 10;
            random = new Random(seed);
            placedBases = new();
            placedFlags = new();
            interestPoints = new();
            this.interestPointsCount = interestPointsCount;
        }

        //public void test()
        //{
        //    placeBases();
        //    placeNecessaryFlags();

        //    foreach (Tuple<int, int> a in placedBases)
        //    {
        //        matrix[a.Item1, a.Item2] = 1;
        //    }
        //    foreach (Tuple<int, int> a in placedFlags)
        //    {
        //        matrix[a.Item1, a.Item2] = 7;
        //    }

        //    for (int x = 0; x < matrixSizeX; x++)
        //    {
        //        for (int y = 0; y < matrixSizeY; y++)
        //        {
        //            Console.Write(matrix[x, y]);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        public void generateMatrix()
        {
            foreach (Tuple<int, int> a in placedBases)
            {
                matrix[a.Item1, a.Item2] = BASE_INDEX;
            }
            foreach (Tuple<Tuple<int, int>, int> a in placedFlags)
            {
                matrix[a.Item1.Item1, a.Item1.Item2] = a.Item2;
            }
        }

        private bool placeBase(int depth = 0)
        {
            if (depth >= matrixSizeX * matrixSizeY)
                return false;
            int posX, posY;
            posX = random.Next(0, matrixSizeX);
            posY = random.Next(0, matrixSizeY);
            Tuple<int, int> pos = new Tuple<int, int>(posY, posX);
            if (baseCanBePlaced(pos))
            {
                placedBases.Add(pos);
                return true;
            }
            else
            {
                depth++;
                if (placeBase(depth))
                    return true;
                return false;
            }
        }
        public void placeBases(int depth = 0)
        {
            if (depth > MAX_DEPTH)
            {
                throw new ArgumentException("Wrong base count");
            }
            for (int i = 0; i < amountOfBases; ++i)
                if (!placeBase())
                {
                    placedBases.Clear();
                    placedFlags.Clear();
                    depth++;
                    placeBases(depth);
                }
        }
        public void placeNecessaryFlags()
        {
            foreach (Tuple<int, int> bannedPos in placedBases)
            {
                placeFlagInBanRadius(bannedPos);
            }
        }
        public void placeAdditionlFlagsNearPoint(int amount, Tuple<int, int> place)
        {
            for (int i = 0; i < amount; ++i)
            {
                placeFlagInBanRadius(place, HIGH_PRIORITY_FLAG_INDEX);
            }
        }
        private bool placeFlagInBanRadius(Tuple<int, int> place, int flagIndex = LOW_PRIORITY_FLAG_INDEX, int depth = 0)
        {
            if (depth > MAX_DEPTH)
            {
                return false;
            }
            int posY = random.Next(place.Item1 - banRadius, place.Item1 + banRadius);
            int posX = random.Next(place.Item2 - banRadius, place.Item2 + banRadius);

            Tuple<int, int> expectedFlagPos = getNormalizedPos(posY, posX);
            if (calculateDistance(place, expectedFlagPos) < banRadius && calculateDistance(place, expectedFlagPos) > minRadius &&
                !placedFlags.Contains(new(expectedFlagPos, LOW_PRIORITY_FLAG_INDEX)) && !placedFlags.Contains(new(expectedFlagPos, HIGH_PRIORITY_FLAG_INDEX)) &&
                !placedBases.Contains(expectedFlagPos))
            {
                placedFlags.Add(new(expectedFlagPos, flagIndex));
                return true;
            }
            else
            {
                depth++;

                if (!placeFlagInBanRadius(place, flagIndex, depth))
                {
                    throw new Exception($"Cant place new flag in area {place.Item1},{place.Item2}");
                }
                return true;
            }

        }
        public void generateInterestPoints(Tuple<int, int> place)
        {
            for (int i = 0; i < interestPointsCount; i++)
            {
                placeInterestPointInBanRadius(place);
            }
        }
        private bool placeInterestPointInBanRadius(Tuple<int, int> place, int depth = 0)
        {
            if (depth > MAX_DEPTH)
            {
                return false;
            }
            int posY = random.Next(place.Item1 - banRadius, place.Item1 + banRadius);
            int posX = random.Next(place.Item2 - banRadius, place.Item2 + banRadius);

            Tuple<int, int> expectedInterestPos = getNormalizedPos(posY, posX);
            if (calculateDistance(place, expectedInterestPos) < banRadius && calculateDistance(place, expectedInterestPos) > minRadius)
            {
                interestPoints.Add(expectedInterestPos);
                return true;
            }
            else
            {
                depth++;
                if (!placeInterestPointInBanRadius(place, depth))
                    throw new Exception("Couldn't place interest point");
                return true;
            }

        }
        public HashSet<Tuple<int, int>> getUnweightedFlags()
        {
            HashSet<Tuple<int, int>> output = new HashSet<Tuple<int, int>>();
            foreach (Tuple<Tuple<int, int>, int> flagPos in placedFlags)
            {
                output.Add(flagPos.Item1);
            }
            return output;

        }
        private Tuple<int, int> getNormalizedPos(int posY, int posX)
        {
            if (posX < 0)
                posX = 0;
            if (posY < 0)
                posY = 0;
            if (posX >= matrixSizeX)
                posX = matrixSizeX - 1;
            if (posY >= matrixSizeY)
                posY = matrixSizeY - 1;
            return new Tuple<int, int>(posY, posX);
        }
        private bool baseCanBePlaced(Tuple<int, int> placePos)
        {
            if (calculateDistance(placePos, middle) < banRadius * 2)
                return false;

            foreach (Tuple<int, int> bannedPos in placedBases)
            {
                if (calculateDistance(placePos, bannedPos) < banRadius * 2)
                    return false;
            }
            return true;
        }
        private int calculateDistance(Tuple<int, int> pos1, Tuple<int, int> pos2)
        {
            return Convert.ToInt32(Math.Sqrt((pos2.Item1 - pos1.Item1) * (pos2.Item1 - pos1.Item1) + (pos2.Item2 - pos1.Item2) * (pos2.Item2 - pos1.Item2)));
        }
    }
}
