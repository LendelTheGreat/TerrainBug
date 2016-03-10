using UnityEngine;
using System.Collections;

public class Circle {

    public int x;
    public int y;
    public float r;

    private float raise;
    private float radiusUpdate = 0.1f;
    private float raiseUpdate = -0.00001f;
    private float sigma;
    private float margin;

    private float[] gaussian;
    private int gaussianSizeIncrease;

    public Circle(int x, int y, float initRaise, float raiseUpdate, float radiusUpdate, float sigma, float margin)
    {
        this.x = x;
        this.y = y;

        this.raiseUpdate = raiseUpdate;
        this.radiusUpdate = radiusUpdate;

        this.sigma = sigma;
        this.margin = margin;

        raise = initRaise;
        r = 2;

        gaussianSizeIncrease = 10;
        gaussian = new float[gaussianSizeIncrease * (int)margin+1];

        for (int i = 0; i < gaussian.Length; i++)
        {
            gaussian[i] = gaussian1D(i/(float)gaussianSizeIncrease);
        }
    }
	
	public bool Update (float minRaise, float maxRadius) {
        r += radiusUpdate;

        raise += raiseUpdate;
        if (raise < minRaise)
        {
            return false;
        }

        if (r > maxRadius)
        {
            return false;
        }


        return true;
	}

    public float[,] drawCircle(float[,] heights)
    {
        int rad = Mathf.RoundToInt(r + margin);
        int fromX = Mathf.Max(x - rad, 0);
        int fromY = Mathf.Max(y - rad, 0);

        int toX = Mathf.Min(fromX + 2 * rad, heights.GetLength(0));
        int toY = Mathf.Min(fromY + 2 * rad, heights.GetLength(1));

        for (int i = fromX; i < toX; i++)
        {
            float xDist = Mathf.Pow(i - x, 2);
            for (int j = fromY; j < toY; j++)
            {
                float distFromR = Mathf.Abs(Mathf.Sqrt(xDist + Mathf.Pow(j - y, 2)) - r);
                

                if ((distFromR < margin))
                {
                    heights[i, j] += raise * gaussian[Mathf.RoundToInt(gaussianSizeIncrease * distFromR)];
                }
            }
        }

        return heights;
    }

    private float gaussian1D(float dist)
    {
        return (1 / (2 * Mathf.PI * sigma)) * Mathf.Exp((dist * dist) / (-2 * Mathf.Pow(sigma, 2)));
    }
}
