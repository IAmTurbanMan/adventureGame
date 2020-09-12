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
		public int Level
		{
			get { return ((EXP / 100) + 1); }
		}
		public List<InventoryItem> Inventory { get; set; }
		public List<PlayerQuest> Quests { get; set; }
		public Location CurrentLocation { get; set; }

		public Player(int currenthp, int maxhp, int gold, int exp) : base(currenthp, maxhp)
		{
			Gold = gold;
			EXP = exp;

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
			return Inventory.Exists(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
		}

		public bool HasThisQuest(Quest quest)
		{
			return Quests.Exists(pq => pq.Details.ID == quest.ID);
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
			//see if player has all quest items
			foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
			{
				//check each item in inventory to see if enough of qci is there
				if(!Inventory.Exists(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
				{
					return false;
				}
				//if it gets here, there is enough in inventory
				return true;
			}

			//player does have items in inventory if we get here
			return true;
		}

		public void RemoveQuestCompletionItems(Quest quest)
		{
			foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
			{
				InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);

				if(item != null)
				{
					//subtract qci quantity from player's inventory
					item.Quantity -= qci.Quantity;
				}
			}
		}

		public void AddItemToInventory(Item itemToAdd)
		{
			InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

			if(item == null)
			{
				//didn't have item, so add 1 to inventory
				Inventory.Add(new InventoryItem(itemToAdd, 1));
			}
			else
			{
				//have item, so increase quantity by 1
				item.Quantity++;
			}
		}

		public void MarkQuestCompleted(Quest quest)
		{
			//find quest in Quests list
			PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);

			if(playerQuest != null)
			{
				playerQuest.IsCompleted = true;
			}
		}

	}
}
