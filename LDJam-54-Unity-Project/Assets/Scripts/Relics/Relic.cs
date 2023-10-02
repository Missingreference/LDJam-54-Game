using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Relic
{
    static public Color commonColor => new Color(0.1f, 0.1f, 0.1f, 1.0f);
    static public Color uncommonColor => new Color(0.1f, 1.0f, 0.1f, 1.0f);
    static public Color rareColor => new Color(0.1f, 0.1f, 1.0f, 1.0f);
    static public Color legendaryColor => new Color(0.3f, 0.1f, 0.5f, 1.0f);

    public abstract RelicRarity rarity { get; }
}
