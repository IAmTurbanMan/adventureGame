using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Engine
{
	public class Enemy : Creature
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int MaxDamage { get; set; }
		public int RewardEXP { get; set; }
		public int RewardGold { get; set; }
		public List<LootItem> LootTable { get; set; }

		public Enemy(int id, string name, int maxdamage, int rewardexp, int rewardgold, int currenthp, int maxhp) : base(currenthp, maxhp)
		{
			ID = id;
			Name = name;
			MaxDamage = maxdamage;
			RewardEXP = rewardexp;
			RewardGold = rewardgold;

			LootTable = new List<LootItem>();
		}
	}
}
