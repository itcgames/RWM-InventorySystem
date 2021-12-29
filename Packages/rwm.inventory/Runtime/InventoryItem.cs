using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryItem : MonoBehaviour
{
    public const string defaultString = "default";
    [Tooltip("This is the name of the item so that the inventory is able to check if it is already in the inventory when adding new items. This name will also be used to display the name " +
        "of the item if the default display is being used and this item is currently selected.")]
    [SerializeField]
    private string _itemTag = null;
    [Tooltip("This is the description of the item that will be displayed if the default display for the inventory is being used and this item is currently selected." +
        "This can be left as null if you are not using the default display inside of the inventory that you will be adding this item to.")]
    [SerializeField]
    [TextArea]
    private string _description = null;
    [Tooltip("Set to true if you want the number of items in the current stack to be displayed inside of the inventory when the item is being hovered over.")]
    [SerializeField]
    private bool _displayNumberOfItems;
    [Tooltip("This is how many of the item is currently in the inventory")]
    private uint _numberOfItems = 0;
    [Tooltip("This is how many of the item you can store per stack")]
    [SerializeField]
    private uint _maxItemsPerStack = 1;
    [Tooltip("Set to true if you want to be able to have more than one of this item per stack." +
        " Updating number of items without setting this to true won't affect the inventory.")]
    [SerializeField]
    private bool _isStackable = true;
    [Tooltip("This is the sprite that will be used to display the item if the default display is being used in the inventory to display. Can be left as null if you do not want to use the" +
        "default display")]
    [SerializeField]
    private Sprite _sprite;
    private Image _image;
    private int _row;
    private int _col;

    public Canvas canvas;
    [SerializeField]
    public delegate bool Use();//should return true if succeeded, otherwise false
    public Use useFunction;
    private Vector3 _position;

    public Vector3 Position { get => _position; set => _position = value; }

    public bool DisplayAmountOfItems { get => _displayNumberOfItems; set => _displayNumberOfItems = value; }

    public string Description { get => _description; set => _description = value; }

    public uint NumberOfItems { get => _numberOfItems; set {
            if (value > _maxItemsPerStack)
            {
                _numberOfItems = _maxItemsPerStack;
                return;
            }
            _numberOfItems = value;
            if (!_isStackable)
            {
                _numberOfItems = 1;
                _maxItemsPerStack = 1;
            }
        } }
    public string Name { get => _itemTag; set => _itemTag = value; }
    public uint MaxItemsPerStack { get => _maxItemsPerStack; set {
            if (!_isStackable) _maxItemsPerStack = 1;
            _maxItemsPerStack = value;
        } }
    public bool IsStackable { get => _isStackable; set => _isStackable = value; }

    public Image Image { get => _image; set => _image = value; }

    public Sprite Sprite { get => _sprite; set => _sprite = value; }

    public int Row { get => _row; set => _row = value; }

    public int Col { get => _col; set => _col = value; }

    public void SetToMaxStackAmount()
    {
        _numberOfItems = _maxItemsPerStack;
    }

    private void Start()
    {
        _image = gameObject.GetComponent<Image>();
        if(_image == null)
        {
            _image = gameObject.AddComponent<Image>();
            if(_sprite != null)
            {
                _image.sprite = _sprite;
            }          
        }       
    }

    public void SetUpDisplay()
    {
        RectTransform trans = gameObject.GetComponent<RectTransform>();
        if(trans == null)
        {
            trans = gameObject.AddComponent<RectTransform>();
        }
        trans.localScale = Vector3.one;
        trans.anchoredPosition = (_position != null) ? _position : new Vector3(0,0,0);
        trans.sizeDelta = new Vector2(30, 30);
    }

    public void SetParentTransform(Transform transform)
    {
        if (transform == null || canvas == null) return;
        RectTransform trans = gameObject.GetComponent<RectTransform>();
        trans.SetParent(canvas.transform);
    }

    public void SetCanvasAsParent()
    {
        if (transform == null || canvas == null) return;
        if (transform.parent != canvas.transform)
        {
            RectTransform trans = gameObject.GetComponent<RectTransform>();
            trans.SetParent(canvas.transform);
        }
    }
}
