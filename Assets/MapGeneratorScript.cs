using System;
using UnityEngine;
using Generation;
using UnityEngine.UI;



public class MapGeneratorScript : MonoBehaviour
{
    public Slider playerCountSlider;
    public Slider flagCountSlider;
    public Slider outpostCountSlider;
    public RawImage rawImage;



    private int X;
    private int Y;
    private int BASE_COUNT;
    private int FLAG_COUNT;
    private int MIDDLE_FLAG_COUNTER;
    [SerializeField] private int RANGE_DENOMINATOR = 20;
    [SerializeField] private int CELLS_PER_PLAYER = 10;
    [SerializeField] private float DAMPING = 0.6f;
    [SerializeField] private float CONTRAST = 4.0f;
    [SerializeField] private float CLIP = 1.0f;
    public double[,] terrain;
    public int seed;    
 
    public void Generate()
    {

        BASE_COUNT = Convert.ToInt32(playerCountSlider.value);
        FLAG_COUNT = Convert.ToInt32(flagCountSlider.value);
        MIDDLE_FLAG_COUNTER = Convert.ToInt32(outpostCountSlider.value);
        X = CELLS_PER_PLAYER * BASE_COUNT;
        Y = X;
        terrain = Combiner.generatePlayField(X, Y, seed, BASE_COUNT, FLAG_COUNT, MIDDLE_FLAG_COUNTER, Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP);


        Texture2D newTexture = new Texture2D(X, Y);

        for (int y = 0; y < Y; y++)
        {
            for (int x = 0; x < X; x++)
            {

                float alpha = (float)(terrain[y, x] + 1.0f)/2.0f;
                Color color = new Color(1.0f, 1.0f, 1.0f, alpha);
                newTexture.SetPixel(x, y, color);
            }
        }

        newTexture.Apply();
        rawImage.texture= newTexture;

    }

}
