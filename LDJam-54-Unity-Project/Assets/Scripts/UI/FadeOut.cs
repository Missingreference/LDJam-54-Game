using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elanetic.Tools;

public class FadeOut : MonoBehaviour
{

    float fadeTimer;
    float fadeTime = 2f;
    Image fadeOutBlackImage;
    
    // Start is called before the first frame update
    void Start()
    {
        //get transform of fade to clear image
        fadeOutBlackImage = transform.Find("Fade Out Black Image").GetComponent<Image>();

        //set image to black

        fadeTimer = fadeTime; 
    }

    // Update is called once per frame
    void Update()
    {
        //if time is not 0, fade
        if (fadeTimer >= 0)
        {

            fadeTimer -= Time.deltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeOutBlackImage.color = Easing.Linear.InOut(Color.black, Color.clear, fadePercent);
            
        }
        else
        {
            fadeOutBlackImage.gameObject.SetActive(false);
        }

    }
}
