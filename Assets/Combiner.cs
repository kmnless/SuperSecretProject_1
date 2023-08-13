
using System;
using PerlineNoise;


namespace Generation
{
    public class Combiner
    {

        static public Tuple<double[,], int[,]> generatePlayField(int sizeX, int sizeY, int seed, int baseCount, int flagAmount, int middleFlagAmount, int minimalDistance,  int safePlaceRadius,
            double terrainScale, int smoothRange, float smoothCoef, float contrast, float clip, int roadGenerationComplexity=80)
        {
            double[,] output;
            int additionalInterestPoints = Math.Max(sizeX, sizeY)/roadGenerationComplexity;     
            BuildingsGenerator bg = new(sizeX, sizeY, baseCount, seed, middleFlagAmount,safePlaceRadius,minimalDistance,additionalInterestPoints);
            PerlinNoise perlineNoise = new(seed);
            RoadGenerator roadGenerator;

            bg.placeBases();
            bg.placeNecessaryFlags();
            bg.placeAdditionlFlagsNearPoint(flagAmount, bg.middle);
            bg.generateInterestPoints(bg.middle);
            bg.generateMatrix();
            
            roadGenerator = new RoadGenerator(bg.placedBases, bg.getUnweightedFlags(), bg.interestPoints, sizeX, sizeY, bg.matrix, seed,additionalInterestPoints+1);
            roadGenerator.connectAllObjectives();
            //UnityEngine.Debug.Log(roadGenerator.ToString());
            output = perlineNoise.GetNoiseArray(sizeX, sizeY, terrainScale);
            lowerTerrainNearMatrix(output, roadGenerator.matrix, smoothRange, smoothCoef);
            multiplyArray(output, contrast);
            clipArray(output, -1 * clip, clip);
            return new (output,roadGenerator.matrix);
        }

        static public void lowerTerrainNear(double[,] array, int x, int y, int range = 5, float delta = 0.55f)
        {
            int ring;
            double multiplier;
            for (int i = y - range; i < y + range; ++i)
            {
                for (int j = x - range; j < x + range; ++j)
                {
                    if (i >= 0 && j >= 0 && i < array.GetLength(0) && j < array.GetLength(1))
                    {
                        ring = Math.Max(Math.Abs(i - y), Math.Abs(j - x));
                        multiplier = delta * (double)Math.Sqrt(ring);
                        if (multiplier > 1.0f)
                        {
                            multiplier = 1.0f;
                        }
                        array[i, j] *= multiplier;
                    }

                }
            }
        }

        static public void multiplyArray(double[,] array,double multiplier)
        {
            int i, j;
            for (i = 0; i < array.GetLength(0); ++i)
            {
                for (j = 0; j < array.GetLength(1); ++j)
                {
                    array[i, j] *= multiplier;
                }

            }
        }


        static public void clipArray(double[,] array,double bottomEdge,double upperEdge)
        {
            int i, j;
            for (i = 0; i < array.GetLength(0); ++i)
            {
                for (j = 0; j < array.GetLength(1); ++j)
                {
                    if(array[i, j] < bottomEdge) { array[i, j] = bottomEdge; }
                    else if (array[i, j] > upperEdge) { array[i, j]= upperEdge; }
                }

            }
        } 

        static public void lowerTerrainNearMatrix(double[,] array, int[,] matrix, int range = 5, float delta = 0.55f) 
        {
            int i, j;
            if (matrix.GetLength(0) != array.GetLength(0) || matrix.GetLength(1) != array.GetLength(1)) 
            {
                throw new Exception("Matrices dimentions doesn't match");
            }
            for (i = 0; i < matrix.GetLength(0); ++i) 
            {
                for(j=0; j < matrix.GetLength(1); ++j) 
                {
                    if (matrix[i, j]>0) { lowerTerrainNear(array, j, i, range, delta); }
                } 
                    
            }
        }
        static public double[,] lowerPerlinNearMatrix(int seed,double scale,int[,] matrix, int range = 5, float delta = 0.55f,double contrast=4.0,double clip=1.0)
        {
            int i, j;
            PerlinNoise perlineNoise = new(seed);
            int sizeX=matrix.GetLength(1);
            int sizeY=matrix.GetLength(0);
            double[,] array=perlineNoise.GetNoiseArray(sizeX,sizeY,scale);

            for (i = 0; i < sizeY; ++i)
            {
                for (j = 0; j < sizeX; ++j)
                {
                    if (matrix[i, j] > 0) { lowerTerrainNear(array, j, i, range, delta); }
                }

            }
            multiplyArray(array, contrast);
            clipArray(array, -1 * clip, clip);
            return array;

        }







    }
}
