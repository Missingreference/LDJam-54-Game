using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    protected override void Awake()
    {
        base.Awake();

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.red);
        texture.SetPixel(1, 0, new Color32(255, 172,28, 255));
        texture.SetPixel(0, 1, new Color32(255, 172, 28, 255));
        texture.SetPixel(1, 1, Color.red);
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    protected override void OnMove()
    {
        base.OnMove();
    }

}
