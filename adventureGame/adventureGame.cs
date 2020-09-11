using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace adventureGame
{
	public partial class adventureGame : Form
	{
		private Player _player;
		private Enemy _currentEnemy;

		public adventureGame()
		{
			InitializeComponent();

			_player = new Player(10, 10, 20, 0, 1);
			MoveTo(World.LocationById(World.LOCATION_ID_HOME));
			_player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

			lblHP.Text = _player.CurrentHP.ToString();
			lblGold.Text = _player.Gold.ToString();
			lblExp.Text = _player.EXP.ToString();
			lblLevel.Text = _player.Level.ToString();
		}

		private void btnNorth_Click(object sender, EventArgs e)
		{
			MoveTo(_player.CurrentLocation.LocationToNorth);
		}

		private void btnEast_Click(object sender, EventArgs e)
		{
			MoveTo(_player.CurrentLocation.LocationToEast);
		}

		private void btnSouth_Click(object sender, EventArgs e)
		{
			MoveTo(_player.CurrentLocation.LocationToSouth);
		}

		private void btnWest_Click(object sender, EventArgs e)
		{
			MoveTo(_player.CurrentLocation.LocationToWest);
		}

		private void MoveTo(Location newLocation)
		{
			//Does the location have required items
			if(!_player.HasRequiredItemToEnterThisLocation(newLocation))
			{
				rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this place." + Environment.NewLine;
		
				return;
			}

			//update player's current location
			_player.CurrentLocation = newLocation;

			//show/hide movement buttons
			btnNorth.Visible = (newLocation.LocationToNorth != null);
			btnEast.Visible = (newLocation.LocationToEast != null);
			btnSouth.Visible = (newLocation.LocationToSouth != null);
			btnWest.Visible = (newLocation.LocationToWest != null);

			//Display current location name and description
			rtbLocation.Text = newLocation.Name + Environment.NewLine;
			rtbLocation.Text += newLocation.Description + Environment.NewLine;

			//heal player
			_player.CurrentHP = _player.MaxHP;

			//update HP in UI
			lblHP.Text = _player.CurrentHP.ToString();

			//does location have quest?
			if (newLocation.QuestAvailableHere != null)
			{
				//See if player already has quest and if completed
				bool playerHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
				bool playerCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);

				//see if player already has quest
				if(playerHasQuest)
				{
					//if player has not completed quest
					if(!playerCompletedQuest)
					{
						//see if player has all items to complete quest
						bool playerHasItemsForQuest = _player.HasItemsForQuest(newLocation.QuestAvailableHere);

						//player has all items to complete quest
						if(playerHasItemsForQuest)
						{
							//display message
							rtbMessages.Text += Environment.NewLine;
							rtbMessages.Text += "You complete the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;

							//remove quest items from inventory
							_player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

							//Give quest rewards
							rtbMessages.Text += "You receive: " + Environment.NewLine;
							rtbMessages.Text += newLocation.QuestAvailableHere.RewardEXP.ToString() + " EXP" + Environment.NewLine;
							rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " Gold" + Environment.NewLine;
							rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
							rtbMessages.Text += Environment.NewLine;

							_player.EXP += newLocation.QuestAvailableHere.RewardEXP;
							_player.Gold += newLocation.QuestAvailableHere.RewardGold;

							//add reward item to inventory
							_player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

							//mark quest as completed
							_player.MarkQuestCompleted(newLocation.QuestAvailableHere);
						}
					}
				}
				else
				{
					//player does not already have quest

					//display messages
					rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
					rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
					rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
					foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
					{
						if(qci.Quantity == 1)
						{
							rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
						}
						else
						{
							rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
						}
					}
					rtbMessages.Text += Environment.NewLine;

					//add quest to quest list
					_player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
				}
			}

			//Does location have enemy?
			if(newLocation.EnemyLivingHere != null)
			{
				rtbMessages.Text += "You see a " + newLocation.EnemyLivingHere.Name + Environment.NewLine;

				//make new enemy using values from std enemy in World.Enemy list
				Enemy standardEnemy = World.EnemyById(newLocation.EnemyLivingHere.ID);

				_currentEnemy = new Enemy(standardEnemy.ID, standardEnemy.Name, 
					standardEnemy.MaxDamage, standardEnemy.RewardEXP, standardEnemy.RewardGold, standardEnemy.CurrentHP, standardEnemy.MaxHP);

				foreach(LootItem lootItem in standardEnemy.LootTable)
				{
					_currentEnemy.LootTable.Add(lootItem);
				}

				cboWeapons.Visible = true;
				cboPotions.Visible = true;
				btnUseWeapon.Visible = true;
				btnUsePotion.Visible = true;
			}
			else
			{
				_currentEnemy = null;

				cboWeapons.Visible = false;
				cboPotions.Visible = false;
				btnUseWeapon.Visible = false;
				btnUsePotion.Visible = false;
			}

			//refresh inventory list
			UpdateInventoryListUI();

			//refresh quest list
			UpdateQuestListUI();

			//refresh weapon combo box
			UpdateWeaponListUI();

			//refresh potions combo box
			UpdatePotionsListUI();
		}

		private void UpdateInventoryListUI()
		{
			dgvInventory.RowHeadersVisible = false;

			dgvInventory.ColumnCount = 2;
			dgvInventory.Columns[0].Name = "Name";
			dgvInventory.Columns[0].Width = 197;
			dgvInventory.Columns[1].Name = "Quantity";

			dgvInventory.Rows.Clear();

			foreach (InventoryItem inventoryItem in _player.Inventory)
			{
				if (inventoryItem.Quantity > 0)
				{
					dgvInventory.Rows.Add(new[] { 
						inventoryItem.Details.Name, 
						inventoryItem.Quantity.ToString() });
				}
			}
		}

		private void UpdateQuestListUI()
		{
			dgvQuests.RowHeadersVisible = false;

			dgvQuests.ColumnCount = 2;
			dgvQuests.Columns[0].Name = "Name";
			dgvQuests.Columns[0].Width = 197;
			dgvQuests.Columns[1].Name = "Done?";

			dgvQuests.Rows.Clear();

			foreach (PlayerQuest playerQuest in _player.Quests)
			{
				dgvQuests.Rows.Add(new[] { 
					playerQuest.Details.Name, 
					playerQuest.IsCompleted.ToString() });
			}
		}

		private void UpdateWeaponListUI()
		{
			List<Weapon> weapons = new List<Weapon>();

			foreach (InventoryItem inventoryItem in _player.Inventory)
			{
				if (inventoryItem.Details is Weapon)
				{
					if (inventoryItem.Quantity > 0)
					{
						weapons.Add((Weapon)inventoryItem.Details);
					}
				}
			}

			if (weapons.Count == 0)
			{
				//player doesn't have any weapons, hide combobox and "use" button
				cboWeapons.Visible = false;
				btnUseWeapon.Visible = false;
			}
			else
			{
				cboWeapons.DataSource = weapons;
				cboWeapons.DisplayMember = "Name";
				cboWeapons.ValueMember = "ID";

				cboWeapons.SelectedIndex = 0;
			}
		}

		private void UpdatePotionsListUI()
		{
			List<HealingPotion> healingPotions = new List<HealingPotion>();

			foreach (InventoryItem inventoryItem in _player.Inventory)
			{
				if (inventoryItem.Details is HealingPotion)
				{
					if (inventoryItem.Quantity > 0)
					{
						healingPotions.Add((HealingPotion)inventoryItem.Details);
					}
				}
			}

			if (healingPotions.Count == 0)
			{
				//no potions in inventory, hide potions combo box and "use" button
				cboPotions.Visible = false;
				btnUsePotion.Visible = false;
			}
			else
			{
				cboPotions.DataSource = healingPotions;
				cboPotions.DisplayMember = "Name";
				cboPotions.ValueMember = "ID";

				cboPotions.SelectedIndex = 0;
			}
		}

		private void btnUseWeapon_Click(object sender, EventArgs e)
		{
			//damage enemy with selected weapon
			AttackEnemy();

			//enemy is defeated
			if(_currentEnemy.CurrentHP <= 0)
			{
				EnemyDefeated();
			}
			//enemy survives
			else
			{
				EnemyAttacks();

				//player eats dust
				if(_player.CurrentHP <= 0)
				{
					PlayerEatsDust();
				}
			}
		}

		private void btnUsePotion_Click(object sender, EventArgs e)
		{
			//use selected potion
			PotionUse();

			//monster attacks
			EnemyAttacks();

			//player eats dust
			if (_player.CurrentHP <= 0)
			{
				PlayerEatsDust();
			}

			//refresh player data in UI
			UpdateInventoryListUI();
			UpdatePotionsListUI();
		}

		private void EnemyDefeated()
		{
			//enemy is dead
			rtbMessages.Text += Environment.NewLine;
			rtbMessages.Text += "You defeated the " + _currentEnemy.Name + "! Way to go!" + Environment.NewLine;

			//give exp
			_player.EXP += _currentEnemy.RewardEXP;
			rtbMessages.Text += "You receive " + _currentEnemy.RewardEXP.ToString() + " experience. Keep 'em. You've earned 'em." + Environment.NewLine;

			//give gold
			_player.Gold += _currentEnemy.RewardGold;
			rtbMessages.Text += "You receive " + _currentEnemy.RewardGold.ToString() + " gold. Go buy yourself something nice." + Environment.NewLine;

			//give random loot
			List<InventoryItem> lootedItems = new List<InventoryItem>();

			//add items to lootedItems list, comparing random number to drop percentage
			foreach (LootItem lootItem in _currentEnemy.LootTable)
			{
				if (RNG.NumberBetween(1, 100) <= lootItem.DropPercentage)
				{
					lootedItems.Add(new InventoryItem(lootItem.Details, 1));
				}
			}

			//if no items randomly selected, add default loot items
			if (lootedItems.Count == 0)
			{
				foreach (LootItem lootItem in _currentEnemy.LootTable)
				{
					if (lootItem.IsDefaultItem)
					{
						lootedItems.Add(new InventoryItem(lootItem.Details, 1));
					}
				}
			}

			//add looted items to player's inventory
			foreach (InventoryItem inventoryItem in lootedItems)
			{
				_player.AddItemToInventory(inventoryItem.Details);

				if (inventoryItem.Quantity == 1)
				{
					rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
				}
				else
				{
					rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
				}
			}

			//refresh player info and inventory controls
			lblHP.Text = _player.CurrentHP.ToString();
			lblGold.Text = _player.Gold.ToString();
			lblExp.Text = _player.EXP.ToString();
			lblLevel.Text = _player.Level.ToString();

			UpdateInventoryListUI();
			UpdateWeaponListUI();
			UpdatePotionsListUI();

			//add blank line to messages box for text flow
			rtbMessages.Text += Environment.NewLine;

			//move player to current location (to heal and creat new monster to fight)
			MoveTo(_player.CurrentLocation);
		}

		private void AttackEnemy()
		{
			//get currently selected weapon from cboWeapons
			Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

			//determine damage to enemy
			int damageToEnemy = RNG.NumberBetween(currentWeapon.MinDamage, currentWeapon.MaxDamage);

			//apply damage to enemy
			_currentEnemy.CurrentHP -= damageToEnemy;

			//display message
			rtbMessages.Text += "You hit the " + _currentEnemy.Name + " for " + damageToEnemy.ToString() + " damage. WOW!" + Environment.NewLine;
		}

		private void EnemyAttacks()
		{
			//determine damage enemy does to player
			int damageToPlayer = RNG.NumberBetween(0, _currentEnemy.MaxDamage);

			//display message
			rtbMessages.Text += "The " + _currentEnemy.Name + " did " + damageToPlayer.ToString() + " to you. Are you going to take that?" + Environment.NewLine;

			//subtract damage from player
			_player.CurrentHP -= damageToPlayer;

			//refresh player data in UI
			lblHP.Text = _player.CurrentHP.ToString();
		}

		private void PlayerEatsDust()
		{
			//display message
			rtbMessages.Text += "The " + _currentEnemy.Name + " killed you. You suck at adventuring..." + Environment.NewLine;

			//move player to "home"
			MoveTo(World.LocationById(World.LOCATION_ID_HOME));
		}

		private void PotionUse()
		{
			//get currently selected potion
			HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

			//add healing amouint to player's current hp
			_player.CurrentHP += potion.AmountToHeal;

			//current hp cannot be more than maxHP
			if (_player.CurrentHP > _player.MaxHP)
			{
				_player.CurrentHP = _player.MaxHP;
			}

			//remove potion from inventory
			foreach (InventoryItem ii in _player.Inventory)
			{
				if (ii.Details.ID == potion.ID)
				{
					ii.Quantity--;
					break;
				}
			}

			//display message
			rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
		}

	}
}
