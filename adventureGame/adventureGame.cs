using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace adventureGame
{
	public partial class adventureGame : Form
	{
		public adventureGame()
		{
			InitializeComponent();
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
			lblGold.Text = "123";
		}
	}
}
