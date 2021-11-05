﻿using System.Collections;
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
        if(newItem.GetComponent<InventoryItem>().IsStackable)
        {
            // check if it is already in the inventory and if it is add to the stack
            foreach (GameObject item in _items)
            {
                InventoryItem script = item.GetComponent<InventoryItem>();
                if (script.Name == newItem.GetComponent<InventoryItem>().Name)
                {
                    AddItemToInventory(FindLastAddedStackOfItem(script), amount);
                    return;
                }
            }
        }
        AddNewItemToInventory(newItem, amount);
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

    private GameObject FindLastAddedStackOfItem(InventoryItem item)
    {
        return _items.FindLast(x => x.name == item.name);
    }

    private void AddItemToInventory(GameObject item, uint amount)
    {
        InventoryItem script = item.GetComponent<InventoryItem>();
        if ((script.NumberOfItems + amount) <= script.MaxItemsPerStack)
        {
            script.NumberOfItems = script.NumberOfItems + amount;
            _items[_items.Count - 1].GetComponent<InventoryItem>().NumberOfItems = script.NumberOfItems;
            return;
        }
        else if ((script.NumberOfItems + amount) > script.MaxItemsPerStack && _items.Count < _maxStackAmount)
        {
            uint remainingItems = amount - script.NumberOfItems;
            script.NumberOfItems = script.MaxItemsPerStack;
            remainingItems++;  
            _items[_items.Count - 1].GetComponent<InventoryItem>().NumberOfItems = script.NumberOfItems;
            AddNewItemToInventory(item, remainingItems);
            return;
        }
    }
}