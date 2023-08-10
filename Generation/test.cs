using Generation;
using PerlineNoise;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

const int X = 40;
const int Y = 80;
const int BASE_COUNT = 8;
const int FLAG_COUNT = 3;
const int RANGE_DENOMINATOR = 20;
const int MIDDLE_FLAG_COUNT = 2;

const float DAMPING = 0.6f;
const float CONTRAST = 4.0f;
const float CLIP = 1.0f;

Bitmap bitmap = new(X, Y);

int seed = (int)DateTime.UtcNow.Ticks;

double[,] terrain = Combiner.generatePlayField(X, Y, seed, BASE_COUNT, FLAG_COUNT, MIDDLE_FLAG_COUNT,  Math.Max(Math.Abs(X / RANGE_DENOMINATOR), Math.Abs(Y / RANGE_DENOMINATOR)), DAMPING, CONTRAST, CLIP);

for (int i = 0; i < terrain.GetLength(0); ++i) 
{
    for (int j = 0; j < terrain.GetLength(1); ++j) 
    {
        int brightness = (int)((terrain[i, j] + 1.0) * 127.5);
        Color color = Color.FromArgb(brightness, brightness, brightness);
        bitmap.SetPixel(j,i, color);
        //Console.Write(terrain[i, j]);
    }
    //Console.WriteLine();
}
bitmap.Save("out.png");
