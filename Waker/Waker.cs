/*Smarbot Profile for tempowaker mage
* Deck from : http://www.hearthpwn.com/decks/240196-fast-legend-mage
* Contributors : Wirmate, Botfanatic
*/
using System.Linq;
using System.Collections.Generic;

namespace SmartBot.Plugins.API
{
	public class bProfile : RemoteProfile
	{
		private int MinionEnemyTauntValue = 10;
		private int MinionEnemyWindfuryValue = 5;
		private int MinionDivineShield = 4;

		private int HeroEnemyHealthValue = 4;
		private int HeroFriendHealthValue = 2;

		private int FriendCardDrawValue = 5;
		private int EnemyCardDrawValue = 2;

		private int MinionEnemyAttackValue = 2;
		private int MinionEnemyHealthValue = 2;
		private int MinionFriendAttackValue = 2;
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

		//Secret Modifier
		private int SecretModifier = 0;

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
				value -= GetCardValue(board, c);
			}

			//friend board
			foreach(Card c in board.MinionFriend)
			{
				value += GetCardValue(board, c);
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

			value += SecretModifier;

			return value;
		}

		public float GetCardValue(Board board, Card card)
		{
			float value = 0;

			//divine shield value
			if(card.IsDivineShield)
				value += MinionDivineShield;

			if(card.IsFriend)
			{
				value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

				if(card.IsFrozen)
					value -= 2;
			}
			else
			{
				value += GetThreatModifier(card);
				//Taunt value
				if(card.IsTaunt)
					value += MinionEnemyTauntValue;

				if(card.IsWindfury)
					value += MinionEnemyWindfuryValue;

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
					MinionCastGlobalCost += 8;
				break;

				case Card.Cards.BRM_002://Flamewaker
					if(board.TurnCount < 5 && board.MinionFriend.Count < 3)
					MinionCastGlobalCost += 16;
				else
					MinionCastGlobalCost += 7;
				break;

				case Card.Cards.EX1_096://Loot Hoarder
					MinionCastGlobalValue += 2;
				break;

				case Card.Cards.CS2_203://Ironbeak Owl
					MinionCastGlobalCost += 12;
				break;

				case Card.Cards.EX1_284://Azure Drake
					break;

				case Card.Cards.EX1_559://Archmage Antonidas
					if(board.SecretEnemy) MinionCastGlobalCost += 150;
				MinionCastGlobalCost += 40;
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

				case Card.Cards.EX1_012://Bloodmage Thalnos
					if(board.Hand.Count > 2)
					MinionCastGlobalCost += 5;
				break;

				case Card.Cards.GVG_094://Jeeves
					if(board.Hand.Count <= 1)	MinionCastGlobalValue += 50;
				MinionCastGlobalCost += 15;
				break;
			}

			if(BoardHelper.IsFirstMove(board))
				OnFirstAction(board, minion, target, true, false, false);
		}

		public override void OnCastSpell(Board board, Card spell, Card target)
		{
			if(board.MinionFriend.Any(x => x.Template.Id == Card.Cards.EX1_559) && board.TurnCount > 8)
				SpellsCastGlobalValue += 50;

			if(board.MinionFriend.Any(x => x.Template.Id == Card.Cards.BRM_002))
				SpellsCastGlobalValue += 5;

			if(board.Hand.Any(x => x.Template.Id == Card.Cards.BRM_002))
				SpellsCastGlobalCost += 5;

			if(BoardHelper.IsSparePart(spell) && !board.MinionFriend.Any(x => x.Template.Id == Card.Cards.EX1_559))
				SpellsCastGlobalCost += 14;

			if(board.HeroEnemy.CurrentHealth <= 10 && target != null && target.Id != board.HeroEnemy.Id)
				SpellsCastGlobalCost += 5;

			switch (spell.Template.Id)
			{
				case Card.Cards.EX1_277://Arcane Missiles
					SpellsCastGlobalCost += 16;
				SpellsCastGlobalValue += board.MinionEnemy.FindAll(x => x.CurrentHealth <= 1).Count * 3;
				break;

				case Card.Cards.CS2_027://Mirror Image
				if(!board.MinionFriend.Any(x => x.Template.Id == Card.Cards.EX1_559))
					SpellsCastGlobalCost += 10;
				break;

				case Card.Cards.GVG_001://Flamecannon
					SpellsCastGlobalCost += 9;
				break;

				case Card.Cards.CS2_024://Frostbolt
					SpellsCastGlobalCost += 14;
				break;

				case Card.Cards.GVG_003://Unstable Portal
					SpellsCastGlobalValue += 6;
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
					if(board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_029) > 0 && target.Id == board.HeroEnemy.Id)
					SpellsCastGlobalValue += 25;
				SpellsCastGlobalCost += 25;
				break;

				case Card.Cards.CS2_022://Polymorph
					SpellsCastGlobalCost += 14;
				break;

				case Card.Cards.GAME_005://The Coin
					SpellsCastGlobalCost += GetCoinValue(board);
				break;

				case Card.Cards.CS2_031://Ice Lance
					SpellsCastGlobalCost += 20;
				break;

				case Card.Cards.CS2_025://Arcane Explosion
					SpellsCastGlobalCost += 10;
				break;
			}

			if(BoardHelper.IsFirstMove(board))
				OnFirstAction(board, spell, target, true, false, false);
		}

		public override void OnCastWeapon(Board board, Card weapon, Card target)
		{
			switch (weapon.Template.Id)
			{
			}

			if(BoardHelper.IsFirstMove(board))
				OnFirstAction(board, weapon, target, false, false, false);
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

			if(BoardHelper.IsFirstMove(board))
				OnFirstAction(board, attacker, target, false, true, false);
		}

		public override void OnCastAbility(Board board, Card ability, Card target)
		{
			if(board.HeroEnemy.CurrentHealth <= 6 && target != null && target.Id == board.HeroEnemy.Id) HeroPowerGlobalCost -= 10;
			if(board.TurnCount < 2) HeroPowerGlobalCost += 10;
			HeroPowerGlobalCost += 2;

			if(BoardHelper.IsFirstMove(board))
				OnFirstAction(board, ability, target, false, false, true);
		}

		public override RemoteProfile DeepClone()
		{
			bProfile ret = new bProfile();

			ret._logBestMove.AddRange(_logBestMove);
			ret._log = _log;

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

			ret.SecretModifier = SecretModifier;

			return ret;
		}

		public void OnMinionDeath(Board board, Card minion)
		{
			switch (minion.Template.Id)
			{
			}
		}

		public void OnFirstAction(Board board, Card minion, Card target, bool castCard, bool attackCard, bool castAbility)
		{
			if (board.Hand.Any(x => x.Template.Id == Card.Cards.GVG_074))
			{
				if (minion.Template.Id == Card.Cards.GVG_074)
					SecretModifier += 100;

				return;
			}

			Card.CClass enemyclass = board.EnemyClass;
			bool lowestValueActor = false;

			if (minion != null && board.GetWorstMinionCanAttack() != null && board.GetWorstMinionCanAttack().Id == minion.Id && castCard)
				lowestValueActor = true;
			else if (minion != null && board.GetWorstMinionFromHand() != null && board.GetWorstMinionFromHand().Id == minion.Id && castCard)
				lowestValueActor = true;

			switch (enemyclass)
			{
				case Card.CClass.HUNTER:

				if (castAbility && minion.Template.Id == Card.Cards.CS1h_001 &&	target.Type == Card.CType.MINION && target.IsFriend && target.CurrentHealth <= 2 && target.MaxHealth >= 3)
					SecretModifier += 40;

				if (castCard && minion.Template.Id == Card.Cards.FP1_007)
					SecretModifier += 100;

				if (castCard && minion.Template.Id == Card.Cards.EX1_093)
				{
					if ((board.GetLeftMinion(minion) != null && board.GetLeftMinion(minion).CurrentHealth == 2) &&	(board.GetRightMinion(minion) != null && board.GetRightMinion(minion).CurrentHealth == 2))
					{
						if (BoardHelper.Get2HpMinions(board) > 1)
							SecretModifier += 100;
					}
					else if (board.GetLeftMinion(minion) != null && board.GetLeftMinion(minion).CurrentHealth == 2)
					{
						if (BoardHelper.Get2HpMinions(board) > 0)
							SecretModifier += 50;
					}
					else if (board.GetRightMinion(minion) != null && board.GetRightMinion(minion).CurrentHealth == 2)
					{
						if (BoardHelper.Get2HpMinions(board) > 0)
							SecretModifier += 50;
					}
				}

				if (attackCard)
				{
					if (lowestValueActor && !board.TrapMgr.TriggeredHeroWithMinion)
					{
						if (BoardHelper.GetWeakMinions(board) > 1 && board.MinionEnemy.Count > 0)
						{
							if (target.Type == Card.CType.HERO)
							{
								if (board.MinionEnemy.Count == 0 || BoardHelper.GetCanAttackMinions(board) == 1)
									SecretModifier += 10;
								else
									SecretModifier -= BoardHelper.GetWeakMinions(board)*3;
							}

							if (target.Type == Card.CType.MINION)
								SecretModifier += 5;
						}
					}

					if (!lowestValueActor && (!board.TrapMgr.TriggeredHeroWithMinion || !board.TrapMgr.TriggeredMinionWithMinion))
						SecretModifier -= 3;
				}
				else
					SecretModifier -= 10;

				break;

				case Card.CClass.MAGE:
					if (!board.TrapMgr.TriggeredCastMinion && lowestValueActor 	&& castCard)
				{
					SecretModifier += 50;
				}

				break;

				case Card.CClass.PALADIN:
					if (!board.TrapMgr.TriggeredHeroWithMinion && lowestValueActor
						&& minion != null && castCard
						&& target != null && target.Type == Card.CType.HERO
						&& attackCard)
				{
					SecretModifier += 10;
				}

				break;
			}

			if(castCard)
			{
				board.TrapMgr.TriggeredCastMinion = true;
			}
			else if(castAbility)
			{
			}
			else if(attackCard)
			{
				if (target!= null && target.Type == Card.CType.HERO)
					board.TrapMgr.TriggeredHeroWithMinion = true;

				if (target != null && target.Type == Card.CType.MINION)
					board.TrapMgr.TriggeredMinionWithMinion = true;
			}
		}

		public float GetThreatModifier(Card card)
		{
			switch (card.Template.Id)
			{
				case Card.Cards.GVG_006://Mechwarper
					return 7;

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
					return 8;

				case Card.Cards.NEW1_012://Mana Wyrm
					return 5;

				case Card.Cards.BRM_002://Flamewaker
					return 9;

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
					return 7;

				case Card.Cards.EX1_001://Lightwarden
					return 4;

				case Card.Cards.EX1_563://Malygos
					return 6;

				case Card.Cards.GVG_103://Micro Machine
					return 5;

				case Card.Cards.EX1_044://Questing Adventurer
					return 6;

				case Card.Cards.EX1_298://Ragnaros the Firelord
					return 7;

				case Card.Cards.GVG_037://Whirling Zap-o-matic
					return 6;

				case Card.Cards.NEW1_020://Wild Pyromancer
					return 6;

				case Card.Cards.GVG_013://Cogmaster
					return 8;
			}

			return 0;
		}

		public static int GetCoinValue(Board board)
		{
			return 4;
		}
	}

	public static class BoardHelper
	{
		public static List<Card> GetPlayables(Card.CType card_type, int min_cost, int max_cost, Board board)
		{
			return board.Hand.FindAll(x => x.Type == card_type && x.CurrentCost >= min_cost && x.CurrentCost <= max_cost).ToList();
		}

		public static List<Card> GetPlayables(int min_cost, int max_cost, Board board)
		{
			return board.Hand.FindAll(x => x.CurrentCost >= min_cost && x.CurrentCost <= max_cost).ToList();
		}

		public static bool IsSilenceCard(Card c)
		{
			switch(c.Template.Id)
			{
				case Card.Cards.EX1_332://Silence
					return true;

				case Card.Cards.EX1_166://Keeper of the Grove
					return true;

				case Card.Cards.CS2_203://Ironbeak Owl
					return true;

				case Card.Cards.EX1_048://Spellbreaker
					return true;

				case Card.Cards.EX1_245://Earth Shock
					return true;

				case Card.Cards.EX1_626://Mass Dispel
					return true;

				default:
					return false;
			}

			return false;
		}

		public static bool IsSparePart(Card c)
		{
			switch(c.Template.Id)
			{
				case Card.Cards.PART_001:
					return true;
				case Card.Cards.PART_002:
					return true;
				case Card.Cards.PART_003:
					return true;
				case Card.Cards.PART_004:
					return true;
				case Card.Cards.PART_005:
					return true;
				case Card.Cards.PART_006:
					return true;
				case Card.Cards.PART_007:
					return true;

				default:
					return false;
			}

			return false;
		}

		public static bool IsFirstMove(Board board)
		{
			return (board.SecretEnemy && board.ActionsStack.Count == 0);
		}

		public static int Get2HpMinions(Board b)
		{
			int i = 0;

			foreach (Card card in b.MinionFriend)
			{
				if (card.CurrentHealth == 2 && card.IsDivineShield == false)
					i++;
			}

			return i;
		}

		public static int GetWeakMinions(Board b)
		{
			int i = 0;

			foreach (Card card in b.MinionFriend)
			{
				if (card.CurrentHealth <= 2 && card.IsDivineShield == false)
					i++;
			}

			return i;
		}

		public static int GetCanAttackMinions(Board b)
		{
			int i = 0;

			foreach (Card card in b.MinionFriend)
			{
				if (card.CanAttack)
					i++;
			}

			return i;
		}
	}
}