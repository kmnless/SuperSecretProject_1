namespace PerlineNoise
{

    public class PerlinNoise
    {
        private readonly int[] permutation;
        private readonly int[] p;
        private readonly Random random;
        private int[] GetPermutation()
        {
            int[] output = new int[256];
            byte current;
            for (int i = 0; i < 256; ++i)
            {
                output[i] = -1;
            }
            for (int i = 0; i < 256; ++i)
            {
                current = (byte)random.Next();
                if (!isIn(current, output))
                {
                    output[i] = current;
                }
                else
                {
                    i--;
                }
            }
            return output;
        }
        private bool isIn(byte val, int[] array)
        {
            foreach (int a in array)
            {
                if (a == val)
                {
                    return true;
                }
            }
            return false;
        }
        public PerlinNoise(int seed)
        {
            random = new Random(seed);
            permutation = GetPermutation();
            p = new int[512];
            for (int i = 0; i < 256; i++)
            {
                p[i] = permutation[i];
                p[256 + i] = permutation[i];
            }
        }
        private double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        private double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }
        private double Grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
        private double Noise(double x, double y, double z)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            int Z = (int)Math.Floor(z) & 255;

            x -= Math.Floor(x);
            y -= Math.Floor(y);
            z -= Math.Floor(z);

            double u = Fade(x);
            double v = Fade(y);
            double w = Fade(z);

            int A = p[X] + Y;
            int AA = p[A] + Z;
            int AB = p[A + 1] + Z;
            int B = p[X + 1] + Y;
            int BA = p[B] + Z;
            int BB = p[B + 1] + Z;

            return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                Grad(p[BA], x - 1, y, z)),
                        Lerp(u, Grad(p[AB], x, y - 1, z),
                                Grad(p[BB], x - 1, y - 1, z))),
                Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                Grad(p[BA + 1], x - 1, y, z - 1)),
                        Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                Grad(p[BB + 1], x - 1, y - 1, z - 1))));
        }
        public double[,] GetNoiseArray(int sizeX, int sizeY, double noiseScale = 0.1)
        {
            double[,] outputArray = new double[sizeX, sizeY];
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    outputArray[x, y] = Noise(x * noiseScale, y * noiseScale, 0);
                }
            }
            return outputArray;
        }
    }
}