using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Use this to set how many stacks of items you want to store inside of the inventory")]
    private uint _maxStackAmount = 0;
    [SerializeField]
    [Tooltip("Set to true if you want the inventory to automatically draw. Set to false if you want to implement your own UI for the inventory.")]
    private bool _useDefaultDisplay;
    [SerializeField]
    [Tooltip("Font that the inventory will use to display the UI. Only needed if Use Default Display is set to true.")]
    private Font _font;
    private List<GameObject> _items;

    [HideInInspector]
    public uint MaxStackAmount { get => _maxStackAmount; }

    [HideInInspector]
    public List<GameObject> Items { get => _items; }

    public void SetMaxStackAmount(uint stackAmount)
    {
        if(_items != null)
        {
            if (stackAmount < _items.Count) return;
        }
        _maxStackAmount = stackAmount;
    }


    public void AddItem(GameObject newItem, uint amount)
    {
        if (newItem.GetComponent<InventoryItem>() == null) return;//if it doesnt have the script then we can't add it as we won't be able to process it
        if (_items == null)//if this is the first item to be put into the inventory
        {
            AddFirstItemToInventory(newItem, amount);
            return;
        }
        // check if it is already in the inventory and if it is add to the stack
        _items.ForEach( item => {
            InventoryItem script = item.GetComponent<InventoryItem>();
            if (script.itemTag == newItem.GetComponent<InventoryItem>().itemTag)
            {
                AddItemToInventory(script, amount);
                return;
            }
        });
        AddNewItemToInventory(newItem, amount);
    }

    private GameObject AddNewStackOfItem(InventoryItem item, uint amount)
    {
        GameObject newStack = new GameObject();
        InventoryItem newScript = newStack.AddComponent<InventoryItem>();
        newScript.NumberOfItems = amount;
        newScript.isStackable = item.isStackable;
        newScript.itemTag = item.itemTag;
        newScript.maxItemsPerStack = item.maxItemsPerStack;
        return newStack;
    }

    private void AddFirstItemToInventory(GameObject newItem, uint amount)
    {
        _items = new List<GameObject>();
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        _items.Add(newItem);
    }

    private void AddNewItemToInventory(GameObject newItem, uint amount)
    {
        if (1 + _items.Count > _maxStackAmount) return;//don't add to the inventory when it's full
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        _items.Add(newItem);
    }

    private void AddItemToInventory(InventoryItem item, uint amount)
    {
        if (item.isStackable && (item.NumberOfItems + amount) <= item.maxItemsPerStack) item.NumberOfItems = item.NumberOfItems + amount;
        if (item.isStackable && (item.NumberOfItems + amount) > item.maxItemsPerStack && _items.Count < _maxStackAmount)
        {
            uint remainingItems = item.maxItemsPerStack - item.NumberOfItems;
            item.NumberOfItems = item.maxItemsPerStack;
            GameObject newStack = AddNewStackOfItem(item, remainingItems);
            _items.Add(newStack);
        }
    }
}