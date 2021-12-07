using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] itemsToAdd;
    public uint[] numberOfItemsToAdd;
    private Inventory inventory;
    private Vector2 speed = new Vector2(5.0f, 5.0f);
    public GameObject prefab;
    public float maxHealth;
    float _health;
    // Start is called before the first frame update
    void Start()
    {
        _health = maxHealth / 2.0f;
        inventory = GetComponentInChildren<Inventory>();
        for (int i = 0; i < itemsToAdd.Length; i++)
        {
            inventory.AddItem(Instantiate(itemsToAdd[i]), numberOfItemsToAdd[i]);
        }
        inventory.Items.ForEach(item => Debug.Log(item.GetComponent<InventoryItem>().NumberOfItems));
    }

    public void Heal(float amount)
    {
        if (_health == maxHealth)
        {
            Debug.Log("Player already at full health");
            return;
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
            }
            else
            {
                inventory.OpenInventory();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            inventory.UseItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Item")
        {
            GameObject item = Instantiate(collision.gameObject);
            inventory.AddItem(item, 1);
            Destroy(collision.gameObject);
            item.SetActive(false);
        }
    }
}
