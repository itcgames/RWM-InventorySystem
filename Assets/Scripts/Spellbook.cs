using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    InventoryItem _item;
    void Awake()
    {
        _item = GetComponent<InventoryItem>();
        _item.useFunction += PlayerGainMagic;
    }

    bool PlayerGainMagic()
    {
        Debug.Log("Player Gaining Block");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript script = player.GetComponent<PlayerScript>();
        return script.GainMagic(1.5f);
    }
}
