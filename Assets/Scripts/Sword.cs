using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    InventoryItem _item;
    // Start is called before the first frame update
    void Awake()
    {
        _item = GetComponent<InventoryItem>();
        _item.useFunction += GainStrength;
    }

    bool GainStrength()
    {
        Debug.Log("Player Gaining Strength");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript script = player.GetComponent<PlayerScript>();
        return script.GainStrength(1.5f);
    }
}
