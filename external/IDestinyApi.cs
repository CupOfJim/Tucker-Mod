using Nickel;

namespace TuckerTheSaboteur;

public interface IDestinyApi
{
	IDeckEntry DestinyDeck { get; }
	
	IStatusEntry MagicFindStatus { get; }
	IStatusEntry PristineShieldStatus { get; }
	
	ICardTraitEntry EnchantedTrait { get; }
	ICardTraitEntry ExplosiveTrait { get; }

	Spr? GetEnchantedCardArt(Card? card, Spr? defaultArt = null, int[]? split = null);
	int GetMaxEnchantLevel(string cardKey, Upgrade upgrade);
	int GetEnchantLevel(Card card);
	void SetEnchantLevel(Card card, int level);
}