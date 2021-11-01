using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] itemsToAdd;
    public uint[] numberOfItemsToAdd;
    private Inventory inventory;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        for (int i = 0; i < itemsToAdd.Length; i++)
        {
            inventory.AddItem(Instantiate(itemsToAdd[i]), numberOfItemsToAdd[i]);
        }
        inventory.Items.ForEach(item => Debug.Log(item.GetComponent<InventoryItem>().NumberOfItems));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
