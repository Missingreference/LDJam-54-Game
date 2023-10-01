using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elanetic.Tools;


public class RewardScreen : MonoBehaviour
{

    Button confirmButton;
    Image rewardsBackground;
    float animationTime = 1;
    float animationTimer;
    bool confirmClick = false;
    Color32 newColor = new Color32(63, 63, 63, 200);
    CanvasGroup rewardsImage;




    // Start is called before the first frame update
    void Start()
    {

        rewardsImage = transform.Find("Image").GetComponent<CanvasGroup>();

        rewardsBackground = transform.Find("Rewards Background").GetComponent<Image>();


        confirmButton = transform.Find("Image").Find("Confirm Button").GetComponent<Button>();

        confirmButton.onClick.AddListener(OnConfirmButtonClick);

        rewardsBackground.color = newColor;

        
    }

    void OnConfirmButtonClick()
    {
        confirmClick = true;

        
        animationTime = 1f;
        animationTimer = animationTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (confirmClick == true && animationTimer >= 0f)
        {
            Debug.Log(animationTimer);
            animationTimer -= Time.deltaTime;
            float animationPercent = 1f - (animationTimer / animationTime);
            rewardsBackground.color = Easing.Linear.InOut(newColor, Color.clear, animationPercent);
            rewardsImage.alpha = Easing.Linear.InOut(1,0,animationPercent);

            if (animationTimer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
