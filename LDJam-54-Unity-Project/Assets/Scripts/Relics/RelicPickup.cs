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


    }

    public void SetRelic(Relic relic)
    {
        this.relic = relic;
    }
}
