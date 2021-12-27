using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    InventoryItem _item;
    void Awake()
    {
        _item = GetComponent<InventoryItem>();
        _item.useFunction += PlayerGainBlock;
    }

    bool PlayerGainBlock()
    {
        Debug.Log("Player Gaining Block");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript script = player.GetComponent<PlayerScript>();
        return script.GainBlock(1.5f);
    }
}
