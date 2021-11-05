using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [Tooltip("This is the name of the item so that the inventory is able to check if it is already in the inventory when adding new items.")]
    [SerializeField]
    private string itemTag = "default";
    [Tooltip("This is how many of the item is currently in the inventory")]
    private uint numberOfItems = 0;
    [Tooltip("This is how many of the item you can store per stack")]
    [SerializeField]
    private uint maxItemsPerStack = 1;
    [Tooltip("Set to true if you want to be able to have more than one of this item per stack." +
        " Updating number of items without setting this to true won't affect the inventory.")]
    [SerializeField]
    private bool isStackable = true;
    public uint NumberOfItems { get => numberOfItems; set {
            if (value > maxItemsPerStack) numberOfItems = maxItemsPerStack;
            numberOfItems = value;
    } }
    public string Name { get => itemTag; set => itemTag = value; }
    public uint MaxItemsPerStack { get => maxItemsPerStack; set => maxItemsPerStack = value; }
    public bool IsStackable { get => isStackable; set => isStackable = value; }
}
