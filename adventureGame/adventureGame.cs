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

		public adventureGame()
		{
			InitializeComponent();

			Location location = new Location(1, "Home", "This is your house.");

			_player = new Player(10, 10, 20, 0, 1);

			lblHP.Text = _player.CurrentHP.ToString();
			lblGold.Text = _player.Gold.ToString();
			lblExp.Text = _player.EXP.ToString();
			lblLevel.Text = _player.Level.ToString();
		}
	}
}
