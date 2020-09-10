using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Location
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Item ItemRequiredToEnter { get; set; }
		public Quest QuestAvailableHere { get; set; }
		public Enemy EnemyLivingHere { get; set; }
		public Location LoacationToNorth { get; set; }
		public Location LoacationToEast { get; set; }
		public Location LoacationToSouth { get; set; }
		public Location LoacationToWest { get; set; }

		public Location(int id, string name, string description, Item itemRequiredToEnter = null, Quest questAvailableHere = null, Enemy enemyLivingHere = null)
		{
			ID = id;
			Name = name;
			Description = description;
			ItemRequiredToEnter = itemRequiredToEnter;
			QuestAvailableHere = questAvailableHere;
			EnemyLivingHere = enemyLivingHere;
		}
	}
}
