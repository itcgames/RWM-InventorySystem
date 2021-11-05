using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] itemsToAdd;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        inventory.AddItem(itemsToAdd[0], 5);
        inventory.AddItem(itemsToAdd[0], 1);
        inventory.AddItem(itemsToAdd[0], 7);
        inventory.Items.ForEach(item => Debug.Log(item.GetComponent<InventoryItem>().NumberOfItems));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
