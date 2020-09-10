using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Player : Creature
	{
		public int Gold { get; set; }
		public int EXP { get; set; }
		public int Level { get; set; }
		public List<InventoryItem> Inventory { get; set; }
		public List<PlayerQuest> Quests { get; set; }
		public Location CurrentLocation { get; set; }

		public Player(int currenthp, int maxhp, int gold, int exp, int level) : base(currenthp, maxhp)
		{
			Gold = gold;
			EXP = exp;
			Level = level;

			Inventory = new List<InventoryItem>();
			Quests = new List<PlayerQuest>();
		}

		public bool HasRequiredItemToEnterThisLocation(Location location)
		{
			if(location.ItemRequiredToEnter == null)
			{
				//no item required to enter, return true
				return true;
			}

			//see if player has item required in inventory
			foreach(InventoryItem ii in Inventory)
			{
				if(ii.Details.ID == location.ItemRequiredToEnter.ID)
				{
					//found item, return true
					return true;
				}
			}

			//item is not in inventory, return false
			return false;
		}

		public bool HasThisQuest(Quest quest)
		{
			foreach(PlayerQuest pq in Quests)
			{
				if(pq.Details.ID == quest.ID)
				{
					return true;
				}
			}
			return false;
		}

		public bool CompletedThisQuest(Quest quest)
		{
			foreach(PlayerQuest pq in Quests)
			{
				if(pq.Details.ID == quest.ID)
				{
					return pq.IsCompleted;
				}
			}
			return false;
		}

		public bool HasItemsForQuest(Quest quest)
		{
			//see if player ahs all quest items
			foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
			{
				bool foundItemInInventory = false;

				//check each item in inventory to see if enough of qci is there
				foreach(InventoryItem ii in Inventory)
				{
					//item is in inventory
					if(ii.Details.ID == qci.Details.ID)
					{
						foundItemInInventory = true;

						//not enough to complete quest
						if(ii.Quantity < qci.Quantity)
						{
							return false;
						}
					}
				}

				//player does not have any of this item in inventory
				if(!foundItemInInventory)
				{
					return false;
				}
			}

			//player does have items in inventory if we get here
			return true;
		}

		public void RemoveQuestCompletionItems(Quest quest)
		{
			foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
			{
				foreach (InventoryItem ii in Inventory)
				{
					if (ii.Details.ID == qci.Details.ID)
					{
						//subtract needed quantity from inventory
						ii.Quantity -= qci.Quantity;
						break;
					}
				}
			}
		}

		public void AddItemToInventory(Item itemToAdd)
		{
			foreach (InventoryItem ii in Inventory)
			{
				if (ii.Details.ID == itemToAdd.ID)
				{
					//already have item, so add to quantity
					ii.Quantity++;

					return; //added item, so get out of function
				}
			}
			//didn't have item, so add to inventory
			Inventory.Add(new InventoryItem(itemToAdd, 1));
		}

		public void MarkQuestCompleted(Quest quest)
		{
			//find quest in Quests list
			foreach (PlayerQuest pq in Quests)
			{
				if (pq.Details.ID == quest.ID)
				{
					//mark as completed
					pq.IsCompleted = true;

					return; //found quest, marked complete, get out of function
				}
			}
		}

	}
}
