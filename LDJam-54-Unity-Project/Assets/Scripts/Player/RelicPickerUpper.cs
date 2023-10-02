using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicPickerUpper : MonoBehaviour
{

    public Action<Relic> onRelicPickup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        RelicPickup relicPickup = other.GetComponent<RelicPickup>();
        if (relicPickup != null)
        {
            onRelicPickup?.Invoke(relicPickup.relic);
            Destroy(relicPickup.gameObject);
        }
    }
}
