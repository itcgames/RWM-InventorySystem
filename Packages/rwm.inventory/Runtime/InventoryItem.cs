using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private Sprite _sprite;
    private Image _image;
    private int _row;
    private int _col;

    public Canvas canvas;
    public delegate void Use();
    public Use useFunction;
    private Vector3 _position;

    public Vector3 Position { get => _position; set => _position = value; }

    public uint NumberOfItems { get => numberOfItems; set {
            if (value > maxItemsPerStack)
            {
                numberOfItems = maxItemsPerStack;
                return;
            }
            numberOfItems = value;
            if (!isStackable)
            {
                numberOfItems = 1;
                maxItemsPerStack = 1;
            }
        } }
    public string Name { get => itemTag; set => itemTag = value; }
    public uint MaxItemsPerStack { get => maxItemsPerStack; set {
            if (!isStackable) maxItemsPerStack = 1;
            maxItemsPerStack = value;
        } }
    public bool IsStackable { get => isStackable; set => isStackable = value; }

    public Image Image { get => _image; set => _image = value; }

    public Sprite Sprite { get => _sprite; set => _sprite = value; }

    public int Row { get => _row; set => _row = value; }

    public int Col { get => _col; set => _col = value; }

    public void SetToMaxStackAmount()
    {
        numberOfItems = maxItemsPerStack;
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
