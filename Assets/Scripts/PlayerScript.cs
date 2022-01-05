using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] itemsToAdd;
    public uint[] numberOfItemsToAdd;
    private Inventory inventory;
    private Vector2 speed = new Vector2(5.0f, 5.0f);
    public GameObject prefab;
    public float maxHealth;
    float _health;
    float _strength;
    float _block;
    public Text currentIndex;
    public Text currentHealth;
    public Text currentStrength;
    public Text currentBlock;
    // Start is called before the first frame update
    void Start()
    {
        _health = 10.0f;
        inventory = GetComponentInChildren<Inventory>();
        currentIndex.enabled = false;
    }

    public bool Heal(float amount)
    {
        if (_health == maxHealth)
        {
            Debug.Log("Player already at full health");
            return false;
        }

        if (_health + amount >= maxHealth)
        {
            Debug.Log("Player healed to full health");
            _health = maxHealth;
        }
        else if (_health + amount < maxHealth)
        {
            Debug.Log("Player healed for amount: " + amount);
            _health += amount;
        }
        currentHealth.text = "Health: " + _health + " out of a max:\n " + maxHealth;
        return true;
    }

    public bool GainStrength(float amount)
    {
        _strength += amount;
        currentStrength.text = "Strength: " + _strength;
        return true;
    }

    public bool GainBlock(float amount)
    {
        _block += amount;
        currentBlock.text = "Block: " + _block;
        return true;
    }

    private void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(speed.x * xInput, speed.y * yInput, 0);
        movement *= Time.deltaTime;
        transform.Translate(movement);
        
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(inventory.IsOpen)
            {
                inventory.CloseInventory();
                currentIndex.enabled = false;
            }
            else
            {
                inventory.OpenInventory();
                currentIndex.enabled = true;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            inventory.UseItem();
        }

        if(inventory.IsOpen)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                inventory.GoToPreviousItem();
            }
            else if(Input.GetKeyDown(KeyCode.L))
            {
                inventory.GoToNextItem();
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                inventory.GoToItemAbove();
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                inventory.GoToItemBelow();
            }
            currentIndex.gameObject.SetActive(true);
        }
        else
        {
            currentIndex.gameObject.SetActive(false);
        }

        currentIndex.text = "Current Index: " + inventory.ActiveItemIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Item")
        {
            GameObject item = Instantiate(collision.gameObject);
            if(item.GetComponent<SpriteRenderer>() != null)
            {
                Destroy(item.GetComponent<SpriteRenderer>());
            }
            if(item.GetComponent<Rigidbody2D>() != null)
            {
                Destroy(item.GetComponent<Rigidbody2D>());
            }
            if (item.GetComponent<BoxCollider2D>() != null)
            {
                Destroy(item.GetComponent<BoxCollider2D>());
            }
            item.GetComponent<InventoryItem>().canvas = collision.gameObject.GetComponent<InventoryItem>().canvas;
            inventory.AddItem(item, 1);
            Destroy(collision.gameObject);
            item.SetActive(false);
        }
    }

    public void SaveToJson()
    {
        inventory.SaveToJson("json/", "test", false, false);
    }

    public void LoadFromJson()
    {
        inventory.LoadFromJsonFile("json/", "test", false);
    }
}
