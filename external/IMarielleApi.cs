using Nickel;

namespace TuckerTheSaboteur;

public interface IMarielleApi
{
	IDeckEntry MarielleDeck();

	IStatusEntry Curse();

	IStatusEntry Enflamed();

	ICardTraitEntry Fleeting();
}
