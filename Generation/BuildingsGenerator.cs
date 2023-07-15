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
        private Random random;
        private int matrixSizeX;
        private int matrixSizeY;
        private int amountOfBases;
        private int[,] matrix;
        private HashSet<Tuple<int, int>> placedBases { get; }
        private HashSet<Tuple<int, int>> placedFlags { get; }
        private Tuple<int, int> middle;
        public BuildingsGenerator(int sizeX, int sizeY, int amountOfBases, int seed)
        {
            matrixSizeX = sizeX;
            matrixSizeY = sizeY;
            this.amountOfBases = amountOfBases;
            middle = new(matrixSizeX / 2, matrixSizeY / 2);
            matrix = new int[matrixSizeX, matrixSizeY];
            minRadius = 3;
            banRadius = 10;
            random = new Random(seed);
            placedBases = new();
            placedFlags = new();
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

        private bool placeBase(int depth = 0)
        {
            if (depth >= matrixSizeX * matrixSizeY)
                return false;
            int posX, posY;
            posX = random.Next(0, matrixSizeX);
            posY = random.Next(0, matrixSizeY);
            Tuple<int, int> pos = new Tuple<int, int>(posX, posY);
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
            if (depth > 5)
                throw new ArgumentException("Wrong base count");
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
        private void placeFlagInBanRadius(Tuple<int, int> basePos)
        {

            int posX = random.Next(basePos.Item1 - banRadius, basePos.Item1 + banRadius);
            int posY = random.Next(basePos.Item2 - banRadius, basePos.Item2 + banRadius);
            Tuple<int, int> expectedFlagPos = getNormalizedPos(posX, posY);
            if (calculateDistance(basePos, expectedFlagPos) < banRadius && calculateDistance(basePos, expectedFlagPos) > minRadius)
            {
                placedFlags.Add(expectedFlagPos);
            }
            else
            {
                placeFlagInBanRadius(basePos);
            }
        }
        private Tuple<int, int> getNormalizedPos(int posX, int posY)
        {
            if (posX < 0)
                posX = 0;
            if (posY < 0)
                posY = 0;
            if (posX >= matrixSizeX)
                posX = matrixSizeX - 1;
            if (posY >= matrixSizeY)
                posY = matrixSizeY - 1;
            return new Tuple<int, int>(posX, posY);
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
