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
            inventory.SetMaxStackAmount(0);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items, null);
            Assert.AreEqual(inventory.MaxStackAmount, 0);
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

        [UnityTest]
        public IEnumerator AddingMultipleOfItemCreatesMultipleStacks()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(3);
            GameObject item = CreateItem(2, true, "item");
            inventory.AddItem(item, 6);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(inventory.Items.Count, 3);
            Assert.AreEqual(inventory.Items[0].GetComponent<InventoryItem>().NumberOfItems, 2);
            Assert.AreEqual(inventory.Items[1].GetComponent<InventoryItem>().NumberOfItems, 2);
            Assert.AreEqual(inventory.Items[2].GetComponent<InventoryItem>().NumberOfItems, 2);
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
        public IEnumerator NoItemsUsedWhenInventoryIsNull()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            inventory.OpenInventory();
            inventory.UseItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNull(inventory.UsedItems);
            Assert.IsNull(inventory.Items);//remove item from inventory after it is used
        }

        [UnityTest]
        public IEnumerator NoItemsUsedWhenInventoryEmpty()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.UseItem();
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
            Assert.AreEqual(1, inventory.Items.Count);
        }

        [UnityTest]
        public IEnumerator UseFunctionIfAble()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(5, true, "item");
            item.GetComponent<InventoryItem>().useFunction += UseItem;
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.UseItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNull(inventory.UsedItems);
            Assert.AreEqual(0, inventory.Items.Count);
        }
        #endregion

        #region Updating Active Item
        [UnityTest]
        public IEnumerator ActiveItemSetWhenInventoryHasItem()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(5, true, "item");
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator ActiveItemIndexStaysNegativeWhenNoItems()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            inventory.OpenInventory();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNull(inventory.Items);
            Assert.AreEqual(-1, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CanMoveRightWhenThereAreMultipleItems()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToNextItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(2, inventory.Items.Count);
            Assert.AreEqual(1, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CannotMoveRightWhenNoMoreToRight()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToNextItem();
            inventory.GoToNextItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CanMoveLeft()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToNextItem();
            inventory.GoToPreviousItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(2, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CannotMoveLeft()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(2);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.AddItem(item, 1);
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToPreviousItem();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(2, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CanMoveDown()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(9);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.maxItemsPerRow = 8;
            inventory.AddItem(item, 9);
            inventory.OpenInventory();
            inventory.GoToItemBelow();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(9, inventory.Items.Count);
            Assert.AreEqual(8, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CannotMoveDown()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.maxItemsPerRow = 1;
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToItemBelow();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CanMoveUp()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(9);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.maxItemsPerRow = 8;
            inventory.AddItem(item, 9);
            inventory.OpenInventory();
            inventory.GoToItemBelow();
            inventory.GoToItemAbove();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(9, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
        }

        [UnityTest]
        public IEnumerator CannotMoveUp()
        {
            Inventory inventory = new Inventory();
            inventory.SetMaxStackAmount(1);
            yield return new WaitForSeconds(0.1f);
            GameObject item = CreateItem(1, true, "item");
            inventory.maxItemsPerRow = 1;
            inventory.AddItem(item, 1);
            inventory.OpenInventory();
            inventory.GoToItemAbove();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual(0, inventory.ActiveItemIndex);
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

        public bool UseItem()
        {
            Debug.Log("using an item in the tests");
            return true;
        }
        #endregion
    }
}
