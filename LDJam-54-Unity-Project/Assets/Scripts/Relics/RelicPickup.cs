using Elanetic.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicPickup : MonoBehaviour
{

    static private Sprite m_CommonSprite;
    static private Sprite m_UncommonSprite;
    static private Sprite m_RareSprite;
    static private Sprite m_LegendarySprite;

    public Relic relic { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public SpriteRenderer spriteLightRenderer { get; private set; }

    public BoxCollider2D trigger { get; private set; }


    private float m_PickupDelay = 0.85f;

    private void Awake()
    {
        if(m_CommonSprite == null)
        {
            m_CommonSprite = Resources.Load<Sprite>("Sprites/Items/Relics/Common_Relic");
            m_UncommonSprite = Resources.Load<Sprite>("Sprites/Items/Relics/Uncommon_Relic");
            m_RareSprite = Resources.Load<Sprite>("Sprites/Items/Relics/Rare_Relic");
            m_LegendarySprite = Resources.Load<Sprite>("Sprites/Items/Relics/Legendary_Relic");
        }

        GameObject spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = transform;
        spriteObject.transform.localPosition = Vector3.zero;
        spriteObject.transform.localEulerAngles = Vector3.zero;
        spriteObject.transform.localScale = Vector3.one;
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

        spriteObject = new GameObject("Light");
        spriteObject.transform.parent = transform;
        spriteObject.transform.localPosition = Vector3.zero;
        spriteObject.transform.localEulerAngles = Vector3.zero;
        spriteObject.transform.localScale = Vector3.one;
        spriteLightRenderer = spriteObject.AddComponent<SpriteRenderer>();
        Texture2D square = new Texture2D(2, 2);

        Sprite lightSprite = Sprite.Create(square, new Rect(0, 0, square.width, square.height), new Vector2(0.5f, 0.0f), 36.0f);
        spriteLightRenderer.sprite = lightSprite;
        spriteLightRenderer.transform.localScale = new Vector3(1, 100, 1);

        trigger = gameObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.enabled = false;

    }

    float animateTime = 1.0f;
    float animateTimer;

    float minLightWidth = 4f;
    float maxLightWidth = 8f;

    private void Update()
    {
        if(m_PickupDelay > 0)
        {
            m_PickupDelay -= Time.deltaTime;
        }
        else
        {
            trigger.enabled = true;
        }

        //Animate light
        animateTimer += Time.deltaTime;
        if(animateTimer >= animateTime) 
        {
            animateTimer %= animateTime;
        }

        if(animateTimer < (animateTime * 0.5f))
        {
            float percent = (animateTimer / animateTime) * 2.0f;
            spriteLightRenderer.transform.localScale = Easing.Ease(new Vector3(minLightWidth, 10000.0f, 1), new Vector3(maxLightWidth, 10000.0f, 1.0f), percent, Easing.EasingFunction.SinusoidalInOut);
        }
        else
        {
            float percent = ((animateTimer / animateTime) - 0.5f) * 2.0f;
            spriteLightRenderer.transform.localScale = Easing.Ease(new Vector3(maxLightWidth, 10000.0f, 1), new Vector3(minLightWidth, 10000.0f, 1.0f), percent, Easing.EasingFunction.SinusoidalInOut);
        }

    }

    public void SetRelic(Relic relic)
    {
        this.relic = relic;
        switch(relic.rarity)
        {
            case RelicRarity.Common:
                spriteRenderer.sprite = m_CommonSprite;
                spriteLightRenderer.color = new Color(Relic.commonColor.r, Relic.commonColor.g, Relic.commonColor.b, 0.5f);
                break;
            case RelicRarity.Uncommon:
                spriteRenderer.sprite = m_UncommonSprite;
                spriteLightRenderer.color = new Color(Relic.uncommonColor.r, Relic.uncommonColor.g, Relic.uncommonColor.b, 0.5f);
                break;
            case RelicRarity.Rare:
                spriteRenderer.sprite = m_RareSprite;
                spriteLightRenderer.color = new Color(Relic.rareColor.r, Relic.rareColor.g, Relic.rareColor.b, 0.5f);
                break;
            case RelicRarity.Legendary:
                spriteRenderer.sprite = m_LegendarySprite;
                spriteLightRenderer.color = new Color(Relic.legendaryColor.r, Relic.legendaryColor.g, Relic.legendaryColor.b, 0.5f);
                break;
            default:
                break;
        }
    }
}
