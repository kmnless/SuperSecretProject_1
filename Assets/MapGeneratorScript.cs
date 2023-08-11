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


    private int X;
    private int Y;
    private int baseCount;
    private int flagCount;
    private int middleFlagCount;
    private int banRadius;
    private int minRadius;
    private double terrainScale;
    private int textureSize;
    private int waterline;
    private int mountainLine;

    [SerializeField] private int RANGE_DENOMINATOR = 20;
    [SerializeField] private int CELLS_PER_PLAYER = 10;
    [SerializeField] private float DAMPING = 0.6f;
    [SerializeField] private float CONTRAST = 4.0f;
    [SerializeField] private float CLIP = 1.0f;
    [SerializeField] private int MIN_RADIUS_DENOMINATOR = 2;   // MIN RADIUS = BAN RADIUS / MIN_RADIUS_DENOMINATOR :(
    [SerializeField] private int BAN_RADIUS_MULTIPLIER = 24;
    [SerializeField] private double BAN_RADIUS_ROOT_MULTIPLIER = 0.9; // BAN RADIUS = BAN_RADIUS_MULTIPLIER * PLAYER COUNT :)
    public double[,] terrain;
    public int seed;

    private void SetTile(int height, Texture2D baseTexture, int x, int y)
    {
        if (height <= waterline)
        {
            MapTexture(baseTexture, textures.First(), x, y);
            return;
        }
        else if (height >= mountainLine)
        {
            MapTexture(baseTexture, textures.Last(), x, y);
            return;
        }
        int range = 255 - mountainLine - waterline;
        int singleInterval = range / (textures.Count() - 2);
        MapTexture(baseTexture, textures[(height - waterline) / singleInterval], x, y);
    }
    private void MapTexture(Texture2D baseTexture, Texture2D overlayTexture, int x, int y)
    {
        for(int i = 0; i < overlayTexture.Size().y; ++i) 
        {
            for(int j = 0; j < overlayTexture.Size().x; ++j) 
            {
                baseTexture.SetPixel(x+j,y+i,overlayTexture.GetPixel(j,i));
            }
        }
    }
    public void Generate()
    {
        if (textures==null){ throw new ArgumentException("Texture array is empty."); }
        textureSize = Convert.ToInt32(textures[0].Size().x);
        baseCount = Convert.ToInt32(playerCountSlider.value);
        flagCount = Convert.ToInt32(flagCountSlider.value);
        middleFlagCount = Convert.ToInt32(outpostCountSlider.value);
        X = CELLS_PER_PLAYER * baseCount;
        Y = X;
        banRadius = Convert.ToInt32(BAN_RADIUS_MULTIPLIER*Math.Sqrt(Math.Log(BAN_RADIUS_ROOT_MULTIPLIER*Math.Sqrt(baseCount))));
        terrainScale = 1.0/terrainScaleSlider.value;
        //Debug.Log(terrainScale);
        
        minRadius = banRadius/MIN_RADIUS_DENOMINATOR;
        terrain = Combiner.generatePlayField(X, Y, seed, baseCount, flagCount, middleFlagCount, minRadius, banRadius, terrainScale, Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP);


        Texture2D newTexture = new Texture2D(X*textureSize, Y*textureSize);
        Debug.Log(X * textureSize);
        for (int y = 0; y < Y*textureSize; y+= textureSize)
        {
            for (int x = 0; x < X*textureSize; x+=textureSize)
            {

                int alpha = Convert.ToInt32(255 * ((double)(terrain[y / textureSize, x / textureSize] + 1.0) / 2.0));
                //Debug.Log($"x={x},y={y}, alpha = {alpha}");
                SetTile(alpha, newTexture, x, y);
            }
        }

        newTexture.Apply();
        rawImage.texture= newTexture;

    }

}
