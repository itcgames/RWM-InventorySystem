using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InventoryTests
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator CreateEmptyInventory()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(10);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items, null);
            Assert.AreEqual(inventory.MaxStackAmount, 10);
        }

        [UnityTest]
        public IEnumerator AddItemToInventory()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            InventoryItem itemScript = inventory.Items[0].GetComponent<InventoryItem>();
            Assert.AreEqual(1, itemScript.NumberOfItems);
        }
        
        [UnityTest]
        public IEnumerator CannotAddMoreToFullStack()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(1, true, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 10);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
        }

        [UnityTest]
        public IEnumerator AddingTwoItemsCreatesTwoStacks()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            GameObject item = CreateItem(5, true, "item");
            GameObject item2 = CreateItem(5, true, "item2");
            inventory.AddItem(item, 1);
            inventory.AddItem(item2, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 2);
        }

        [UnityTest]
        public IEnumerator CannotCreateMoreThanMaxStacks()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(5, true, "item");
            GameObject item2 = CreateItem(5, true, "item2");
            inventory.AddItem(item, 1);
            inventory.AddItem(item2, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
        }

        private GameObject CreateItem(uint maxItemsPerStack, bool isStackable, string itemName)
        {
            GameObject item = new GameObject();
            InventoryItem script = item.AddComponent<InventoryItem>();
            script.maxItemsPerStack = maxItemsPerStack;
            script.isStackable = isStackable;
            script.itemTag = itemName;
            return item;
        }
    }
}
