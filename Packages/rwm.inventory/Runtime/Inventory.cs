using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool _isOpen;
    private const string _notSetString = "not set";
    private string _openCommand = _notSetString;
    private string _closeCommand = _notSetString;
    private string _submitCommand = _notSetString;
    private string _goToNextCommand = _notSetString;
    private string _goToAboveCommand = _notSetString;
    private string _goToBelowCommand = _notSetString;
    private string _goToPreviousCommand = _notSetString;
    private int _currentlySelectedIndex = -1;
    private int _currentPageNumber = 0;
    private int _totalNumberOfPages = 0;
    public Vector3 initialItemPosition = new Vector3(0,0,0);
    public Transform initialTransform;
    public Text pagesText;
    public Text totalItemsText;
    public int rowOffset = 10;
    public int columnOffset = 10;
    public int maxItemsPerRow = 0;
    public int maxRows = 0;
    [HideInInspector]
    public uint MaxStackAmount { get => _maxStackAmount; }

    [HideInInspector]
    public List<GameObject> Items { get => _items; }

    [HideInInspector]
    public List<GameObject> UsedItems { get => _usedItems;}

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
        if(pagesText != null)
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
        foreach(GameObject obj in _items)
        {
            obj.GetComponent<InventoryItem>().SetCanvasAsParent();
        }
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
                AddUntilNoItemsLeft(lastItem, (int)amount);
                return;
            }
        }
        AddNewItemToInventory(newItem, amount);
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

            if(item.useFunction != null)
            {
                Debug.Log("has function");
                item.useFunction();
            }
            else
            {
                if (_usedItems == null) _usedItems = new List<GameObject>();
                _usedItems.Add(_items[_currentlySelectedIndex]);
            }
            item.NumberOfItems--;
            if (item.NumberOfItems <= 0)
            {
                _items.Remove(_items[_currentlySelectedIndex]);
                if (_items.Count - 1 < _currentlySelectedIndex && _items.Count > 0) _currentlySelectedIndex--;
                if (_items.Count == 0) _currentlySelectedIndex = -1;
                Debug.Log("Removing used item from inventory");
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
        pagesText.gameObject.SetActive(true);
        totalItemsText.gameObject.SetActive(true);
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
        pagesText.gameObject.SetActive(false);
        totalItemsText.gameObject.SetActive(false);
    }

    public void GoToNextItem()
    {
        if(_isOpen)
        {
            if(_items.Count > _currentlySelectedIndex + 1)
            {
                _currentlySelectedIndex++;
            }
        }
    }

    public void GoToPreviousItem()
    {
        if(_isOpen)
        {
            if(_currentlySelectedIndex > 0)
            {
                _currentlySelectedIndex--;
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

    void SetGoToPreviousCommand(string previousCommand)
    {
        _goToPreviousCommand = previousCommand;
    }

    void SetGoToAboveCommand(string aboveCommand)
    {
        _goToAboveCommand = aboveCommand;
    }

    void SetGoToNextCommand(string nextCommand)
    {
        _goToNextCommand = nextCommand;
    }

    void SetGoToBelowCommand(string belowCommand)
    {
        _goToBelowCommand = belowCommand;
    }

    private void AddFirstItemToInventory(GameObject newItem, uint amount)
    {
        _items = new List<GameObject>();
        InventoryItem script = newItem.GetComponent<InventoryItem>();
        if ((script.NumberOfItems + amount) <= script.MaxItemsPerStack)
        {
            script.NumberOfItems = script.NumberOfItems + amount;
            //script.Image
            script.Position = initialItemPosition;
            script.SetUpDisplay();
            script.SetParentTransform(initialTransform);
            script.SetCanvasAsParent();
            _items.Add(newItem);
            if (!_isOpen)
            {
                _items[_items.Count - 1].SetActive(false);
            }
            _currentPageNumber = 0;
            _totalNumberOfPages = 1;
            OnlyDisplayCurrentPage();
            _currentlySelectedIndex = 0;
            return;
        }
        else if ((script.NumberOfItems + amount) > script.MaxItemsPerStack && _items.Count < _maxStackAmount)
        {
            uint remainingItems = amount - (script.MaxItemsPerStack - script.NumberOfItems);
            script.NumberOfItems = script.MaxItemsPerStack;
            script.Position = initialItemPosition;
            script.SetUpDisplay();
            script.SetParentTransform(initialTransform);
            script.SetCanvasAsParent();
            _items.Add(newItem);
            if (!_isOpen)
            {
                _items[_items.Count - 1].SetActive(false);
            }
            OnlyDisplayCurrentPage();
            AddUntilNoItemsLeft(newItem, (int)remainingItems);
            return;
        }        
    }

    private void AddUntilNoItemsLeft(GameObject itemType, int amountOfItems)
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

        while (amountOfItems > 0 && _items.Count < _maxStackAmount)
        {
            if(amountOfItems > script.MaxItemsPerStack)
            {
                amountOfItems -= (int)script.MaxItemsPerStack;
                GameObject newobj = Instantiate(itemType);
                newobj.GetComponent<InventoryItem>().NumberOfItems = script.MaxItemsPerStack;
                InventoryItem newScript = newobj.GetComponent<InventoryItem>();
                newScript.Position = initialItemPosition;
                newScript.SetUpDisplay();
                newScript.SetParentTransform(initialTransform);
                newScript.SetCanvasAsParent();
                _items.Add(newobj);
                if (!_isOpen)
                {
                    _items[_items.Count - 1].SetActive(false);
                }                
            }
            else
            {
                GameObject newobj = Instantiate(itemType);
                newobj.GetComponent<InventoryItem>().NumberOfItems = (uint)amountOfItems;
                amountOfItems = 0;
                InventoryItem newScript = newobj.GetComponent<InventoryItem>();
                newScript.Position = initialItemPosition;
                newScript.SetUpDisplay();
                newScript.SetParentTransform(initialTransform);
                newScript.SetCanvasAsParent();
                _items.Add(newobj);
                if (!_isOpen)
                {
                    _items[_items.Count - 1].SetActive(false);
                }
            }
        }
        OnlyDisplayCurrentPage();
        _totalNumberOfPages = Mathf.FloorToInt((_items.Count - 1) / (maxItemsPerRow * maxRows)) + 1;
        if (_items.Count == 0) _totalNumberOfPages = 0;
        pagesText.text = "Page " + (_currentPageNumber + 1) + " of " + _totalNumberOfPages + " pages";
        totalItemsText.text = "Num Items: " + _items.Count;
    }

    private void AddNewItemToInventory(GameObject newItem, uint amount)
    {
        if (1 + _items.Count > _maxStackAmount) return;//don't add to the inventory when it's full
        newItem.GetComponent<InventoryItem>().NumberOfItems = amount;
        InventoryItem script = newItem.GetComponent<InventoryItem>();
        script.Position = initialItemPosition;
        script.SetUpDisplay();
        script.SetParentTransform(initialTransform);
        script.SetCanvasAsParent();
        _items.Add(newItem);
        OnlyDisplayCurrentPage();
        if (!_isOpen)
        {
            _items[_items.Count - 1].SetActive(false);
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

    private void OnlyDisplayCurrentPage()
    {
        int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
        int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows) - 1;
        if(lastPageIndex >= _items.Count)
        {
            lastPageIndex = _items.Count - 1;
        }

        List<GameObject> currentPage = _items.GetRange(0 + ((maxItemsPerRow * maxRows) * _currentPageNumber), (lastPageIndex - initialPageIndex) + 1);
        if(_isOpen)
        {
            foreach (GameObject item in currentPage)
            {
                item.SetActive(true);
            }
        }
        SetPositionsForCurrentPage(currentPage);

        for (int i = 0; i < _items.Count; i++)
        {
            if(i >= initialPageIndex && i <= lastPageIndex)
            {
                continue;
            }
            _items[i].SetActive(false);
        }

        pagesText.text = "Page " + (_currentPageNumber + 1) + " of " + _totalNumberOfPages + " pages";
        totalItemsText.text = "Num Items: " + _items.Count;
    }

    private void SetPositionsForCurrentPage(List<GameObject> currentPage)
    {
        Vector3 originalPosition = initialTransform.position;
        int countOnRow = 0;
        foreach (GameObject item in currentPage)
        {
            Debug.Log(originalPosition);
            item.GetComponent<RectTransform>().anchoredPosition = originalPosition;
            countOnRow++;
            if(countOnRow >= maxItemsPerRow)
            {
                countOnRow = 0;
                originalPosition.x = initialTransform.position.x;
                originalPosition.y += columnOffset;
            }
            else
            {
                originalPosition.x += rowOffset;
            }
        }
    }

    private bool IsOnCurrentPage(int index)
    {
        int initialPageIndex = 0 + ((maxItemsPerRow * maxRows) * _currentPageNumber);
        int lastPageIndex = initialPageIndex + (maxItemsPerRow * maxRows);
        return (index >= initialPageIndex && index <= lastPageIndex);
    }
}