using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProceduralTerrain : MonoBehaviour {

    public Camera cam;

    public Vector2 raiseBounds = new Vector2(0.005f, 0.0025f);
    public float raiseUpdate;
    public float radiusUpdate;
    public int maxCircles;
    public float slopeHeight;

    private Terrain terrain;
    private float[,] heights;
    private int width;
    private int height;

    public float sigma;
    public float margin;

    //private Vector2 kernelSize;
    //private float[,] kernel = null;

    private Circle[] circles;

    public bool gameStarted = false;

	void Start () {
        terrain = GetComponent<Terrain>();

        width = terrain.terrainData.heightmapWidth;
        height = terrain.terrainData.heightmapHeight;
        heights = new float[width, height];

        circles = new Circle[maxCircles];

        // Not used
        //kernelSize = new Vector2(5, 5);
        //kernel = new float[(int)kernelSize.x, (int)kernelSize.y];
        //for (int x = 0; x < (int)kernelSize.x; x++)
        //{
        //    for (int y = 0; y < (int)kernelSize.y; y++)
        //    {
        //        kernel[x, y] = gaussian(x, y);
        //    }
        //}
    }
	
	void Update () {

	    if (Input.GetMouseButtonDown(0) && gameStarted)
        {
            int freeCircle = -1;
            for (int i = 0; i < maxCircles; i++)
            {
                if (circles[i] == null)
                {
                    freeCircle = i;
                    break;
                }
            }

            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && freeCircle != -1)
            {

                int x = (int)((hit.point.x - terrain.transform.position.x) / terrain.terrainData.size.x * width);
                int z = (int)((hit.point.z - terrain.transform.position.z) / terrain.terrainData.size.z * height);
                //int bias = 25 * (height - z)/height;
                //if (x >= height/2)
                //{
                //    x = height - bias;
                //}
                //else
                //{
                //    x = bias;
                //}
                //Debug.Log(string.Format("z: {0}    x: {1}", z, x));
                Circle tempCircle = new Circle(z, x, raiseBounds.x, raiseUpdate, radiusUpdate, sigma, margin);
                circles[freeCircle] = tempCircle;
            }
        }

        // Redraw to empty
        for (int i = 0; i < width; i++)
        {
            float x = Mathf.Max(0, Mathf.Pow((width / 2f - i) / (width / 3f), 9) * -slopeHeight);
            for (int j = 0; j < height; j++)
            {
                float y = x * Mathf.Max(0, Mathf.Pow((height / 2f - j) / (height / 3f), 2));

                heights[i, j] = x + y;
            }
        }

        // Draw circles
        for (int c = 0; c < maxCircles; c++)
        {
            if (circles[c] != null && circles[c].Update(raiseBounds.y, Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2))))
            {
                heights = circles[c].drawCircle(heights);
            }
            else
            {
                circles[c] = null;
            }
        }

        // Update the terrain
        terrain.terrainData.SetHeightsDelayLOD(0, 0, heights);
    }

    // Classic convolution
    // Unfortunately, turns out to be too slow as expected.
    //private void convolution()
    //{
    //    float[,] newHeights = new float[width, height];

    //    for (int i = 0; i < width; i++)
    //    {
    //        for (int j = 0; j < height; j++)
    //        {

    //            for (int x = 0; x < (int)kernelSize.x; x++)
    //            {
    //                for (int y = 0; y < (int)kernelSize.y; y++)
    //                {
    //                    int hx = (int)Mathf.PingPong((i - x - (int)(kernelSize.x / 2)), width);
    //                    int hy = (int)Mathf.PingPong((j - y - (int)(kernelSize.y / 2)), height);
    //                    newHeights[i, j] += heights[hx, hy] * kernel[x, y];
    //                }
    //            }
    //        }
    //    }

    //    heights = newHeights;
    //}

    private float gaussian(float x, float y)
    {
        return (1 / (2 * Mathf.PI * sigma)) * Mathf.Exp((x * x + y * y) / (-2 * Mathf.Pow(sigma, 2)));
    }
}
