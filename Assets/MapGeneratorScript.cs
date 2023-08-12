using System;
using UnityEngine;
using Generation;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class MapGeneratorScript : MonoBehaviour
{
    public Slider playerCountSlider;
    public Slider flagCountSlider;
    public Slider outpostCountSlider;
    public Slider waterlineSlider;
    public Slider mounainLineSlider;
    public Slider terrainScaleSlider;
    public RawImage rawImage;
    public Texture2D[] textures;
    public Texture2D roadTexture;
    public Texture2D flagTexture;
    public Texture2D baseTexture;
    [SerializeField] private Color highPrioritySimplifiedFlagColor;
    [SerializeField] private Color lowPrioritySimplifiedFlagColor;
    [SerializeField] private Color SimplifiedBaseColor;

    private bool generated=false;
    private Color[] textureColors;
    private int X;
    private int Y;
    private int baseCount;
    private int flagCount;
    private int middleFlagCount;
    private int banRadius;
    private int minRadius;
    private double terrainScale;
    private int waterline;
    private int mountainLine;
    private const int LOW_PRIORITY_FLAG_INDEX = Generation.BuildingsGenerator.LOW_PRIORITY_FLAG_INDEX;
    private const int HIGH_PRIORITY_FLAG_INDEX = Generation.BuildingsGenerator.HIGH_PRIORITY_FLAG_INDEX;
    private const int BASE_INDEX = Generation.BuildingsGenerator.BASE_INDEX;
    [SerializeField] private int RANGE_DENOMINATOR = 20;
    [SerializeField] private int ROAD_GENERATION_COPLEXITY_DENOMINATOR = 80;
    [SerializeField] private int CELLS_PER_PLAYER = 10;
    [SerializeField] private float DAMPING = 0.6f;
    [SerializeField] private float CONTRAST = 4.0f;
    [SerializeField] private float CLIP = 1.0f;
    [SerializeField] private int MIN_RADIUS_DENOMINATOR = 2;   // MIN RADIUS = BAN RADIUS / MIN_RADIUS_DENOMINATOR :(
    [SerializeField] private int BAN_RADIUS_MULTIPLIER = 24;
    [SerializeField] private double BAN_RADIUS_ROOT_MULTIPLIER = 0.9; // BAN RADIUS = BAN_RADIUS_MULTIPLIER * PLAYER COUNT :)
    public Tuple<double[,], int[,]> terrain;
    private int[,] roadsAndBuildings;
    public int seed;

    private void FillTerrain(int height, Texture2D baseTexture, int x, int y)
    {
        if (height <= waterline)
        {
            baseTexture.SetPixel(x, y, textureColors.First());
            return;
        }
        else if (height >= 255 - mountainLine)
        {
            baseTexture.SetPixel(x, y, textureColors.Last());
            return;
        }
        int range = 255 - mountainLine - waterline;
        int singleInterval = range / (textures.Count() - 2);
        baseTexture.SetPixel(x, y, textureColors[(height - waterline) / singleInterval + 1]);
    }

    private void FillBuildings(Texture2D texture, int[,] matrix)
    {
        for(int i = 0; i < matrix.GetLength(0); ++i) 
        {
            for(int j=0; j < matrix.GetLength(1); ++j)
            {
                if (matrix[i, j] == LOW_PRIORITY_FLAG_INDEX)
                {
                    texture.SetPixel(j,i,lowPrioritySimplifiedFlagColor);
                }
                else if (matrix[i,j]== HIGH_PRIORITY_FLAG_INDEX) 
                {
                    texture.SetPixel(j,i,highPrioritySimplifiedFlagColor);
                }
                else if (matrix[i,j] == BASE_INDEX) 
                {
                    texture.SetPixel(j, i, SimplifiedBaseColor);
                }
            }
        }

    }


    private void PlaceRoads(Texture2D texture, int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); ++i)
        {
            for (int j = 0; j < matrix.GetLength(1); ++j)
            {
                if (matrix[i, j] == LOW_PRIORITY_FLAG_INDEX)
                {
                    texture.SetPixel(j, i, lowPrioritySimplifiedFlagColor);
                }
                else if (matrix[i, j] == HIGH_PRIORITY_FLAG_INDEX)
                {
                    texture.SetPixel(j, i, highPrioritySimplifiedFlagColor);
                }
                else if (matrix[i, j] == BASE_INDEX)
                {
                    texture.SetPixel(j, i, SimplifiedBaseColor);
                }
                else if (matrix[i, j] != 0)
                {
                    texture.SetPixel(j, i, roadTexture.GetPixel(0, 0));
                }
            }
        }
    }
    public void Generate()
    {
        if (textures==null){ throw new ArgumentException("Texture array is empty."); }
        textureColors = new Color[textures.Length];
        for(int i = 0; i < textures.Length; ++i)
        {
            textureColors[i] = textures[i].GetPixel(0,0);
        }
        baseCount = Convert.ToInt32(playerCountSlider.value);
        flagCount = Convert.ToInt32(flagCountSlider.value);
        middleFlagCount = Convert.ToInt32(outpostCountSlider.value);
        X = CELLS_PER_PLAYER * baseCount;
        Y = X;
        banRadius = Convert.ToInt32(BAN_RADIUS_MULTIPLIER*Math.Sqrt(Math.Log(BAN_RADIUS_ROOT_MULTIPLIER*Math.Sqrt(baseCount))));
        terrainScale = 1.0/terrainScaleSlider.value;
        //Debug.Log(terrainScale);
        
        minRadius = banRadius/MIN_RADIUS_DENOMINATOR;
        seed = (int)(System.DateTime.Now.TimeOfDay.TotalMilliseconds);
        try
        {
            terrain = Combiner.generatePlayField(X, Y, seed, baseCount, flagCount, middleFlagCount, minRadius, banRadius, terrainScale, Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP, ROAD_GENERATION_COPLEXITY_DENOMINATOR);

        }
        catch 
        {
            seed = Convert.ToInt32(System.DateTime.Now.TimeOfDay.TotalMilliseconds);
            terrain = Combiner.generatePlayField(X, Y, seed, baseCount, flagCount, middleFlagCount, minRadius, banRadius, terrainScale, Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP, ROAD_GENERATION_COPLEXITY_DENOMINATOR);
        }
        roadsAndBuildings = terrain.Item2;
        Texture2D newTexture = new Texture2D(X, Y);
        newTexture.filterMode = FilterMode.Point;
        //  Debug.Log(X * textureSize);
        for (int y = 0; y < Y; y++)
        {
            for (int x = 0; x < X; x++)
            {

                int alpha = Convert.ToInt32(255 * ((double)(terrain.Item1[y,x] + 1.0) / 2.0));
                //Debug.Log($"x={x},y={y}, alpha = {alpha}");
                FillTerrain(alpha, newTexture, x, y);
                //newTexture.SetPixel(x, y, Color.green);
            }
        }
        PlaceRoads(newTexture, terrain.Item2);
        //FillBuildings(newTexture, terrain.Item2);
        newTexture.Apply();
        rawImage.texture= newTexture;
        generated = true;
    }
    public void GenerateNewScale()
    {
        if (generated)
        {
            terrainScale = 1.0 / terrainScaleSlider.value;
            Texture2D newTexture = new Texture2D(X, Y);
            newTexture.filterMode = FilterMode.Point;
            terrain=new(Combiner.lowerPerlinNearMatrix(seed, terrainScale,terrain.Item2, Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP),terrain.Item2);
            //  Debug.Log(X * textureSize);
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {

                    int alpha = Convert.ToInt32(255 * ((double)(terrain.Item1[y, x] + 1.0) / 2.0));
                    //Debug.Log($"x={x},y={y}, alpha = {alpha}");
                    FillTerrain(alpha, newTexture, x, y);
                    //newTexture.SetPixel(x, y, Color.green);
                }
            }
            PlaceRoads(newTexture, terrain.Item2);
            //FillBuildings(newTexture, terrain.Item2);
            newTexture.Apply();
            rawImage.texture = newTexture;
            generated = true;
        }
    }

}