using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

		private Player(int currenthp, int maxhp, int gold, int exp) : base(currenthp, maxhp)
		{
			Gold = gold;
			EXP = exp;

			Inventory = new List<InventoryItem>();
			Quests = new List<PlayerQuest>();
		}

		public static Player CreateDefaultPlayer()
		{
			Player player = new Player(10, 10, 20, 0);
			player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
			player.CurrentLocation = World.LocationById(World.LOCATION_ID_HOME);

			return player;
		}

		public static Player CreatePlayerFromXML(string xmlPlayerData)
		{
			try
			{
				XmlDocument playerData = new XmlDocument();

				playerData.LoadXml(xmlPlayerData);

				int currentHP = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHP").InnerText);
				int maxHP = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaxHP").InnerText);
				int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
				int EXP = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/EXP").InnerText);

				Player player = new Player(currentHP, maxHP, gold, EXP);

				int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
				player.CurrentLocation = World.LocationById(currentLocationID);

				foreach(XmlNode node in playerData.SelectNodes("/Player/Inventory/Item"))
				{
					int id = Convert.ToInt32(node.Attributes["ID"].Value);
					int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

					for(int i = 0; i < quantity; i++)
					{
						player.AddItemToInventory(World.ItemByID(id));
					}
				}

				foreach(XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/Quest"))
				{
					int id = Convert.ToInt32(node.Attributes["ID"].Value);
					bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

					PlayerQuest playerQuest = new PlayerQuest(World.QuestById(id));
					playerQuest.IsCompleted = isCompleted;

					player.Quests.Add(playerQuest);
				}

				return player;
			}
			catch
			{
				//if error with XML data, return default player
				return Player.CreateDefaultPlayer();
			}
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

		public string ToXmlString()
		{
			XmlDocument playerData = new XmlDocument();

			//create top-level XML node
			XmlNode player = playerData.CreateElement("Player");
			playerData.AppendChild(player);

			//create "stats" child node
			XmlNode stats = playerData.CreateElement("Stats");
			player.AppendChild(stats);

			//create child nodes for stats
			XmlNode currentHP = playerData.CreateElement("CurrentHP");
			currentHP.AppendChild(playerData.CreateTextNode(this.CurrentHP.ToString()));
			stats.AppendChild(currentHP);

			XmlNode maxHP = playerData.CreateElement("MaxHP");
			maxHP.AppendChild(playerData.CreateTextNode(this.MaxHP.ToString()));
			stats.AppendChild(maxHP);

			XmlNode gold = playerData.CreateElement("Gold");
			gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
			stats.AppendChild(gold);

			XmlNode EXP = playerData.CreateElement("EXP");
			EXP.AppendChild(playerData.CreateTextNode(this.EXP.ToString()));
			stats.AppendChild(EXP);

			XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
			currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
			stats.AppendChild(currentLocation);

			//create "Inventory" node
			XmlNode inventory = playerData.CreateElement("Inventory");
			player.AppendChild(inventory);

			//create an "Item" node for each item in inventory
			foreach(InventoryItem ii in this.Inventory)
			{
				XmlNode item = playerData.CreateElement("Item");

				XmlAttribute idAttribute = playerData.CreateAttribute("ID");
				idAttribute.Value = ii.Details.ID.ToString();
				item.Attributes.Append(idAttribute);

				XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
				quantityAttribute.Value = ii.Details.ID.ToString();
				item.Attributes.Append(quantityAttribute);

				inventory.AppendChild(item);
			}

			//create "PlayerQuests" node
			XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
			player.AppendChild(playerQuests);

			//create "Quests" child node for each quest player has acquired
			foreach(PlayerQuest pq in this.Quests)
			{
				XmlNode quest = playerData.CreateElement("Quest");

				XmlAttribute idAttribute = playerData.CreateAttribute("ID");
				idAttribute.Value = pq.Details.ID.ToString();
				quest.Attributes.Append(idAttribute);

				XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
				isCompletedAttribute.Value = pq.IsCompleted.ToString();
				quest.Attributes.Append(isCompletedAttribute);

				playerQuests.AppendChild(quest);
			}
			//xml doc as string to save data to disk
			return playerData.InnerXml;
		}

	}
}
