using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Engine
{
	public class Vendor
	{
		public string Name { get; set; }
		public BindingList<InventoryItem> Inventory { get; private set; }

		public Vendor(string name)
		{
			Name = name;
			Inventory = new BindingList<InventoryItem>();	
		}
		
		public void AddItemToInventory(Item itemToAdd, int quantity=1)
		{
			InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

			if(item == null)
			{
				//vendor does not have item, add to inventory
				Inventory.Add(new InventoryItem(itemToAdd, quantity));
			}
			else
			{
				//vendor has item, increase quantity
				item.Quantity += quantity;
			}

			OnPropertyChanged("Inventory");
		}

		public void RemoveItemFromInvetory(Item itemToRemove, int quantity = 1)
		{
			InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

			if(item == null)
			{
				//item is not in player's inventory, so ignore
				//maybe raise error here
			}
			else
			{
				//vendor has item, decrease quantity
				item.Quantity -= quantity;

				//no negative quantities
				//maybe raise error here
				if(item.Quantity < 0)
				{
					item.Quantity = 0;
				}

				//if quantity is 0, remove item from list
				if(item.Quantity == 0)
				{
					Inventory.Remove(item);
				}

				//notify UI inventory has changed
				OnPropertyChanged("Inventory");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
