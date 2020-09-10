﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Quest
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int RewardEXP { get; set; }
		public int RewardGold { get; set; }
		public Item RewardItem { get; set; }
		public List<QuestCompletionItem> QuestCompletionItems { get; set; }

	public Quest(int id, string name, string description, int rewardexp, int rewardgold)
		{
			ID = id;
			Name = name;
			Description = description;
			RewardEXP = rewardexp;
			RewardGold = rewardgold;

			QuestCompletionItems = new List<QuestCompletionItem>();
		}
	}
}
