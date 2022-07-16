using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Test : MonoBehaviour
{
    public Texture2D texturePrefab;
    private SpriteRenderer sr;

    public float radius;
    public float minRadius;
    public float maxRadius;
    public int textureSize = 64;
    [Range(0f, 1f)]
    public float alpha;

    private float prevRadius;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GenerateTexture();
        prevRadius = radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevRadius != radius)
        {
            GenerateTexture();
            prevRadius = radius;
        }
    }

    void GenerateTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Point;
        //Renderer renderer = GetComponent<Renderer>();
        //GetComponent<Renderer>().material.mainTexture = texture;
        //texture.SetPixels(new Color[] { new Color(0, 0, 0, 0)});
        var clear = Enumerable.Repeat(new Color(0, 0, 0, 0), texture.width * texture.height).ToArray();
        texture.SetPixels(0, 0, texture.width, texture.height, clear);
        var xStart = texture.width / 2;
        var yStart = texture.height / 2;

        var t = 0f;
        var xp = Random.value * 10000;
        var yp = Random.value * 10000;
        for (float a = 0; a < Mathf.PI * 2; a += alpha)
        {
            // Approach - 1
            //var r = radius;
            //var r = Random.Range(minRadius, maxRadius);
            //var r = minRadius + Mathf.PerlinNoise(xp + t, yp + t) * (maxRadius - minRadius);
            //var x = Mathf.RoundToInt(xoff + r * Mathf.Cos(a));
            //var y = Mathf.RoundToInt(yoff + r * Mathf.Sin(a));
            //texture.SetPixel(x, y, Color.white);
            //t += 0.01f;

            // Approach - 2
            //var r = Random.Range(minRadius, maxRadius);
            //var r = 64;
            //var x_perlin = Mathf.RoundToInt(xp + r * Mathf.Cos(a));
            //var y_perlin = Mathf.RoundToInt(yp + r * Mathf.Sin(a));
            //var radius_val = Mathf.PerlinNoise(x_perlin, y_perlin) * radius;
            //var x = Mathf.RoundToInt(radius_val * Mathf.Cos(a));
            //var y = Mathf.RoundToInt(radius_val * Mathf.Sin(a));
            //texture.SetPixel(x, y, Color.white);

            // Approach - 3
            var xoff = Mathf.Cos(a) + 1;
            var yoff = Mathf.Sin(a) + 1;
            var p = Mathf.PerlinNoise(xp + xoff, yp + yoff);
            var r = Map(p, 0, 1, minRadius, maxRadius);
            var x = Mathf.RoundToInt(xStart + r * Mathf.Cos(a));
            var y = Mathf.RoundToInt(yStart + r * Mathf.Sin(a));
            texture.SetPixel(x, y, Color.white);
        }

        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32);
        sr.sprite = sprite;
    }

    float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


}
