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
    private List<GameObject> _usedItems;
    private uint _activeItemIndex;
    private bool _isOpen;
    private const string _notSetString = "not set";
    private string _openCommand = _notSetString;
    private string _closeCommand = _notSetString;
    private string _submitCommand = _notSetString;

    [HideInInspector]
    public uint MaxStackAmount { get => _maxStackAmount; }

    [HideInInspector]
    public List<GameObject> Items { get => _items; }

    [HideInInspector]
    public List<GameObject> UsedItems { get => _usedItems;}

    [HideInInspector]
    public GameObject ActiveItem { get {
            if (_items == null || _activeItemIndex > _items.Count) return null;
            return _items[(int)_activeItemIndex];
    }}

    public bool IsOpen { get => _isOpen; set => _isOpen = value; }

    [HideInInspector]
    public string OpenCommand { get => _openCommand; set => _openCommand = value; }

    [HideInInspector]
    public string CloseCommand { get => _closeCommand; set => _closeCommand = value; }

    [HideInInspector]
    public string SubmitCommand { get => _submitCommand; set => _submitCommand = value; }

    public void SetMaxStackAmount(uint stackAmount)
    {
        if(_items != null)
        {
            if (stackAmount < _items.Count) return;
        }
        _maxStackAmount = stackAmount;
    }

    private void Start()
    {
        _usedItems = new List<GameObject>();
    }


    public void AddItem(GameObject newItem, uint amount)
    {
        if (newItem.GetComponent<InventoryItem>() == null) return;//if it doesnt have the script then we can't add it as we won't be able to process it
        if (_items == null)//if this is the first item to be put into the inventory
        {
            AddFirstItemToInventory(newItem, amount);
            return;
        }
        if (newItem.GetComponent<InventoryItem>().IsStackable)
        {
            // check if it is already in the inventory and if it is add to the stack or create new stack if last stack is full
            GameObject lastItem = FindLastAddedStackOfItem(newItem.GetComponent<InventoryItem>());
            if (lastItem != null)
            {
                AddItemToInventory(lastItem, newItem, amount);
                return;
            }
        }
        AddNewItemToInventory(newItem, amount);
    }

    public void UseItem(string submitCommand)
    {
        if(Input.GetButtonDown(submitCommand) && _isOpen)
        {
            if (_items[(int)_activeItemIndex] == null) return;
            InventoryItem item = _items[(int)_activeItemIndex].GetComponent<InventoryItem>();

            if (item.useFunction != null)
            {
                item.useFunction();
            }
            else
            {
                if(_usedItems == null) _usedItems = new List<GameObject>();
                _usedItems.Add(_items[(int)_activeItemIndex]);
            }
            item.NumberOfItems--;
            if(item.NumberOfItems <= 0)
            {
                _items.Remove(_items[(int)_activeItemIndex]);
                if (_items.Count < _activeItemIndex && _items.Count > 0) _activeItemIndex--;
            }
        }
    }

    public void UseItem()
    {
        if (_isOpen)
        {
            if (_items[(int)_activeItemIndex] == null) return;
            InventoryItem item = _items[(int)_activeItemIndex].GetComponent<InventoryItem>();

            if(item.useFunction != null)
            {
                Debug.Log("has function");
                item.useFunction();
            }
            else
            {
                if (_usedItems == null) _usedItems = new List<GameObject>();
                _usedItems.Add(_items[(int)_activeItemIndex]);
            }
            item.NumberOfItems--;
            if (item.NumberOfItems <= 0)
            {
                _items.Remove(_items[(int)_activeItemIndex]);
                if (_items.Count - 1 < _activeItemIndex && _items.Count > 0) _activeItemIndex--;
                Debug.Log("Removing used item from inventory");
            }
        }
    }

    public void OpenInventory(string openCommand)
    {
        if(Input.GetButtonDown(openCommand))
        {
            _isOpen = true;
        }
    }

    public void OpenInventory()
    {
        _isOpen = true;
        Debug.Log("Inventory Opened");
    }

    public void CloseInventory(string closeCommand)
    {
        if(Input.GetButtonDown(closeCommand))
        {
            _isOpen = false;
        }
    }

    public void CloseInventory()
    {
        _isOpen = false;
        Debug.Log("Inventory Opened");
    }

    

    void SetSubmitCommand(string submitCommand)
    {
        _submitCommand = submitCommand;
    }

    void SetOpenCommand(string openCommand)
    {
        _openCommand = openCommand;
    }

    void SetCloseCommand(string closeCommand)
    {
        _closeCommand = closeCommand;
    }

    private void AddFirstItemToInventory(GameObject newItem, uint amount)
    {
        _items = new List<GameObject>();
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        _items.Add(newItem);
        _activeItemIndex = 0;
    }

    private void AddNewItemToInventory(GameObject newItem, uint amount)
    {
        if (1 + _items.Count > _maxStackAmount) return;//don't add to the inventory when it's full
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        _items.Add(newItem);
        _activeItemIndex = (uint)_items.Count - 1;
    }

    private GameObject FindLastAddedStackOfItem(InventoryItem item)
    {
        return _items.FindLast(x => x.GetComponent<InventoryItem>().Name == item.Name);
    }

    private void AddItemToInventory(GameObject item, GameObject newItem, uint amount)
    {
        InventoryItem script = item.GetComponent<InventoryItem>();
        if ((script.NumberOfItems + amount) <= script.MaxItemsPerStack)
        {
            script.NumberOfItems = script.NumberOfItems + amount;
            return;
        }
        else if ((script.NumberOfItems + amount) > script.MaxItemsPerStack && _items.Count < _maxStackAmount)
        {
            uint remainingItems = amount - (script.MaxItemsPerStack - script.NumberOfItems);
            script.NumberOfItems = script.MaxItemsPerStack;
            AddNewItemToInventory(newItem, remainingItems);
            return;
        }
    }
}