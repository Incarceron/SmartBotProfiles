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
					return 5;

		case Card.Cards.NEW1_012://Mana Wyrm
					return 5;

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