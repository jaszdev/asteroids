using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PerlinTest : MonoBehaviour
{
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        var width = 64;
        var height = 65;
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        var xp = Random.value * 10000;
        var yp = Random.value * 10000;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var perlin = Mathf.PerlinNoise(xp + x, yp + y);
                var color = new Color(perlin, perlin, perlin);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), width);
        sr.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
