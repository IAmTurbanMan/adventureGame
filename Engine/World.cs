using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public static class World
	{
		public static readonly List<Item> Items = new List<Item>();
		public static readonly List<Enemy> Enemies = new List<Enemy>();
		public static readonly List<Quest> Quests = new List<Quest>();
		public static readonly List<Location> Locations = new List<Location>();

		public const int ITEM_ID_RUSTY_SWORD = 1;
		public const int ITEM_ID_RAT_TAIL = 2;
		public const int ITEM_ID_PIECE_OF_FUR = 3;
		public const int ITEM_ID_SNAKE_FANG = 4;
		public const int ITEM_ID_SNAKESKIN = 5;
		public const int ITEM_ID_CLUB = 6;
		public const int ITEM_ID_HEALING_POTION = 7;
		public const int ITEM_ID_SPIDER_FANG = 8;
		public const int ITEM_ID_SPIDER_SILK = 9;
		public const int ITEM_ID_ADVENTURER_PASS = 10;

		public const int ENEMY_ID_RAT = 1;
		public const int ENEMY_ID_SNAKE = 2;
		public const int ENEMY_ID_GIANT_SPIDER = 3;

		public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
		public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;

		public const int LOCATION_ID_HOME = 1;
		public const int LOCATION_ID_TOWN_SQUARE = 2;
		public const int LOCATION_ID_GUARD_POST = 3;
		public const int LOCATION_ID_ALCHEMIST_HUT = 4;
		public const int LOCATION_ID_ALCHEMIST_GARDEN = 5;
		public const int LOCATION_ID_FARMHOUSE = 6;
		public const int LOCATION_ID_FARM_FIELD = 7;
		public const int LOCATION_ID_BRIDGE = 8;
		public const int LOCATION_ID_SPIDER_FIELD = 9;

		static World()
		{
			PopulateItems();
			PopulateEnemies();
			PopulateQuests();
			PopulateLocations();
		}

		private static void PopulateItems()
		{
			Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rusty sword", "rusty swords", 1, 5));
			Items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat tail", "rat tails"));
			Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Piece of fur", "pieces of fur"));
			Items.Add(new Item(ITEM_ID_SNAKE_FANG, "Snake fang", "snake fangs"));
			Items.Add(new Item(ITEM_ID_SNAKESKIN, "Snake skin", "snake skins"));
			Items.Add(new Weapon(ITEM_ID_CLUB, "Club", "clubs", 3, 10));
			Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing Potion", "healing potions", 5));
			Items.Add(new Item(ITEM_ID_SPIDER_FANG, "Spider fang", "spider fangs"));
			Items.Add(new Item(ITEM_ID_SPIDER_SILK, "Spider silk", "spider silk"));
			Items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Adventurer Pass", "adventurer passes"));
		}

		private static void PopulateEnemies()
		{
			Enemy rat = new Enemy(ENEMY_ID_RAT, "Rat", 5, 3, 10, 3, 3);
			rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
			rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));

			Enemy snake = new Enemy(ENEMY_ID_SNAKE, "Snake", 5, 3, 10, 3, 3);
			snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
			snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

			Enemy giantSpider = new Enemy(ENEMY_ID_GIANT_SPIDER, "Giant spider", 20, 7, 40, 10, 10);
			giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
			giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

			Enemies.Add(rat);
			Enemies.Add(snake);
			Enemies.Add(giantSpider);
		}

		private static void PopulateQuests()
		{
			Quest clearAlchemistGarden = new Quest(QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
													"Clear the alchemist's garden",
													"Kill rats in the alchemist's garden and bring back 3 rat tails.\nYou will recieve a healing potion and 10 gold pieces.", 20, 10);

			clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));

			clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

			Quest clearFarmersField = new Quest(QUEST_ID_CLEAR_FARMERS_FIELD,
													"Clear the farmer's field",
													"Kill snakes in the farmer's field and bring back 3 snake fangs.\nYou will receive an adventurer's pass and 20 gold pieces.", 20, 20);

			clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));

			clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

			Quests.Add(clearAlchemistGarden);
			Quests.Add(clearFarmersField);
		}

		private static void PopulateLocations()
		{
			//create each location
			Location home = new Location(LOCATION_ID_HOME, "Home", "Your house. Your place is as tidy as ever.");

			Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Town square", "There is a fountain slowly trickling water out of the only spout that isn't clogged.\nThe town square is dead as usual.");

			Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Alchemist hut", "You see a few strange and glowing plants sit perilously on the shelves in the spartan hut.");
			alchemistHut.QuestAvailableHere = QuestById(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);

			Location alchemistGarden = new Location(LOCATION_ID_ALCHEMIST_GARDEN, "Alchemist garden", "You see more strange, glowing plants that almost seem to reach out to you.");
			alchemistGarden.EnemyLivingHere = EnemyById(ENEMY_ID_RAT);

			Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farmhouse", "This is a small farm with a big red barn. You can smell the fresh manure.");
			farmhouse.QuestAvailableHere = QuestById(QUEST_ID_CLEAR_FARMERS_FIELD);

			Location farmField = new Location(LOCATION_ID_FARM_FIELD, "Farmer's field", "Just past the barn, you see rows of vegetables growing. Be careful not to step in any patties.");
			farmField.EnemyLivingHere = EnemyById(ENEMY_ID_SNAKE);

			Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post", "Two small, strange looking guards stand at either side of the gate.", ItemByID(ITEM_ID_ADVENTURER_PASS));

			Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge", "A wooden bridge spans the fast flowing river. You wonder how they raised the bridge.");

			Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Forest", "You see spider webs covering the trees of this forest.");
			spiderField.EnemyLivingHere = EnemyById(ENEMY_ID_GIANT_SPIDER);

			//link locations
			home.LocationToNorth = townSquare;

			townSquare.LocationToNorth = alchemistHut;
			townSquare.LocationToEast = guardPost;
			townSquare.LocationToSouth = home;
			townSquare.LocationToWest = farmhouse;

			farmhouse.LocationToEast = townSquare;
			farmhouse.LocationToWest = farmField;

			farmField.LocationToEast = farmhouse;

			alchemistHut.LocationToSouth = townSquare;
			alchemistHut.LocationToNorth = alchemistGarden;

			alchemistGarden.LocationToSouth = alchemistHut;

			guardPost.LocationToEast = bridge;
			guardPost.LocationToWest = townSquare;

			bridge.LocationToWest = guardPost;
			bridge.LocationToEast = spiderField;

			spiderField.LocationToWest = bridge;

			//add locations to static list
			Locations.Add(home);
			Locations.Add(townSquare);
			Locations.Add(guardPost);
			Locations.Add(alchemistHut);
			Locations.Add(alchemistGarden);
			Locations.Add(farmhouse);
			Locations.Add(farmField);
			Locations.Add(bridge);
			Locations.Add(spiderField);
		}

		public static Item ItemByID(int id)
		{
			foreach(Item item in Items)
			{
				if (item.ID == id)
				{
					return item;
				}
			}
			return null;
		}

		public static Enemy EnemyById(int id)
		{
			foreach(Enemy enemy in Enemies)
			{
				if (enemy.ID == id)
				{
					return enemy;
				}
			}
			return null;
		}

		public static Quest QuestById(int id)
		{
			foreach (Quest quest in Quests)
			{
				if (quest.ID == id)
				{
					return quest;
				}
			}
			return null;
		}

		public static Location LocationById(int id)
		{
			foreach (Location location in Locations)
			{
				if (location.ID == id)
				{
					return location;
				}
			}
			return null;
		}
	}
}
