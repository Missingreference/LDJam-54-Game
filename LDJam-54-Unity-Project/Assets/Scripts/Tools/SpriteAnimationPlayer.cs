using Elanetic.Tools;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SpriteAnimationPlayer : MonoBehaviour
{
    public float playbackSpeed = 1.0f;
    public Sprite[] sprites;
    public Easing.EasingFunction easingAnimation =  Easing.EasingFunction.Linear;

    public Easing.EasingFunction easingMovement = Easing.EasingFunction.Linear;
    public Vector3 endPosition = Vector3.zero;

    public float playbackTime = 0.0f;

    private SpriteRenderer m_SpriteRenderer;

    private void Start()
    {
        GameObject childObject = new GameObject("Sprite Renderer");
        childObject.transform.parent = this.transform;
        childObject.transform.localPosition = Vector3.zero;
        childObject.transform.localEulerAngles = Vector3.zero;
        childObject.transform.localScale = Vector3.one;

        m_SpriteRenderer = childObject.AddComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(sprites == null || sprites.Length == 0) return;

        playbackTime += Time.deltaTime * playbackSpeed;
        
        if(playbackTime < 0.0f)
        {
            playbackTime = Mathf.Abs(1.0f + playbackTime);
        }
        if(playbackTime > 1.0f)
        {
            playbackTime %= 1.0f;
        }

        float percent = Easing.Ease(0.0f, 1.0f, playbackTime, easingAnimation);

        int frame = (int)(sprites.Length * percent);

        m_SpriteRenderer.sprite = sprites[frame];

        m_SpriteRenderer.transform.localPosition = Easing.Ease(Vector3.zero, endPosition, playbackTime, easingMovement);
    }

}
