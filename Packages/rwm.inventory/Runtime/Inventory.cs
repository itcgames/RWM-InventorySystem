using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Use this to set how many stacks of items you want to store inside of the inventory")]
    private uint _maxStackAmount = 0;
    [SerializeField]
    [Tooltip("Set to true if you want the inventory to automatically draw. Set to false if you want to implement your own UI for the inventory.")]
    private bool _useDefaultDisplay;
    [SerializeField]
    [Tooltip("Set to true if you want to display info on the current item when the inventory is open. The corresponding text variables need to be passed in so that the inventory is able" +
        "to properly display the info")]
    private bool _displayCurrentItemInfo;
    [SerializeField]
    [Tooltip("Set this text if you want the name of the current item to be displayed.")]
    private Text _currentItemName;
    [SerializeField]
    private Vector2 _currentNameOffset;
    [SerializeField]
    [Tooltip("Set this text if you want the description for the current item to be displayed.")]
    private Text _currentItemDescription;
    [SerializeField]
    private Vector2 _currentDescriptionOffset;
    [SerializeField]
    [Tooltip("Set to text if you want the current item amount to be displayed")]
    private Text _currentItemAmount;
    [SerializeField]
    private Vector2 _currentAmountOffset;
    private List<GameObject> _items;
    private List<GameObject> _usedItems;
    private bool _isOpen;
    private const string _notSetString = "not set";
    private string _openCommand = _notSetString;
    private string _closeCommand = _notSetString;
    private string _submitCommand = _notSetString;
    private int _currentlySelectedIndex = -1;
    private int _currentPageNumber = 0;
    private int _totalNumberOfPages = 0;
    private int _currentlySelectedEquippable = -1;
    private int _currentEquippablePageNumber = 0;
    private int _totalEquippableNumberOfPages = 0;
    public Vector3 initialItemPosition = new Vector3(0,0,0);
    public Vector3 initialEquippableItemPosition = new Vector3(0, 0, 0);
    public Transform initialTransform;
    public Transform initialEquippableTransform;
    public Text pagesText;
    public Text totalItemsText;
    public int rowOffset = 10;
    public int columnOffset = 10;
    public int maxItemsPerRow = 0;
    public int maxRows = 0;
    public GameObject cursor;
    [Tooltip("Folder that the images for the items are stored in so that when the inventory is loaded back in it can load the correct images.")]
    public string spriteLocations = "not set";
    public bool useDefaultLocation;
    public string jsonName;
    public string pathToJson;
    public bool forceOverwrite;
    public string pathToLoadJsonFrom;
    public string jsonToLoadFrom;
    [SerializeField]
    [Tooltip("Use this to set how many stacks of items you want to store inside of the inventory")]
    private uint _maxEquippableStackAmount = 0;
    private List<GameObject> _equippableItems;
    [HideInInspector]
    public uint MaxStackAmount { get => _maxStackAmount; }
    public uint MaxEquippableSlots { get => _maxEquippableStackAmount;}

    [HideInInspector]
    public List<GameObject> Items { get => _items; }

    [HideInInspector]
    public List<GameObject> UsedItems { get => _usedItems;}

    [HideInInspector]
    public List<GameObject> EquippedItems { get => _equippableItems;}

    public bool IsOpen { get => _isOpen; set => _isOpen = value; }

    [HideInInspector]
    public string OpenCommand { get => _openCommand; set => _openCommand = value; }

    [HideInInspector]
    public string CloseCommand { get => _closeCommand; set => _closeCommand = value; }

    [HideInInspector]
    public string SubmitCommand { get => _submitCommand; set => _submitCommand = value; }

    [HideInInspector]
    public int ActiveItemIndex { get => _currentlySelectedIndex;}
   

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
        _isOpen = false;
        _usedItems = new List<GameObject>();
        if (cursor != null)
            cursor.SetActive(false);
        _usedItems = new List<GameObject>();
        if(_currentItemName != null)
            _currentItemName.gameObject.SetActive(false);
        if(_currentItemDescription != null)
            _currentItemDescription.gameObject.SetActive(false);
        if (_currentItemAmount != null)
            _currentItemAmount.gameObject.SetActive(false);
        if (pagesText != null)
        {
            pagesText.text = "Page " + _currentPageNumber + " of " + _totalNumberOfPages;
            pagesText.gameObject.SetActive(false);
        }
        if(totalItemsText != null)
        {
            if(_items == null)
            {
                totalItemsText.text = "Total Items in Inventory: " + 0;
            }
            else
            {
                totalItemsText.text = "Total Items in Inventory: " + _items.Count;
            }
            totalItemsText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(_items != null)
        {
            foreach (GameObject obj in _items)
            {
                obj.GetComponent<InventoryItem>().SetCanvasAsParent();
            }
        }
        if(_equippableItems != null)
        {
            foreach(GameObject obj in _equippableItems)
            {
                obj.GetComponent<InventoryItem>().SetCanvasAsParent();
            }
        }
        if (_useDefaultDisplay)
        {
            if(_items != null)
            {
                _totalNumberOfPages = Mathf.FloorToInt((_items.Count - 1) / (maxItemsPerRow * maxRows)) + 1;
                if (_items.Count == 0) _totalNumberOfPages = 0;
                pagesText.text = "Page " + (_currentPageNumber + 1) + " of " + _totalNumberOfPages + " pages";
                totalItemsText.text = "Num Items: " + _items.Count;
                OnlyDisplayCurrentPage();
            }

        }
        if(_isOpen && _useDefaultDisplay)
        {
            if (cursor != null)
                cursor.SetActive(true);
            if(_items != null && _items.Count > 0)
            {
                if (_currentItemName != null)
                    _currentItemName.gameObject.SetActive(true);
                if (_currentItemDescription != null)
                    _currentItemDescription.gameObject.SetActive(true);
                if (_currentItemAmount != null)
                    _currentItemAmount.gameObject.SetActive(true);
            }
            else
            {
                if (_currentItemName != null)
                    _currentItemName.gameObject.SetActive(false);
                if (_currentItemDescription != null)
                    _currentItemDescription.gameObject.SetActive(false);
                if (_currentItemAmount != null)
                    _currentItemAmount.gameObject.SetActive(false);
            }           
        }
        DisplayEquippableItems();
    }

    public GameObject GetCurrentlySelectedObject()
    {
        if(_isOpen)
        {
            if(_items != null && _currentlySelectedIndex >= 0 && _currentlySelectedIndex < _items.Count)
            {
                return _items[_currentlySelectedIndex];
            }
        }
        return null;
    }

    public GameObject GetCurrentlySelectedObjectWhenClosed()
    {
        if (!_isOpen)
        {
            if (_items != null && _currentlySelectedIndex >= 0 && _currentlySelectedIndex < _items.Count)
            {
                return _items[_currentlySelectedIndex];
            }
        }
        return null;
    }

    public void AddItem(GameObject newItem, uint amount)
    {
        InventoryItem script = newItem.GetComponent<InventoryItem>();
        if (script == null) return;//if it doesnt have the script then we can't add it as we won't be able to process it
        if(!script.equippableItem)
        {
            if (_items == null)//if this is the first item to be put into the inventory
            {
                AddFirstItemToInventory(newItem, amount, ref _items, _maxStackAmount);
                OnlyDisplayCurrentPage();
                return;
            }
            if (newItem.GetComponent<InventoryItem>().IsStackable)
            {
                // check if it is already in the inventory and if it is add to the stack or create new stack if last stack is full
                GameObject lastItem = FindLastAddedStackOfItem(newItem.GetComponent<InventoryItem>());
                if (lastItem != null)
                {
                    AddUntilNoItemsLeft(lastItem, (int)amount, ref _items, _maxStackAmount);
                    OnlyDisplayCurrentPage();
                    return;
                }
            }
            AddNewItemToInventory(newItem, amount, ref _items, _maxStackAmount);
        }
        else
        {
            if (_equippableItems == null)//if this is the first item to be put into the inventory
            {
                AddFirstItemToInventory(newItem, amount, ref _equippableItems, _maxStackAmount);
                OnlyDisplayCurrentPage();
                return;
            }
            if (newItem.GetComponent<InventoryItem>().IsStackable)
            {
                // check if it is already in the inventory and if it is add to the stack or create new stack if last stack is full
                GameObject lastItem = FindLastEquippableItem(newItem.GetComponent<InventoryItem>());
                if (lastItem != null)
                {
                    AddUntilNoItemsLeft(lastItem, (int)amount, ref _equippableItems, _maxStackAmount);
                    OnlyDisplayCurrentPage();
                    return;
                }
            }
            AddNewItemToInventory(newItem, amount, ref _equippableItems, _maxStackAmount);
        }
        OnlyDisplayCurrentPage();
        DisplayEquippableItems();
    }

    public void UseItem(string submitCommand)
    {
        if(Input.GetButtonDown(submitCommand) && _isOpen)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (_isOpen)
        {
            if (_items == null) return;
            if (_items.Count == 0) return;
            if (_items[_currentlySelectedIndex] == null) return;
            InventoryItem item = _items[_currentlySelectedIndex].GetComponent<InventoryItem>();
            bool wasUsed = true;
            if(item.useFunction != null)
            {
                Debug.Log("has function");
                wasUsed = item.useFunction();
            }
            else
            {
                if (_usedItems == null) _usedItems = new List<GameObject>();
                _usedItems.Add(_items[_currentlySelectedIndex]);
            }
            if(wasUsed)
            {
                item.NumberOfItems--;
                DisplayInfoOnCurrentItem();
                if (item.NumberOfItems <= 0)
                {
                    GameObject obj = _items[_currentlySelectedIndex];
                    _items.Remove(_items[_currentlySelectedIndex]);
                    Destroy(obj);
                    _currentlySelectedIndex--;
                    if (_currentlySelectedIndex < 0) _currentlySelectedIndex = 0;
                    OnlyDisplayCurrentPage();
                    DisplayEquippableItems();
                    Debug.Log("Removing used item from inventory");
                }
                int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
                int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows) - 1;
                //List<GameObject> currentPage = _items.GetRange(0 + ((maxItemsPerRow * maxRows) * _currentPageNumber), (lastPageIndex - initialPageIndex) + 1);
                if(initialPageIndex >= _items.Count)
                {
                    _currentPageNumber--;
                    if (_currentPageNumber < 0) _currentPageNumber = 0;
                    OnlyDisplayCurrentPage();
                    DisplayEquippableItems();
                }
            }  
        }
    }

    public void OpenInventory(string openCommand)
    {
        if(Input.GetButtonDown(openCommand))
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        _isOpen = true;
        Debug.Log("Inventory Opened");
        if (_items != null)
        {
            int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
            int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows) - 1;
            if (lastPageIndex >= _items.Count)
            {
                lastPageIndex = _items.Count - 1;
            }

            List<GameObject> currentPage = _items.GetRange(0 + ((maxItemsPerRow * maxRows) * _currentPageNumber), (lastPageIndex - initialPageIndex) + 1);
            if (_isOpen)
            {
                foreach (GameObject item in currentPage)
                {
                    item.SetActive(true);
                }
            }
        }

        if(_useDefaultDisplay)
        {
            pagesText.gameObject.SetActive(true);
            totalItemsText.gameObject.SetActive(true);
            if(_displayCurrentItemInfo && _items != null && _items.Count > 0)
            {
                if (cursor != null)
                    cursor.SetActive(true);
                if (_currentItemName != null)
                    _currentItemName.gameObject.SetActive(true);
                if (_currentItemDescription != null)
                    _currentItemDescription.gameObject.SetActive(true);
                if (_currentItemAmount != null)
                    _currentItemAmount.gameObject.SetActive(true);
            }
        }
    }

    public void CloseInventory(string closeCommand)
    {
        if(Input.GetButtonDown(closeCommand))
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        _isOpen = false;
        Debug.Log("Inventory Opened");
        if(_items != null)
        {
            foreach (GameObject obj in _items)
            {
                obj.SetActive(false);
            }
        }
        if(_useDefaultDisplay)
        {
            pagesText.gameObject.SetActive(false);
            totalItemsText.gameObject.SetActive(false);
            if (cursor != null)
                cursor.SetActive(false);
            if (_currentItemName != null)
                _currentItemName.gameObject.SetActive(false);
            if (_currentItemDescription != null)
                _currentItemDescription.gameObject.SetActive(false);
            if (_currentItemAmount != null)
                _currentItemAmount.gameObject.SetActive(false);
        }
    }

    public void GoToNextItem()
    {
        if(_isOpen)
        {
            int indexOfLastItemOnCurrentPage = ((maxItemsPerRow * maxRows) * (_currentPageNumber + 1)) - 1;
            if(_items.Count > _currentlySelectedIndex + 1)
            {
                if(_currentlySelectedIndex == indexOfLastItemOnCurrentPage && _currentlySelectedIndex < _items.Count - 1)
                {
                    _currentPageNumber++;
                    _currentlySelectedIndex++;
                    DisplayInfoOnCurrentItem();
                    OnlyDisplayCurrentPage();
                    DisplayEquippableItems();
                    return;
                }
                _currentlySelectedIndex++;
                DisplayInfoOnCurrentItem();
                if (cursor != null)
                    cursor.transform.position = _items[_currentlySelectedIndex].transform.position;
            }
        }
    }

    public void GoToPreviousItem()
    {
        if(_isOpen)
        {
            int indexOfFirstItemOnCurrentPage = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
            if(_currentlySelectedIndex > 0)
            {
                if(_currentlySelectedIndex == indexOfFirstItemOnCurrentPage && _currentlySelectedIndex > 0)
                {
                    _currentPageNumber--;
                    _currentlySelectedIndex--;
                    DisplayInfoOnCurrentItem();
                    OnlyDisplayCurrentPage();
                    DisplayEquippableItems();
                    return;
                }
                _currentlySelectedIndex--;
                DisplayInfoOnCurrentItem();
                if (cursor != null)
                    cursor.transform.position = _items[_currentlySelectedIndex].transform.position;
            }
        }
    }

    public void GoToItemBelow()
    {
        if(_isOpen && maxItemsPerRow > 0)
        {
            if (_currentlySelectedIndex == -1) _currentlySelectedIndex++;
            if(_items.Count > _currentlySelectedIndex + maxItemsPerRow)
            {
                _currentlySelectedIndex += maxItemsPerRow;
                DisplayInfoOnCurrentItem();
                if (cursor != null)
                    cursor.transform.position = _items[_currentlySelectedIndex].transform.position;
            }
        }
    }

    public void GoToItemAbove()
    {
        if(_isOpen && _currentlySelectedIndex >= maxItemsPerRow)
        {
            if(_currentlySelectedIndex - maxItemsPerRow >= 0)
            {
                _currentlySelectedIndex -= maxItemsPerRow;
                DisplayInfoOnCurrentItem();
                if (cursor != null)
                    cursor.transform.position = _items[_currentlySelectedIndex].transform.position;
            }
        }
    }

    public void GoToNextItem(string goToNextCommand)
    {
        if(Input.GetButtonDown(goToNextCommand))
        {
            GoToNextItem();
        }
    }

    public void GoToPreviousItem(string goToPreviousCommand)
    {
        if(Input.GetButtonDown(goToPreviousCommand))
        {
            GoToPreviousItem();
        }
    }

    public void GoToItemBelow(string goToBelowCommand)
    {
        if(Input.GetButtonDown(goToBelowCommand))
        {
            GoToItemBelow();
        }
    }

    public void GoToItemAbove(string goToAboveCommand)
    {
        if(Input.GetButtonDown(goToAboveCommand))
        {
            GoToItemAbove();
        }
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

    private void AddFirstItemToInventory(GameObject newItem, uint amount, ref List<GameObject> listToAddTo, uint maxAmountOfStacks)
    {
        listToAddTo = new List<GameObject>();
        InventoryItem script = newItem.GetComponent<InventoryItem>();
        if ((script.NumberOfItems + amount) <= script.MaxItemsPerStack)
        {
            script.NumberOfItems = script.NumberOfItems + amount;
            if (_useDefaultDisplay)
            {
                script.Position = initialItemPosition;
                script.SetUpDisplay();
                script.SetParentTransform(initialTransform);
                script.SetCanvasAsParent();
            }
            listToAddTo.Add(newItem);
            if (!_isOpen)
            {
                listToAddTo[listToAddTo.Count - 1].SetActive(false);
            }
            _currentPageNumber = 0;
            _totalNumberOfPages = 1;
            _currentlySelectedIndex = 0;
            if (cursor != null)
                cursor.transform.position = listToAddTo[_currentlySelectedIndex].transform.position;
            if (_useDefaultDisplay) OnlyDisplayCurrentPage();           
            return;
        }
        else if ((script.NumberOfItems + amount) > script.MaxItemsPerStack && listToAddTo.Count < maxAmountOfStacks)
        {
            uint remainingItems = amount - (script.MaxItemsPerStack - script.NumberOfItems);
            script.NumberOfItems = script.MaxItemsPerStack;
            if (_useDefaultDisplay)
            {
                script.Position = initialItemPosition;
                script.SetUpDisplay();
                script.SetParentTransform(initialTransform);
                script.SetCanvasAsParent();
            }
            listToAddTo.Add(newItem);
            _currentlySelectedIndex = 0;
            if (cursor != null)
                cursor.transform.position = listToAddTo[_currentlySelectedIndex].transform.position;
            if (!_isOpen)
            {
                listToAddTo[listToAddTo.Count - 1].SetActive(false);
            }
            if(_useDefaultDisplay) OnlyDisplayCurrentPage();
            AddUntilNoItemsLeft(newItem, (int)remainingItems, ref listToAddTo, maxAmountOfStacks);
            return;
        }        
    }

    private void AddUntilNoItemsLeft(GameObject itemType, int amountOfItems, ref List<GameObject> listToAddTo, uint maxAmountOfStacks)
    {
        InventoryItem script = itemType.GetComponent<InventoryItem>();

        if ((script.NumberOfItems + amountOfItems) <= script.MaxItemsPerStack)
        {
            script.NumberOfItems = (uint)(script.NumberOfItems + amountOfItems);
            return;
        }

        if(script.NumberOfItems < script.MaxItemsPerStack)
        {
            amountOfItems = (int)(amountOfItems - (script.MaxItemsPerStack - script.NumberOfItems));
            script.NumberOfItems = script.MaxItemsPerStack;
        }

        while (amountOfItems > 0 && listToAddTo.Count < maxAmountOfStacks)
        {
            if(amountOfItems > script.MaxItemsPerStack)
            {
                amountOfItems -= (int)script.MaxItemsPerStack;
                GameObject newobj = Instantiate(itemType);
                newobj.GetComponent<InventoryItem>().NumberOfItems = script.MaxItemsPerStack;
                InventoryItem newScript = newobj.GetComponent<InventoryItem>();
                if (_useDefaultDisplay)
                {
                    newScript.Position = initialItemPosition;
                    newScript.SetUpDisplay();
                    newScript.SetParentTransform(initialTransform);
                    newScript.SetCanvasAsParent();
                }
                listToAddTo.Add(newobj);
                if (!_isOpen)
                {
                    listToAddTo[listToAddTo.Count - 1].SetActive(false);
                }                
            }
            else
            {
                GameObject newobj = Instantiate(itemType);
                newobj.GetComponent<InventoryItem>().NumberOfItems = (uint)amountOfItems;
                amountOfItems = 0;
                InventoryItem newScript = newobj.GetComponent<InventoryItem>();
                if(_useDefaultDisplay)
                {
                    newScript.Position = initialItemPosition;
                    newScript.SetUpDisplay();
                    newScript.SetParentTransform(initialTransform);
                    newScript.SetCanvasAsParent();
                }

                listToAddTo.Add(newobj);
                if (!_isOpen)
                {
                    listToAddTo[listToAddTo.Count - 1].SetActive(false);
                }
            }
        }
        if (_useDefaultDisplay) OnlyDisplayCurrentPage();
        if (_useDefaultDisplay)
        {
            if(_items != null)
            {
                _totalNumberOfPages = Mathf.FloorToInt((_items.Count - 1) / (maxItemsPerRow * maxRows)) + 1;
                if (_items.Count == 0) _totalNumberOfPages = 0;
                pagesText.text = "Page " + (_currentPageNumber + 1) + " of " + _totalNumberOfPages + " pages";
                totalItemsText.text = "Num Items: " + _items.Count;
            }

        }

    }

    private void AddNewItemToInventory(GameObject newItem, uint amount, ref List<GameObject> listToAddTo, uint maxAmountOfStacks)
    {
        if (1 + listToAddTo.Count > _maxStackAmount) return;//don't add to the inventory when it's full
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        InventoryItem script = newItem.GetComponent<InventoryItem>();
        script.Position = initialItemPosition;
        script.SetUpDisplay();
        script.SetParentTransform(initialTransform);
        script.SetCanvasAsParent();
        listToAddTo.Add(newItem);
        if(_useDefaultDisplay) OnlyDisplayCurrentPage();
        if (!_isOpen)
        {
            listToAddTo[listToAddTo.Count - 1].SetActive(false);
        }

        if(_currentlySelectedIndex == -1)
        {
            _currentlySelectedIndex = 0;
        }
    }

    private GameObject FindLastAddedStackOfItem(InventoryItem item)
    {
        return _items.FindLast(x => x.GetComponent<InventoryItem>().Name == item.Name);
    }

    private GameObject FindLastEquippableItem(InventoryItem item)
    {
        return _equippableItems.FindLast(x => x.GetComponent<InventoryItem>().Name == item.Name);
    }

    private void OnlyDisplayCurrentPage()
    {
        if (_items == null || _items.Count == 0) return;
        int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
        int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows) - 1;
        if(lastPageIndex >= _items.Count)
        {
            lastPageIndex = _items.Count - 1;
        }
        if (lastPageIndex < 0) return;
        List<GameObject> currentPage = _items.GetRange(0 + ((maxItemsPerRow * maxRows) * _currentPageNumber), (lastPageIndex - initialPageIndex) + 1);
        if(_isOpen)
        {
            foreach (GameObject item in currentPage)
            {
                item.SetActive(true);
            }
        }
        if(_useDefaultDisplay) SetPositionsForCurrentPage(currentPage, initialTransform, true);

        for (int i = 0; i < _items.Count; i++)
        {
            if(i >= initialPageIndex && i <= lastPageIndex)
            {
                continue;
            }
            _items[i].SetActive(false);
        }
        if(pagesText != null)
            pagesText.text = "Page " + (_currentPageNumber + 1) + " of " + _totalNumberOfPages + " pages";
        if(totalItemsText != null)
            totalItemsText.text = "Num Items: " + _items.Count;
    }

    private void DisplayEquippableItems()
    {
        if (_equippableItems == null || _equippableItems.Count == 0) return;
        int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentEquippablePageNumber);
        int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows) - 1;
        if (lastPageIndex >= _equippableItems.Count)
        {
            lastPageIndex = _equippableItems.Count - 1;
        }
        if (lastPageIndex < 0) return;
        List<GameObject> currentPage = _equippableItems.GetRange(0 + ((maxItemsPerRow * maxRows) * _currentPageNumber), (lastPageIndex - initialPageIndex) + 1);
        if (_isOpen)
        {
            foreach (GameObject item in currentPage)
            {
                item.SetActive(true);
            }
        }
        if (_useDefaultDisplay) SetPositionsForCurrentPage(currentPage, initialEquippableTransform, false);

        for (int i = 0; i < _equippableItems.Count; i++)
        {
            if (i >= initialPageIndex && i <= lastPageIndex)
            {
                continue;
            }
            _equippableItems[i].SetActive(false);
        }
    }

    private void SetPositionsForCurrentPage(List<GameObject> currentPage, Transform transform, bool displayingMainInventory)
    {
        Vector3 originalPosition = (transform != null) ? transform.position : new Vector3(0,0,0);
        int countOnRow = 0;
        foreach (GameObject item in currentPage)
        {
            Debug.Log(originalPosition);
            item.GetComponent<RectTransform>().anchoredPosition = originalPosition;
            countOnRow++;
            if(countOnRow >= maxItemsPerRow)
            {
                countOnRow = 0;
                originalPosition.x = transform.position.x;
                originalPosition.y += columnOffset;
            }
            else
            {
                originalPosition.x += rowOffset;
            }
        }
        if(displayingMainInventory)
        {
            if (cursor != null)
                cursor.transform.position = _items[_currentlySelectedIndex].transform.position;
            if (_displayCurrentItemInfo)
                DisplayInfoOnCurrentItem();
        }
    }

    private void DisplayInfoOnCurrentItem()
    {
        InventoryItem currentItem = _items[_currentlySelectedIndex].GetComponent<InventoryItem>();
        Vector2 currentItemPosition = _items[_currentlySelectedIndex].transform.position;
        if(_currentItemName != null && _currentNameOffset != null)
        {
            string text = (!string.IsNullOrEmpty(currentItem.Name)) ? currentItem.Name : "null";
            _currentItemName.text = "Name: " + text;
            _currentItemName.transform.position = currentItemPosition + _currentNameOffset;
        }
        if(_currentItemDescription != null && _currentDescriptionOffset != null)
        {
            string text = (!string.IsNullOrEmpty(currentItem.Description)) ? currentItem.Description : "null";
            _currentItemDescription.text = "Description: " + text;
            _currentItemDescription.transform.position = currentItemPosition + _currentDescriptionOffset;
        }
        if(_currentItemAmount != null && _currentAmountOffset != null)
        {
            _currentItemAmount.text = "Amount: " + currentItem.NumberOfItems;
            _currentItemAmount.transform.position = currentItemPosition + _currentAmountOffset;
            if(currentItem.DisplayAmountOfItems && _displayCurrentItemInfo)
            {
                _currentItemAmount.enabled = true;
            }
            else
            {
                _currentItemAmount.enabled = false;
            }
        }
    }

    private bool IsOnCurrentPage(int index)
    {
        int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
        int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows);
        return (index >= initialPageIndex && index <= lastPageIndex);
    }

    private void HideOrShowInventory()
    {
        if(!_isOpen)
        {
            if(_useDefaultDisplay)
            {
                foreach(GameObject item in _items)
                {
                    item.SetActive(false);
                }
                _currentItemAmount.gameObject.SetActive(false);
                _currentItemName.gameObject.SetActive(false);
                _currentItemDescription.gameObject.SetActive(false);
                cursor.gameObject.SetActive(false);
                pagesText.gameObject.SetActive(false);
                totalItemsText.gameObject.SetActive(false);
                
            }
        }
        else
        {
            if(_useDefaultDisplay)
            {
                _currentItemAmount.gameObject.SetActive(true);
                _currentItemName.gameObject.SetActive(true);
                _currentItemDescription.gameObject.SetActive(true);
                cursor.gameObject.SetActive(true);
                pagesText.gameObject.SetActive(true);
                totalItemsText.gameObject.SetActive(true);
                DisplayInfoOnCurrentItem();
                OnlyDisplayCurrentPage();
                DisplayEquippableItems();
            }
        }
    }

    public void LoadFromJsonFile()
    {
        InventorySaveData data = null;
        string json;
        if(useDefaultLocation)
        {
            if(!File.Exists(Application.persistentDataPath + jsonToLoadFrom + ".json"))
            {
                Debug.LogError("Tried to load json from persistent path but the file could not be found.");
                return;
            }
            json = File.ReadAllText(Application.persistentDataPath + jsonToLoadFrom + ".json");
            data = JsonUtility.FromJson<InventorySaveData>(json);
        }
        else
        {
            if (!File.Exists(pathToLoadJsonFrom + jsonToLoadFrom + ".json"))
            {
                Debug.LogError("Tried to load json from the provided path but the file could not be found or the directory did not exist.");
                return;
            }
            json = File.ReadAllText(pathToLoadJsonFrom + jsonToLoadFrom + ".json");
            data = JsonUtility.FromJson<InventorySaveData>(json);
        }
        LoadFromSaveData(data);
        HideOrShowInventory();
        return;
    }

    public string SaveToJsonString(bool prettyPrint)
    {
        return JsonUtility.ToJson(GetSaveDataForInventory(), prettyPrint);
    }

    public void SaveToJson()
    {
        if(string.IsNullOrEmpty(pathToJson) && !useDefaultLocation)
        {
            Debug.LogError("No Path Given to where the json should be stored and the default path is not being used.");
            return;
        }
        if(string.IsNullOrEmpty(jsonName))
        {
            Debug.LogError("No name given to the json file that should be created. No File has been created.");
            return;
        }
        InventorySaveData oldData = null;
        if((File.Exists(pathToJson + jsonName + ".json") && !useDefaultLocation) || File.Exists(Application.persistentDataPath + jsonName + ".json"))
        {
            string json;
            if (useDefaultLocation)
            {
                json = File.ReadAllText(Application.persistentDataPath + jsonName + ".json");
            }
            else
            {
                json = File.ReadAllText(pathToJson + jsonName + ".json");
            }
            oldData = JsonUtility.FromJson<InventorySaveData>(json);
        }
        InventorySaveData saveData = GetSaveDataForInventory();
        if (oldData != null && oldData.name != saveData.name && !forceOverwrite)
        {
            Debug.LogWarning("No Data Saved. File that was tried to be used for saving already stores inventory of different name and overwriting has not been enabled for this inventory.");
            return;
        }
        string inventory = JsonUtility.ToJson(saveData, true);
        if(useDefaultLocation)
        {
            File.WriteAllText(Application.persistentDataPath + jsonName + ".json", inventory);
        }
        else
        {
            if(!Directory.Exists(pathToJson))
            {
                Directory.CreateDirectory(pathToJson);
            }
            File.WriteAllText(pathToJson + jsonName + ".json", inventory);
        }
        
        return;
    }

    private InventorySaveData GetSaveDataForInventory()
    {
        string errorsString = "";
        InventorySaveData data = new InventorySaveData();
        data.maxStackAmount = _maxStackAmount;
        data.useDefaultDisplay = _useDefaultDisplay;
        data.displayCurrentItemInfo = _displayCurrentItemInfo;
        data.currentNameOffset = _currentNameOffset;
        if(_useDefaultDisplay)
        {
            if(_currentItemName != null)
            {
                data.currentItemName = _currentItemName.name;
            }
            else
            {
                errorsString += "Current Item Name Text Does Not Exist.\n";
            }
            if(_currentItemDescription != null)
            {
                data.currentItemDescription = _currentItemDescription.name;
            }
            else
            {
                errorsString += "Current Item Description Text Does Not Exist.\n";
            }
            if(initialTransform != null)
            {
                data.initialTransform = initialTransform.name;
            }
            else
            {
                errorsString += "Initial Transform Does Not Exist.\n";
            }
            
            data.spriteLocations = spriteLocations;
            data.initialItemPosition = initialItemPosition;
            if(pagesText != null)
            {
                data.pagesText = pagesText.name;
            }
            else
            {
                errorsString += "Pages Text Does Not Exist.\n";
            }
            if(totalItemsText != null)
            {
                data.totalItemsText = totalItemsText.name;
            }
            else
            {
                errorsString += "Total Items Text Does Not Exist.\n";
            }
            if(cursor != null)
            {
                data.cursor = cursor.name;
            }
            else
            {
                errorsString += "Cursor Does Not Exist.\n";
            }
            data.rowOffset = rowOffset;
            data.columnOffset = columnOffset;
        }
        data.currentDescriptionOffset = _currentDescriptionOffset;
        data.currentItemAmount = _currentItemAmount.name;
        data.currentAmountOffset = _currentAmountOffset;
        data.items = new List<ItemData>();
        data.usedItems = new List<ItemData>();
        data.equippedItems = new List<ItemData>();
        int currentIndex = 0;
        if(_items != null)
        {
            foreach (GameObject item in _items)
            {
                data.items.Add(item.GetComponent<InventoryItem>().CreateSaveData(_useDefaultDisplay));
                if (!string.IsNullOrEmpty(item.GetComponent<InventoryItem>().savingErrors))
                {
                    errorsString += item.GetComponent<InventoryItem>().savingErrors;
                    errorsString += "Error loading item at index: " + currentIndex + " for the items array\n";
                    Debug.LogWarning("Error loading item at index: " + currentIndex + " for the items array\n");
                }
                currentIndex++;
            }
        }

        currentIndex = 0;
        if(_usedItems != null)
        {
            foreach (GameObject item in _usedItems)
            {
                data.usedItems.Add(item.GetComponent<InventoryItem>().CreateSaveData(_useDefaultDisplay));
                if (!string.IsNullOrEmpty(item.GetComponent<InventoryItem>().savingErrors))
                {
                    errorsString += item.GetComponent<InventoryItem>().savingErrors;
                    errorsString += "Error loading item at index: " + currentIndex + " for the used items array\n";
                    Debug.LogWarning("Error loading item at index: " + currentIndex + " for the used items array\n");
                }
                currentIndex++;
            }
        }

        currentIndex = 0;
        if(_equippableItems != null)
        {
            foreach (GameObject item in _equippableItems)
            {
                data.equippedItems.Add(item.GetComponent<InventoryItem>().CreateSaveData(_useDefaultDisplay));
                if (!string.IsNullOrEmpty(item.GetComponent<InventoryItem>().savingErrors))
                {
                    errorsString += item.GetComponent<InventoryItem>().savingErrors;
                    errorsString += "Error loading item at index: " + currentIndex + " for the used items array\n";
                    Debug.LogWarning("Error loading item at index: " + currentIndex + " for the used items array\n");
                }
                currentIndex++;
            }
        }

        data.isOpen = _isOpen;
        data.openCommand = _openCommand;
        data.closeCommand = _closeCommand;
        data.submitCommand = _submitCommand;
        data.currentlySelectedIndex = _currentlySelectedIndex;
        data.currentPageNumber = _currentPageNumber;
        data.totalNumberOfPages = _totalNumberOfPages;
        data.maxItemsPerRow = maxItemsPerRow;
        data.maxRows = maxRows;
        data.name = gameObject.name;
        if (!string.IsNullOrEmpty(errorsString))
        {
            string fileName = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper() + "-" + "InventorySavingLogFile" + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            StreamWriter writer = new StreamWriter(fileName, true);
            writer.WriteLine("Date created: " + DateTime.Now.ToString("dd-MM-yyyy"));
            writer.WriteLine("Time created: " + DateTime.Now.ToString("hh-mm-ss"));
            writer.Write(errorsString);
            writer.Close();
        }
        return data;
    }

    private void LoadFromSaveData(InventorySaveData saveData)
    {
        string errorsString = "";
        _maxStackAmount = saveData.maxStackAmount;
        _useDefaultDisplay = saveData.useDefaultDisplay;
        _displayCurrentItemInfo = saveData.displayCurrentItemInfo;
        _currentNameOffset = saveData.currentNameOffset;

        List<GameObject> canvasChildren = new List<GameObject>();
        GameObject obj;
        GameObject canvas = GameObject.Find("Canvas");
        if (_useDefaultDisplay)
        {
            if(canvas != null)
            {
                GameObject newTextObject;
                for (int i = 0; i < canvas.transform.childCount; ++i)
                {
                    canvasChildren.Add(canvas.transform.GetChild(i).gameObject);
                }
                obj = canvasChildren.Find(x => x.name == saveData.initialTransform);
                if (obj != null)
                {
                    initialTransform = obj.transform;
                }
                else
                {
                    errorsString += "Unable to load initial transform\n";
                    newTextObject = new GameObject(saveData.initialTransform);
                    newTextObject.transform.SetParent(canvas.transform);
                    initialTransform = newTextObject.transform;
                }
                obj = canvasChildren.Find(x => x.name == saveData.currentItemName);
                if (obj != null)
                {
                    _currentItemName = obj.GetComponent<Text>();
                }
                else
                {
                    errorsString += "Unable to current item name\n";
                    newTextObject = new GameObject(saveData.currentItemName);
                    newTextObject.transform.SetParent(canvas.transform);
                    Text newText = newTextObject.AddComponent<Text>();
                    newText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    newText.color = Color.black;
                    _currentItemName = newText;
                }

                obj = canvasChildren.Find(x => x.name == saveData.currentItemDescription);
                if (obj != null)
                {
                    _currentItemDescription = obj.GetComponent<Text>();
                }
                else
                {
                    errorsString += "Unable to current item description\n";
                    newTextObject = new GameObject(saveData.currentItemDescription);
                    newTextObject.transform.SetParent(canvas.transform);
                    Text newText = newTextObject.AddComponent<Text>();
                    newText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    newText.color = Color.black;
                    _currentItemDescription = newText;
                }
                _currentDescriptionOffset = saveData.currentDescriptionOffset;
                obj = canvasChildren.Find(x => x.name == saveData.currentItemAmount);
                if (obj != null)
                {
                    _currentItemAmount = obj.GetComponent<Text>();
                }
                else
                {
                    errorsString += "Unable to load current item amount\n";
                    newTextObject = new GameObject(saveData.currentItemAmount);
                    newTextObject.transform.SetParent(canvas.transform);
                    Text newText = newTextObject.AddComponent<Text>();
                    newText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    newText.color = Color.black;
                    _currentItemAmount = newText;
                }
            }

           
            _currentAmountOffset = saveData.currentAmountOffset;
        }
        _items.ForEach(x => Destroy(x));
        _items = new List<GameObject>();
        spriteLocations = saveData.spriteLocations;
        int currentIndex = 0;
        foreach (ItemData item in saveData.items)
        {
            GameObject newItem = new GameObject(item.itemTag, typeof(RectTransform));
            InventoryItem script = newItem.AddComponent<InventoryItem>();
            script.SetParentTransform(initialTransform);
            item.usingDefaultDisplay = _useDefaultDisplay;
            bool success = script.LoadFromData(item, spriteLocations);
           
            if(success)
            {
                _items.Add(newItem);
            }
            else
            {
                //this is an error but not neccesarilly a breaking error as the rest of the inventory should be able to be loaded without this item
                errorsString += script.loadingErrors;
                errorsString += "Error loading item at index: " + currentIndex + " for the regular items array\n";
                Debug.LogWarning("Error loading item at index: " + currentIndex + " for the regular items array\n");
                if(newItem.GetComponent<InventoryItem>().Sprite != null)
                {
                    _items.Add(newItem);
                }
            }
            currentIndex++;
        }
        _usedItems.ForEach(x => Destroy(x));
        _usedItems = new List<GameObject>();
        currentIndex = 0;
        foreach (ItemData item in saveData.usedItems)
        {
            GameObject newItem = new GameObject(item.itemTag, typeof(RectTransform));
            InventoryItem script = newItem.AddComponent<InventoryItem>();
            script.SetParentTransform(initialTransform);
            item.usingDefaultDisplay = _useDefaultDisplay;
            bool success = script.LoadFromData(item, spriteLocations);
            if (success)
            {
                _usedItems.Add(newItem);
            }
            else
            {
                //this is an error but not neccesarilly a breaking error as the rest of the inventory should be able to be loaded without this item
                errorsString += script.loadingErrors;
                errorsString += "Error loading item at index: " + currentIndex + " for the used items array\n";
                Debug.LogWarning("Error loading item at index: " + currentIndex + " for the used items array\n");
                if (newItem.GetComponent<InventoryItem>().Sprite != null)
                {
                    _usedItems.Add(newItem);
                }
            }
            currentIndex++;
        }
        _equippableItems.ForEach(x => Destroy(x));
        _equippableItems = new List<GameObject>();
        foreach (ItemData item in saveData.equippedItems)
        {
            GameObject newItem = new GameObject(item.itemTag, typeof(RectTransform));
            InventoryItem script = newItem.AddComponent<InventoryItem>();
            script.SetParentTransform(initialTransform);
            item.usingDefaultDisplay = _useDefaultDisplay;
            bool success = script.LoadFromData(item, spriteLocations);
            if (success)
            {
                _equippableItems.Add(newItem);
            }
            else
            {
                //this is an error but not neccesarilly a breaking error as the rest of the inventory should be able to be loaded without this item
                errorsString += script.loadingErrors;
                errorsString += "Error loading item at index: " + currentIndex + " for the equipped items array\n";
                Debug.LogWarning("Error loading item at index: " + currentIndex + " for the equipped items array\n");
                if (newItem.GetComponent<InventoryItem>().Sprite != null)
                {
                    _equippableItems.Add(newItem);
                }
            }
            currentIndex++;
        }
        currentIndex = 0;
        _isOpen = saveData.isOpen;
        _openCommand = saveData.openCommand;
        _closeCommand = saveData.closeCommand;
        _submitCommand = saveData.submitCommand;
        _currentlySelectedIndex = saveData.currentlySelectedIndex;
        _currentPageNumber = saveData.currentPageNumber;
        _totalNumberOfPages = saveData.totalNumberOfPages;
        if(_useDefaultDisplay)
        {
            initialItemPosition = saveData.initialItemPosition;
            obj = canvasChildren.Find(x => x.name == saveData.pagesText);
            if(obj != null)
            {
                pagesText = obj.GetComponent<Text>();
            }
            else
            {
                errorsString += "Unable to load text for pages\n";
                GameObject newTextObject = new GameObject(saveData.pagesText);
                newTextObject.transform.SetParent(canvas.transform);
                Text newText = newTextObject.AddComponent<Text>();
                newText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                newText.color = Color.black;
                pagesText = newText;
            }
            obj = canvasChildren.Find(x => x.name == saveData.totalItemsText);
            if(obj != null)
            {
                totalItemsText = obj.GetComponent<Text>();
            }
            else
            {
                errorsString += "Unable to load text for total number of items\n";
                GameObject newTextObject = new GameObject(saveData.totalItemsText);
                newTextObject.transform.SetParent(canvas.transform);
                Text newText = newTextObject.AddComponent<Text>();
                newText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                newText.color = Color.black;
                totalItemsText = newText;
            }
        }

        rowOffset = saveData.rowOffset;
        columnOffset = saveData.columnOffset;
        maxItemsPerRow = saveData.maxItemsPerRow;
        maxRows = saveData.maxRows;
        obj = canvasChildren.Find(x => x.name == saveData.cursor);
        if(obj != null && _useDefaultDisplay)
        {
            cursor = obj;
        }
        else
        {
            errorsString += "Unable to load cursor\n";
        }
        name = saveData.name;

        if(!string.IsNullOrEmpty(errorsString))
        {
            string fileName = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper() + "-" + "InventoryLoadingLogFile" + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            StreamWriter writer = new StreamWriter(fileName, true);
            writer.WriteLine("Date created: " + DateTime.Now.ToString("dd-MM-yyyy"));
            writer.WriteLine("Time created: " + DateTime.Now.ToString("hh-mm-ss"));
            writer.Write(errorsString);
            writer.Close();
        }
    }
}

[Serializable]
class InventorySaveData
{
    public uint maxStackAmount = 0;
    public bool useDefaultDisplay;
    public string font;
    public bool displayCurrentItemInfo;
    public string currentItemName;
    public Vector2 currentNameOffset;
    public string currentItemDescription;
    public Vector2 currentDescriptionOffset;
    public string currentItemAmount;
    public Vector2 currentAmountOffset;
    public List<ItemData> items;
    public List<ItemData> usedItems;
    public List<ItemData> equippedItems;
    public bool isOpen;
    public const string notSetString = "not set";
    public string openCommand = notSetString;
    public string closeCommand = notSetString;
    public string submitCommand = notSetString;
    public int currentlySelectedIndex = -1;
    public int currentPageNumber = 0;
    public int totalNumberOfPages = 0;
    public Vector3 initialItemPosition = new Vector3(0, 0, 0);
    public string initialTransform;
    public string pagesText;
    public string totalItemsText;
    public int rowOffset = 10;
    public int columnOffset = 10;
    public int maxItemsPerRow = 0;
    public int maxRows = 0;
    public string cursor;
    public string spriteLocations;
    public string name;
}
