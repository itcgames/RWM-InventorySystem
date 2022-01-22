using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ring : MonoBehaviour
{
    InventoryItem _item;
    void Awake()
    {
        _item = GetComponent<InventoryItem>();
        _item.useFunction += PlayerGainSorcery;
    }

    bool PlayerGainSorcery()
    {
        Debug.Log("Player Gaining Block");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript script = player.GetComponent<PlayerScript>();
        return script.GainSorcery(1.5f);
    }
}
