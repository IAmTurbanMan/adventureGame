using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

namespace Engine
{
	public class Player : Creature
	{
		private Location _currentLocation;
		private Enemy _currentEnemy;
		private int _gold;
		private int _EXP;

		public event EventHandler<MessageEventArgs> OnMessage;
		public int Gold
		{
			get { return _gold; }
			set
			{
				_gold = value;
				OnPropertyChanged("Gold");
			}
		}

		public int EXP
		{
			get { return _EXP; }
			private set
			{
				_EXP = value;
				OnPropertyChanged("EXP");
				OnPropertyChanged("Level");
			}
		}
		public int Level
		{
			get { return ((EXP / 100) + 1); }
		}
		public BindingList<InventoryItem> Inventory { get; set; }
		public BindingList<PlayerQuest> Quests { get; set; }
		public Location CurrentLocation
		{
			get { return _currentLocation; }
			set
			{
				_currentLocation = value;
				OnPropertyChanged("CurrentLocation");
			}
		}
		public Weapon CurrentWeapon { get; set; }
		public List<Weapon> Weapons
		{
			get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
		}
		public List<HealingPotion> Potions
		{
			get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
		}

		private Player(int currenthp, int maxhp, int gold, int exp) : base(currenthp, maxhp)
		{
			Gold = gold;
			EXP = exp;

			Inventory = new BindingList<InventoryItem>();
			Quests = new BindingList<PlayerQuest>();
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

				if (playerData.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
				{
					int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
					player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
				}

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

		private void RaiseMessage(string message, bool addExtraNewLine = false)
		{
			if(OnMessage != null)
			{
				OnMessage(this, new MessageEventArgs(message, addExtraNewLine));
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
			return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
		}

		public bool HasThisQuest(Quest quest)
		{
			return Quests.Any(pq => pq.Details.ID == quest.ID);
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
				if(!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
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
					RemoveItemFromInventory(item.Details, qci.Quantity);
				}
			}
		}

		public void AddItemToInventory(Item itemToAdd, int quantity = 1)
		{
			InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

			if(item == null)
			{
				//didn't have item, so add 1 to inventory
				Inventory.Add(new InventoryItem(itemToAdd, quantity));
			}
			else
			{
				//have item, so increase quantity by 1
				item.Quantity += quantity;
			}

			RaiseInventoryChangedEvent(itemToAdd);
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

			if(CurrentWeapon != null)
			{
				XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
				currentWeapon.AppendChild(playerData.CreateTextNode(this.CurrentWeapon.ID.ToString()));
				stats.AppendChild(currentWeapon);
			}

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

		public void AddEXP(int EXPToAdd)
		{
			EXP += EXPToAdd;
			MaxHP = (Level * 10);
		}

		private void RaiseInventoryChangedEvent(Item item)
		{
			if(item is Weapon)
			{
				OnPropertyChanged("Weapons");
			}

			if(item is HealingPotion)
			{
				OnPropertyChanged("Potions");
			}
		}

		public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
		{
			InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

			if(item == null)
			{
				//item is not in inventory so ignore
				//maybe raise error here
			}
			else
			{
				//item is in inventory, decrease quantity
				item.Quantity -= quantity;

				//no negative quantities
				//maybe raise error here
				if(item.Quantity < 0)
				{
					item.Quantity = 0;
				}

				//if quantity is 0, remove from list
				if(item.Quantity == 0)
				{
					Inventory.Remove(item);
				}

				//notify UI inventory has changed
				RaiseInventoryChangedEvent(itemToRemove);
			}
		}

		private void MoveTo(Location newLocation)
		{
			//Does the location have required items
			if (!HasRequiredItemToEnterThisLocation(newLocation))
			{
				RaiseMessage("You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this place.");
				return;
			}

			//update player's current location
			CurrentLocation = newLocation;

			//heal player
			CurrentHP = MaxHP;

			//does location have quest?
			if (newLocation.QuestAvailableHere != null)
			{
				//See if player already has quest and if completed
				bool playerHasQuest = HasThisQuest(newLocation.QuestAvailableHere);
				bool playerCompletedQuest = CompletedThisQuest(newLocation.QuestAvailableHere);

				//see if player already has quest
				if (playerHasQuest)
				{
					//if player has not completed quest
					if (!playerCompletedQuest)
					{
						//see if player has all items to complete quest
						bool playerHasItemsForQuest = HasItemsForQuest(newLocation.QuestAvailableHere);

						//player has all items to complete quest
						if (playerHasItemsForQuest)
						{
							//display message
							RaiseMessage("You complete the " + newLocation.QuestAvailableHere.Name + " quest.");

							//remove quest items from inventory
							RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

							//Give quest rewards
							RaiseMessage("You receive: ");
							RaiseMessage(newLocation.QuestAvailableHere.RewardEXP.ToString() + " EXP");
							RaiseMessage(newLocation.QuestAvailableHere.RewardGold.ToString() + " Gold");
							RaiseMessage(newLocation.QuestAvailableHere.RewardItem.Name);

							AddEXP(newLocation.QuestAvailableHere.RewardEXP);
							Gold += newLocation.QuestAvailableHere.RewardGold;

							//add reward item to inventory
							AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

							//mark quest as completed
							MarkQuestCompleted(newLocation.QuestAvailableHere);
						}
					}
				}
				else
				{
					//player does not already have quest

					//display messages
					RaiseMessage("You receive the " + newLocation.QuestAvailableHere.Name + " quest.");
					RaiseMessage(newLocation.QuestAvailableHere.Description);
					RaiseMessage("To complete it, return with:");
					foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
					{
						if (qci.Quantity == 1)
						{
							RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name);
						}
						else
						{
							RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.NamePlural);
						}
					}

					//add quest to quest list
					Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
				}
			}

			//Does location have enemy?
			if (newLocation.EnemyLivingHere != null)
			{
				RaiseMessage("You see a " + newLocation.EnemyLivingHere.Name);

				//make new enemy using values from std enemy in World.Enemy list
				Enemy standardEnemy = World.EnemyById(newLocation.EnemyLivingHere.ID);

				_currentEnemy = new Enemy(standardEnemy.ID, standardEnemy.Name,
					standardEnemy.MaxDamage, standardEnemy.RewardEXP, standardEnemy.RewardGold, standardEnemy.CurrentHP, standardEnemy.MaxHP);

				foreach (LootItem lootItem in standardEnemy.LootTable)
				{
					_currentEnemy.LootTable.Add(lootItem);
				}
			}
			else
			{
				_currentEnemy = null;
			}
		}

		public void MoveNorth()
		{
			if(CurrentLocation.LocationToNorth != null)
			{
				MoveTo(CurrentLocation.LocationToNorth);
			}
		}

		public void MoveEast()
		{
			if (CurrentLocation.LocationToEast != null)
			{
				MoveTo(CurrentLocation.LocationToEast);
			}
		}

		public void MoveSouth()
		{
			if (CurrentLocation.LocationToSouth != null)
			{
				MoveTo(CurrentLocation.LocationToSouth);
			}
		}

		public void MoveWest()
		{
			if (CurrentLocation.LocationToWest != null)
			{
				MoveTo(CurrentLocation.LocationToWest);
			}
		}

		public void UseWeapon(Weapon weapon)
		{
			//determine amount of damage to do
			int damageToEnemy = RNG.NumberBetween(weapon.MinDamage, weapon.MaxDamage);

			//apply damage to enemy's currentHP
			_currentEnemy.CurrentHP -= damageToEnemy;

			//display message
			RaiseMessage("You hit the " + _currentEnemy.Name + " for " + damageToEnemy + " damage.");

			//check if enemy is dead
			if(_currentEnemy.CurrentHP <= 0)
			{
				RaiseMessage("");
				RaiseMessage("You defeated the " + _currentEnemy.Name);

				//give player EXP
				AddEXP(_currentEnemy.RewardEXP);
				RaiseMessage("You receive " + _currentEnemy.RewardEXP + " experience.");

				//give player gold
				Gold += _currentEnemy.RewardGold;
				RaiseMessage("You receive " + _currentEnemy.RewardGold + " gold.");

				//get random loot
				List<InventoryItem> lootedItems = new List<InventoryItem>();

				//add items to lootedItems list, comparing random number to drop percentage
				foreach(LootItem lootItem in _currentEnemy.LootTable)
				{
					if(RNG.NumberBetween(1, 100) <= lootItem.DropPercentage)
					{
						lootedItems.Add(new InventoryItem(lootItem.Details, 1));
					}
				}

				//if no items were randomly selected, add default loot item(s)
				if(lootedItems.Count == 0)
				{
					foreach(LootItem lootItem in _currentEnemy.LootTable)
					{
						if (lootItem.IsDefaultItem)
						{
							lootedItems.Add(new InventoryItem(lootItem.Details, 1));
						}
					}
				}

				//add looted items to inventory
				foreach(InventoryItem inventoryItem in lootedItems)
				{
					AddItemToInventory(inventoryItem.Details);

					if(inventoryItem.Quantity == 1)
					{
						RaiseMessage("You loot " + inventoryItem.Quantity + " " + inventoryItem.Details.Name);
					}
					else
					{
						RaiseMessage("You loot " + inventoryItem.Quantity + " " + inventoryItem.Details.NamePlural);
					}
				}

				//add blank line to messages
				RaiseMessage("");

				//move player to current location to respawn enemy and heal player
				MoveTo(CurrentLocation);
			}
			else
			{
				//Enemy is still alive

				//determine enemy damage to player
				int damageToPlayer = RNG.NumberBetween(0, _currentEnemy.MaxDamage);

				//display message
				RaiseMessage("The " + _currentEnemy.Name + " did " + damageToPlayer + " damage to you.");

				//subtract damage from player
				CurrentHP -= damageToPlayer;

				if(CurrentHP <= 0)
				{
					//display message
					RaiseMessage("The " + _currentEnemy + " defeated you.");

					//Move player to home
					MoveHome();
				}
			}
		}

		public void UsePotion(HealingPotion potion)
		{
			//add healing amount to player's currentHP
			CurrentHP += potion.AmountToHeal;

			//current hp cannot exceed maxHP
			if (CurrentHP > MaxHP)
			{
				CurrentHP = MaxHP;
			}

			//remove potion from inventory
			RemoveItemFromInventory(potion, 1);

			//display message
			RaiseMessage("You drink a " + potion.Name + ".");

			//enemy attacks
			//determine enemy damage to player
			int damageToPlayer = RNG.NumberBetween(0, _currentEnemy.MaxDamage);

			//display message
			RaiseMessage("The " + _currentEnemy.Name + " did " + damageToPlayer + " damage to you.");

			//subtract damage from player
			CurrentHP -= damageToPlayer;

			if (CurrentHP <= 0)
			{
				//display message
				RaiseMessage("The " + _currentEnemy + " defeated you.");

				//Move player to home
				MoveHome();
			}
		}

		private void MoveHome()
		{
			MoveTo(World.LocationById(World.LOCATION_ID_HOME));
		}
	}
}
