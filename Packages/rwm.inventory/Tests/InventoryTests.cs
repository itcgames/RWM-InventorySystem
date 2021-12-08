using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InventoryTests
    {
        #region general tests
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
        #endregion

        #region stack tests

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

        [UnityTest]
        public IEnumerator CreateExtraStackWhenAddingItemToFullStack()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(3);
            GameObject item = CreateItem(5, true, "item");
            GameObject item2 = CreateItem(5, true, "item");
            GameObject item3 = CreateItem(5, true, "item");
            inventory.AddItem(item, 5);
            inventory.AddItem(item2, 1);
            inventory.AddItem(item3, 7);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 3);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 5);
            Assert.AreEqual(inventory.Items[1].GetComponent<InventoryItem>().NumberOfItems, 5);
            Assert.AreEqual(inventory.Items[2].GetComponent<InventoryItem>().NumberOfItems, 3);
        }

        [UnityTest]
        public IEnumerator AddingExtraItemToFullStackDoesNotOverflowInventory()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 5);
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 5);
        }

        [UnityTest]
        public IEnumerator AddingMultipleNonStackableCreatesExtraStacks()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            GameObject item = CreateItem(1, false, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 2);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
            Assert.AreEqual(inventory.Items[1].GetComponent<InventoryItem>().NumberOfItems, 1);
        }

        [UnityTest]
        public IEnumerator AddingToFullInventoryNonStackableCreatesNoNewStack()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(1, false, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
        }

        [UnityTest]
        public IEnumerator NonStackableItemShouldHaveMaxItemsOf1()
        {
            // Use the Assert class to test conditions
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(5, false, "item");
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
        }
        #endregion

        #region item test
        [UnityTest]
        public IEnumerator NonStackablesOnlyHaveMaxOfOne()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(5, false, "item");
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
        }

        [UnityTest]
        public IEnumerator AddingToNonStackableFullInventoryDoesNothing()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            GameObject item = CreateItem(1, false, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 1);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 1);
        }
        #endregion

        #region Use Items Tests
        [UnityTest]
        public IEnumerator AddUsedItemToList()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.UseItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.UsedItems);
            Assert.AreEqual(1, inventory.UsedItems.Count);
            Assert.AreEqual(0, inventory.Items.Count);//remove item from inventory after it is used
        }

        [UnityTest]
        public IEnumerator DontUseItemWhenClosed()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 1);
            inventory.UseItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNull(inventory.UsedItems);
            Assert.AreEqual(1, inventory.Items.Count);//remove item from inventory after it is used
        }
        #endregion

        #region helper functions
        private GameObject CreateItem(uint maxItemsPerStack, bool isStackable, string itemName)
        {
            GameObject item = new GameObject();
            InventoryItem script = item.AddComponent<InventoryItem>();
            script.MaxItemsPerStack = maxItemsPerStack;
            script.IsStackable = isStackable;
            script.Name = itemName;
            script.NumberOfItems = 0;
            return item;
        }
        #endregion
    }
}
