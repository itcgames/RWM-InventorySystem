using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] itemsToAdd;
    public string[] namesOfItems;
    private Inventory inventory;
    private Vector2 speed = new Vector2(5.0f, 5.0f);
    public GameObject prefab;
    public float maxHealth;
    float _health;
    float _strength;
    float _block;
    float _magic;
    float _sorcery;
    public Text currentIndex;
    public Text currentHealth;
    public Text currentStrength;
    public Text currentBlock;
    public Text currentMagic;
    public Text currentSorcery;
    public Text displayTrading;
    public Text displayRemoving;
    // Start is called before the first frame update
    void Start()
    {
        _health = 10.0f;
        inventory = GetComponentInChildren<Inventory>();
        currentIndex.enabled = false;
        displayTrading.gameObject.SetActive(false);
        displayRemoving.gameObject.SetActive(false);
        for(int i = 0; i < itemsToAdd.Length; ++i)
        {
            itemsToAdd[i] = Instantiate(itemsToAdd[i]);
            itemsToAdd[i].transform.position = new Vector2(1000, 1000);
        }
    }

    private IEnumerator UpdateTrading()
    {
        yield return new WaitForSeconds(2.5f);
        displayTrading.gameObject.SetActive(false);
    }

    private IEnumerator UpdateRemoving()
    {
        yield return new WaitForSeconds(2.5f);
        displayRemoving.gameObject.SetActive(false);
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

    public bool GainMagic(float amount)
    {
        _magic += amount;
        currentMagic.text = "Magic: " + _magic;
        return true;
    }

    public bool GainSorcery(float amount)
    {
        _sorcery += amount;
        currentSorcery.text = "sorcery: " + _sorcery;
        return true;
    }

    public void TradeItem()
    {
        if (inventory.Items == null || inventory.Items.Count == 0) return;
        string itemToSell = "";
        int rnd = Random.Range(0, itemsToAdd.Length);
        GameObject itemToAdd = Instantiate(itemsToAdd[rnd]);
        List<GameObject> items = inventory.Items;
        if(items != null && items.Count > 0)
        {
            rnd = Random.Range(0, items.Count);
            itemToSell = items[rnd].GetComponent<InventoryItem>().Name;
        }
        bool success = inventory.TradeItems(itemToSell, 1, itemToAdd, 1);
        if(!success)
        {
            Destroy(itemToAdd);
            Debug.LogWarning("Could not trade");
        }
        if (!success)
        {
            displayTrading.text = "Could not trade item";
        }
        else
        {
            displayTrading.text = "Traded item";
        }
        displayTrading.gameObject.SetActive(true);
        StartCoroutine(UpdateTrading());
    }

    public void RemoveItem()
    {
        if (inventory.Items == null || inventory.Items.Count == 0) return;
        bool success = inventory.RemoveItem(inventory.Items[0].GetComponent<InventoryItem>().Name, 1);
        if(!success)
        {
            displayRemoving.text = "Could not remove item";
        }
        else
        {
            displayRemoving.text = "Removed item";
        }
        displayRemoving.gameObject.SetActive(true);
        StartCoroutine(UpdateRemoving());
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

            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                inventory.UseEquippableAtCurrentPageIndex(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                inventory.UseEquippableAtCurrentPageIndex(1);
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

}
