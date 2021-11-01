using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [Tooltip("This is the name of the item so that the inventory is able to check if it is already in the inventory when adding new items.")]
    public string itemTag = "default";
    [Tooltip("This is how many of the item is currently in the inventory")]
    private uint numberOfItems = 0;
    [Tooltip("This is how many of the item you can store per stack")]
    public uint maxItemsPerStack = 1;
    public bool isStackable = true;
    public uint NumberOfItems { get => numberOfItems; set => numberOfItems = value; }
}
