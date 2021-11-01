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
        foreach(GameObject item in itemsToAdd)
        {
            inventory.AddItem(item, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
