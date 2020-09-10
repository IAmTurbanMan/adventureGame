using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Weapon : Item
	{
		public int MinDamage { get; set; }
		public int MaxDamage { get; set; }

		public Weapon(int id, string name, string nameplural, int mindamage, int maxdamage) : base(id, name, nameplural)
		{
			MinDamage = mindamage;
			MaxDamage = maxdamage;
		}
	}
}
