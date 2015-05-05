/*Smarbot Profile for tempowaker mage
* Deck from : http://www.hearthpwn.com/decks/240196-fast-legend-mage
* Contributors : Wirmate
*/
using System.Linq;

namespace SmartBot.Plugins.API
{
	public class bProfile : RemoteProfile
	{
		private int FriendCardDrawValue = 5;
		private int EnemyCardDrawValue = 2;

		private int MinionEnemyTauntValue = 6;
		
		private int MinionDivineShield = 4;

		private int HeroEnemyHealthValue = 4;
		private int HeroFriendHealthValue = 2;

		private int MinionEnemyAttackValue = 2;
		private int MinionEnemyHealthValue = 2;
		private int MinionFriendAttackValue = 4;
		private int MinionFriendHealthValue = 2;

		//Spells cast cost
		private int SpellsCastGlobalCost = 0;
		//Spells cast value
		private int SpellsCastGlobalValue = 0;

		//Weapons cast cost
		private int WeaponCastGlobalCost = 0;
		//Weapons cast value
		private int WeaponCastGlobalValue = 0;

		//Minions cast cost
		private int MinionCastGlobalCost = 0;
		//Minions cast value
		private int MinionCastGlobalValue = 0;

		//HeroPowerCost
		private int HeroPowerGlobalCost = 0;

		//Weapons Attack cost
		private int WeaponAttackGlobalCost = 0;

		//GlobalValueModifier
		private int GlobalValueModifier = 0;

		public override float GetBoardValue(Board board)
		{
			float value = 0;

			//Hero friend value
			value += board.HeroFriend.CurrentHealth * HeroFriendHealthValue + board.HeroFriend.CurrentArmor * HeroFriendHealthValue;

			//Hero enemy value
			value -= board.HeroEnemy.CurrentHealth * HeroEnemyHealthValue + board.HeroEnemy.CurrentArmor * HeroEnemyHealthValue;

			//enemy board
			foreach(Card c in board.MinionEnemy)
			{
				value -= GetCardValue(board,c);
			}
			
			//friend board
			foreach(Card c in board.MinionFriend)
			{
				value += GetCardValue(board,c);
			}
			
			//casting costs
			value -= MinionCastGlobalCost;
			value -= SpellsCastGlobalCost;
			value -= WeaponCastGlobalCost;

			//casting action value
			value += WeaponCastGlobalValue;
			value += SpellsCastGlobalValue;
			value += MinionCastGlobalValue;

			//heropower vost
			value -= HeroPowerGlobalCost;

			//Weapon attack cost
			value -= WeaponAttackGlobalCost;

			if (board.HeroEnemy.CurrentHealth <= 0)
				value += 10000;

			if (board.HeroFriend.CurrentHealth <= 0 && board.FriendCardDraw == 0) 
				value -= 100000;

			value += GlobalValueModifier;

			value += board.FriendCardDraw * FriendCardDrawValue;
			value -= board.EnemyCardDraw * EnemyCardDrawValue;

			return value;
		}
		
		public float GetCardValue(Board board,Card card)
		{
			float value = 0;
			
			//divine shield value
			if(card.IsDivineShield)
				value +=  MinionDivineShield;
					
			if(card.IsFriend)
			{
				value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;
			}
			else
			{
				value += GetThreatModifier(card);
				//Taunt value
				if(card.IsTaunt)
					value += MinionEnemyTauntValue;
				value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;
			}
			
			return value;
		}

		public override void OnCastMinion(Board board, Card minion, Card target)
		{
			switch (minion.Template.Id)
			{
				case Card.Cards.FP1_004://Mad Scientist
					if (board.TurnCount <= 2)
						MinionCastGlobalValue += 10;
					break;
				case Card.Cards.NEW1_012://Mana Wyrm
					break;
				case Card.Cards.EX1_608://Sorcerer's Apprentice
					MinionCastGlobalCost += 10;
					break;
				case Card.Cards.BRM_002://Flamewaker
					if(board.TurnCount < 4)
						MinionCastGlobalCost += 16;
					else
						MinionCastGlobalCost += 7;
					break;
				case Card.Cards.EX1_096://Loot Hoarder
					break;
				case Card.Cards.EX1_284://Azure Drake
					break;
				case Card.Cards.EX1_559://Archmage Antonidas
					if(board.SecretEnemy) MinionCastGlobalCost += 150;
					MinionCastGlobalCost += 30;
					break;
				case Card.Cards.GVG_082://Clockwork Gnome
					break;
				case Card.Cards.NEW1_019://Knife Juggler
					MinionCastGlobalCost += 8; 
					break;
				case Card.Cards.FP1_030://Loatheb
					break;
				case Card.Cards.GVG_110://Dr. Boom
					break;
			}
		}

		public override void OnCastSpell(Board board, Card spell, Card target)
		{
			if(board.MinionFriend.Any(x => x.Template.Id == Card.Cards.EX1_559))
				SpellsCastGlobalValue += 50;
			
			switch (spell.Template.Id)
			{
				case Card.Cards.EX1_277://Arcane Missiles
					SpellsCastGlobalCost += 16;
					break;
				case Card.Cards.CS2_027://Mirror Image
					SpellsCastGlobalCost += 10;
					break;
				case Card.Cards.GVG_001://Flamecannon
					SpellsCastGlobalCost += 14;
					break;
				case Card.Cards.CS2_024://Frostbolt
					SpellsCastGlobalCost += 17;
					break;
				case Card.Cards.GVG_003://Unstable Portal
					SpellsCastGlobalValue += 5;
					break;
				case Card.Cards.CS2_023://Arcane Intellect
					SpellsCastGlobalCost += 6;
					break;
				case Card.Cards.EX1_287://Counterspell
					SpellsCastGlobalValue += 5;
					break;
				case Card.Cards.EX1_294://Mirror Entity
					SpellsCastGlobalValue += 5;
					break;
				case Card.Cards.GVG_005://Echo of Medivh
					SpellsCastGlobalCost += 12;
					SpellsCastGlobalValue += board.MinionFriend.Count * 5;
					if(board.MinionFriend.Any(x => x.Template.Id == Card.Cards.BRM_002))SpellsCastGlobalValue += 5;
					break;
				case Card.Cards.CS2_029://Fireball
					SpellsCastGlobalCost += 25;
					break;
				case Card.Cards.CS2_022://Polymorph
					SpellsCastGlobalCost += 14;
					break;
				case Card.Cards.GAME_005://The Coin
					SpellsCastGlobalCost += 4;
					break;
			}
		}

		public override void OnCastWeapon(Board board, Card weapon, Card target)
		{
			switch (weapon.Template.Id)
			{

			}
		}

		public override void OnAttack(Board board, Card attacker, Card target)
		{
			bool IsAttackingWithHero = (attacker.Id == board.HeroFriend.Id);
			bool IsAttackingWithWeapon = (board.WeaponFriend != null && attacker.Id == board.WeaponFriend.Id);

			if ((IsAttackingWithHero || IsAttackingWithWeapon) && board.WeaponFriend != null)//If we attack with weapon equipped
			{
				switch (board.WeaponFriend.Template.Id)
				{

				}
			}

			if (!IsAttackingWithHero && !IsAttackingWithWeapon)
			{
				if (target != null && target.CurrentAtk >= attacker.CurrentHealth && !attacker.IsDivineShield)
					OnMinionDeath(board, attacker);
			}
		}
		public override void OnCastAbility(Board board, Card ability, Card target)
		{
			if(board.TurnCount < 2) HeroPowerGlobalCost += 10;
			HeroPowerGlobalCost += 2;
		}

		public override RemoteProfile DeepClone()
		{
			bProfile ret = new bProfile();
			ret.HeroEnemyHealthValue = HeroEnemyHealthValue;
			ret.HeroFriendHealthValue = HeroFriendHealthValue;
			ret.MinionEnemyAttackValue = MinionEnemyAttackValue;
			ret.MinionEnemyHealthValue = MinionEnemyHealthValue;
			ret.MinionFriendAttackValue = MinionFriendAttackValue;
			ret.MinionFriendHealthValue = MinionFriendHealthValue;

			ret.SpellsCastGlobalCost = SpellsCastGlobalCost;
			ret.SpellsCastGlobalValue = SpellsCastGlobalValue;
			ret.WeaponCastGlobalCost = WeaponCastGlobalCost;
			ret.WeaponCastGlobalValue = WeaponCastGlobalValue;
			ret.MinionCastGlobalCost = MinionCastGlobalCost;
			ret.MinionCastGlobalValue = MinionCastGlobalValue;

			ret.HeroPowerGlobalCost = HeroPowerGlobalCost;
			ret.WeaponAttackGlobalCost = WeaponAttackGlobalCost;

			ret.GlobalValueModifier = GlobalValueModifier;

			return ret;
		}

		public int GetCoinValue(Board board)
		{
			int currentMana = board.MaxMana;
			bool HasDropCurrentTurn = (board.GetPlayables(currentMana, currentMana).Count != 0);
			bool HasDropNextTurn = (board.GetPlayables(currentMana + 1, currentMana + 1).Count != 0);

			if (HasDropCurrentTurn)
			{
				if (board.MaxMana == 1 && board.GetPlayables(1, 1).Count == 2)
					return 0;
				return 8;
			}

			if (!HasDropCurrentTurn && HasDropNextTurn)
			{
				bool CanPlayOnCurve = ((board.GetPlayables(currentMana + 1, currentMana + 1).Count > 1) && (board.GetPlayables(currentMana + 2, currentMana + 2).Count >= 1));

				if (CanPlayOnCurve)
				{
					return 0;
				}
				return 8;
			}

			return 5;
		}

		public void OnMinionDeath(Board board, Card minion)
		{
			switch (minion.Template.Id)
			{
				
			}
		}
		
		public float GetThreatModifier(Card card)
		{
			switch (card.Template.Id)
			{
				case Card.Cards.GVG_006://Mechwarper
					return 4;
					
				case Card.Cards.FP1_013://Kel'Thuzad
					return 6;
					
				case Card.Cards.EX1_016://Sylvanas Windrunner
					return 5;
					
				case Card.Cards.GVG_105://Piloted Sky Golem
					return 3;
					
				case Card.Cards.BRM_031://Chromaggus
					return 5;
					
				case Card.Cards.EX1_559://Archmage Antonidas
					return 8;
					
				case Card.Cards.GVG_021://Mal'Ganis
					return 6;
					
				case Card.Cards.EX1_608://Sorcerer's Apprentice
					return 3;
					
				case Card.Cards.NEW1_012://Mana Wyrm
					return 3;
					
				case Card.Cards.BRM_002://Flamewaker
					return 4;
					
				case Card.Cards.EX1_595://Cult Master
					return 2;
					
				case Card.Cards.NEW1_021://Doomsayer
					return 0;
					
				case Card.Cards.EX1_243://Dust Devil
					return 2;
					
				case Card.Cards.EX1_170://Emperor Cobra
					return 4;
					
				case Card.Cards.BRM_028://Emperor Thaurissan
					return 6;
					
				case Card.Cards.EX1_565://Flametongue Totem
					return 5;
					
				case Card.Cards.GVG_100://Floating Watcher
					return 4;
					
				case Card.Cards.GVG_113://Foe Reaper 4000
					return 0;
					
				case Card.Cards.tt_004://Flesheating Ghoul
					return 2;
					
				case Card.Cards.EX1_604://Frothing Berserker
					return 3;
					
				case Card.Cards.BRM_019://Grim Patron
					return 7;
					
				case Card.Cards.EX1_084://Warsong Commander
					return 7;
					
				case Card.Cards.EX1_095://Gadgetzan Auctioneer
					return 4;
					
				case Card.Cards.NEW1_040://Hogger
					return 3;
					
				case Card.Cards.GVG_104://Hobgoblin
					return 5;
					
				case Card.Cards.EX1_614://Illidan Stormrage
					return 4;
					
				case Card.Cards.GVG_027://Iron Sensei
					return 5;
					
				case Card.Cards.GVG_094://Jeeves
					return 5;
				
				case Card.Cards.NEW1_019://Knife Juggler
					return 4;
					
				case Card.Cards.EX1_001://Lightwarden
					return 4;
					
				case Card.Cards.EX1_563://Malygos
					return 6;
					
				case Card.Cards.GVG_103://Micro Machine
					return 3;
					
				case Card.Cards.EX1_044://Questing Adventurer
					return 3;
					
				case Card.Cards.EX1_298://Ragnaros the Firelord
					return 6;
					
				case Card.Cards.GVG_037://Whirling Zap-o-matic
					return 4;
					
				case Card.Cards.NEW1_020://Wild Pyromancer
					return 5;
			}
			return 0;
		}
	}
}