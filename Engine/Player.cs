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

		public Player(int currenthp, int maxhp, int gold, int exp, int level) : base(currenthp, maxhp)
		{
			Gold = gold;
			EXP = exp;
			Level = level;

			Inventory = new List<InventoryItem>();
			Quests = new List<PlayerQuest>();
		}

	}
}
