using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public float minRadius;
    public float maxRadius;
    public float minRotation;
    public float maxRotation;
    public int textureSize = 64;
    public int pixelsPerUnit = 8;
    [Range(0f, 1f)]
    public float alpha = 0.01f;

    private float rotation;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        GenerateTexture();
        var rotationDir = Random.value < 0.5 ? -1f : 1f;
        var rotationMag = Random.Range(minRotation, maxRotation);
        rotation = rotationDir * rotationMag;
        rotation = 0;
        AddColliders();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, rotation));
    }

    void GenerateTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Point;
        var clear = Enumerable.Repeat(new Color(0, 0, 0, 0), texture.width * texture.height).ToArray();
        texture.SetPixels(0, 0, texture.width, texture.height, clear);
        var xStart = texture.width / 2;
        var yStart = texture.height / 2;
        var xp = Random.value * 10000;
        var yp = Random.value * 10000;
        for (float a = 0; a < Mathf.PI * 2; a += alpha)
        {
            var xoff = Mathf.Cos(a) + 1;
            var yoff = Mathf.Sin(a) + 1;
            var p = Mathf.PerlinNoise(xp + xoff, yp + yoff);
            var r = Map(p, 0, 1, minRadius, maxRadius);
            var x = Mathf.RoundToInt(xStart + r * Mathf.Cos(a));
            var y = Mathf.RoundToInt(yStart + r * Mathf.Sin(a));
            texture.SetPixel(x, y, Color.white);
        }
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        sr.sprite = sprite;
    }

    void AddColliders()
    {
        var polCollider = gameObject.AddComponent<PolygonCollider2D>();
        gameObject.AddComponent<CompositeCollider2D>();
        polCollider.usedByComposite = true;
    }

    float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}
