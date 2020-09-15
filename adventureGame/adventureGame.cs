using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Engine;

namespace adventureGame
{
	public partial class adventureGame : Form
	{
		private Player _player;
		private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

		public adventureGame()
		{
			InitializeComponent();

			if (File.Exists(PLAYER_DATA_FILE_NAME))
			{
				_player = Player.CreatePlayerFromXML(File.ReadAllText(PLAYER_DATA_FILE_NAME));
			}
			else
			{
				_player = Player.CreateDefaultPlayer();
			}

			lblHP.DataBindings.Add("Text", _player, "CurrentHP");
			lblGold.DataBindings.Add("Text", _player, "Gold");
			lblExp.DataBindings.Add("Text", _player, "EXP");
			lblLevel.DataBindings.Add("Text", _player, "Level");

			dgvInventory.RowHeadersVisible = false;
			dgvInventory.AutoGenerateColumns = false;

			dgvInventory.DataSource = _player.Inventory;

			dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Name",
				Width = 197,
				DataPropertyName = "Description"
			});

			dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Quantity",
				DataPropertyName = "Quantity"
			});

			dgvQuests.RowHeadersVisible = false;
			dgvQuests.AutoGenerateColumns = false;

			dgvQuests.DataSource = _player.Quests;

			dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Name",
				Width = 197,
				DataPropertyName = "Name"
			});

			dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Done?",
				DataPropertyName = "IsCompleted"
			});

			cboWeapons.DataSource = _player.Weapons;
			cboWeapons.DisplayMember = "Name";
			cboWeapons.ValueMember = "Id";

			if(_player.CurrentWeapon != null)
			{
				cboWeapons.SelectedItem = _player.CurrentWeapon;
			}

			cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

			cboPotions.DataSource = _player.Potions;
			cboPotions.DisplayMember = "Name";
			cboPotions.ValueMember = "Id";

			_player.PropertyChanged += PlayerOnPropertyChanged;
			_player.OnMessage += DisplayMessage;
		}

		private void btnNorth_Click(object sender, EventArgs e)
		{
			_player.MoveNorth();
		}

		private void btnEast_Click(object sender, EventArgs e)
		{
			_player.MoveEast();
		}

		private void btnSouth_Click(object sender, EventArgs e)
		{
			_player.MoveSouth();
		}

		private void btnWest_Click(object sender, EventArgs e)
		{
			_player.MoveWest();
		}

		private void btnUseWeapon_Click(object sender, EventArgs e)
		{
			//get currently selected weapon from combobox
			Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

			_player.UseWeapon(currentWeapon);

			//damage enemy with selected weapon
			//AttackEnemy();

			//enemy is defeated
			//if(_currentEnemy.CurrentHP <= 0)
			//{
				//dies and gives loot
			//	EnemyDefeated();
			//}
			//enemy survives
			//else
			//{
				//then attacks
			//	EnemyAttacks();
			//}
		}

		private void btnUsePotion_Click(object sender, EventArgs e)
		{
			//get currently selected potion from combobox
			HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

			_player.UsePotion(potion);

			//use selected potion
			//PotionUse();

			//monster attacks
			//EnemyAttacks();
		}

		private void btnTrade_Click(object sender, EventArgs e)
		{
			TradingScreen tradingScreen = new TradingScreen(_player);
			tradingScreen.StartPosition = FormStartPosition.CenterParent;
			tradingScreen.ShowDialog(this);
		}

		private void adventureGame_FormClosing(object sender, FormClosingEventArgs e)
		{
			File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
		}

		private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
		{
			_player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
		}

		private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == "Weapons")
			{
				cboWeapons.DataSource = _player.Weapons;

				if (!_player.Weapons.Any())
				{
					cboWeapons.Visible = false;
					btnUseWeapon.Visible = false;
				}
			}

			if(propertyChangedEventArgs.PropertyName == "Potions")
			{
				cboPotions.DataSource = _player.Potions;

				if(!_player.Potions.Any())
				{
					cboPotions.Visible = false;
					btnUsePotion.Visible = false;
				}
			}

			if(propertyChangedEventArgs.PropertyName == "CurrentLocation")
			{
				//show/hide available movement/trade buttons
				btnNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
				btnEast.Visible = (_player.CurrentLocation.LocationToEast != null);
				btnSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
				btnWest.Visible = (_player.CurrentLocation.LocationToWest != null);
				btnTrade.Visible = (_player.CurrentLocation.VendorWorkingHere != null);

				//display current location name and description
				rtbLocation.Text = _player.CurrentLocation.Name + Environment.NewLine + Environment.NewLine;
				rtbLocation.Text += _player.CurrentLocation.Description + Environment.NewLine;

				if(_player.CurrentLocation.EnemyLivingHere == null)
				{
					cboWeapons.Visible = false;
					cboPotions.Visible = false;
					btnUseWeapon.Visible = false;
					btnUsePotion.Visible = false;
				}
				else
				{
					cboWeapons.Visible = _player.Weapons.Any();
					cboPotions.Visible = _player.Potions.Any();
					btnUseWeapon.Visible = _player.Weapons.Any();
					btnUsePotion.Visible = _player.Potions.Any();
				}
			}
		}

		private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
		{
			rtbMessages.Text += messageEventArgs.Message + Environment.NewLine;

			if (messageEventArgs.AddExtraNewLine)
			{
				rtbMessages.Text += Environment.NewLine;
			}

			rtbMessages.SelectionStart = rtbMessages.Text.Length;
			rtbMessages.ScrollToCaret();
		}
	}
}
