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
	public partial class TradingScreen : Form
	{
		private Player _currentPlayer;
		public TradingScreen(Player player)
		{
			_currentPlayer = player;
			InitializeComponent();

			//style, to display numeric column values
			DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
			rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			//populate datagrid for player's inventory
			dgvMyItems.RowHeadersVisible = false;
			dgvMyItems.AutoGenerateColumns = false;

			//hidden column holds itemID so we know which item to sell
			dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = "ItemID",
				Visible = false
			});

			dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Name",
				Width = 100,
				DataPropertyName = "Description"
			});

			dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Qty",
				Width = 30,
				DefaultCellStyle = rightAlignedCellStyle,
				DataPropertyName = "Quantity"
			});

			dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Price",
				Width = 35,
				DefaultCellStyle = rightAlignedCellStyle,
				DataPropertyName = "Price"
			});

			dgvMyItems.Columns.Add(new DataGridViewButtonColumn
			{
				Text = "Sell 1",
				UseColumnTextForButtonValue = true,
				Width = 50,
				DataPropertyName = "ItemID"
			});

			//bind player's inventory to datagridview
			dgvMyItems.DataSource = _currentPlayer.Inventory;

			//when user clicks on a row, call function
			dgvMyItems.CellClick += dgvMyItems_CellClick;

			//populate vendor inventory datagrid
			dgvVendorItems.RowHeadersVisible = false;
			dgvVendorItems.AutoGenerateColumns = false;

			//hidden ID column
			dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = "ItemID",
				Visible = false
			});

			dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Name",
				Width = 100,
				DataPropertyName = "Description"
			});

			dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = "Price",
				Width = 35,
				DefaultCellStyle = rightAlignedCellStyle,
				DataPropertyName = "Price"
			});

			dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
			{
				Text = "Buy 1",
				UseColumnTextForButtonValue = true,
				Width = 50,
				DataPropertyName = "ItemID"
			});

			//bind vendor inventory with datagridview
			dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

			//when user clicks on a row, call function
			dgvVendorItems.CellClick += dgvVendorItems_CellClick;
		}

		private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			//if player clicked button column, sell item from that row
			if(e.ColumnIndex == 4)
			{
				//get ID value from row 0 (hidden)
				var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

				//get item object for selected row
				Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));

				if(itemBeingSold.Price == World.UNSELLABLE_ITEM)
				{
					MessageBox.Show("You cannot sell the " + itemBeingSold.Name);
				}
				else
				{
					//remove one from player inventory
					_currentPlayer.RemoveItemFromInventory(itemBeingSold);

					//give player gold according to price of item sold
					_currentPlayer.Gold += itemBeingSold.Price;
				}
			}
		}

		private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if(e.ColumnIndex == 3)
			{
				//get ID of item
				var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;

				//get Item object for selected row
				Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));

				//check if player has enough gold
				if(_currentPlayer.Gold >= itemBeingBought.Price)
				{
					//add one to player's inventory
					_currentPlayer.AddItemToInventory(itemBeingBought);

					//remove gold from player
					_currentPlayer.Gold -= itemBeingBought.Price;
				}
				else
				{
					MessageBox.Show("You do not have enough gold to buy the " + itemBeingBought.Name);
				}
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
