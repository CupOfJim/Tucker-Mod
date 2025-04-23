using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur;

public class ApiImplementation : ITuckerApi
{
	public AAttack MakeNewBluntAttack() {
		return new ABluntAttack();
	}
	public bool IsBluntAttack(AAttack attack) {
		return attack is ABluntAttack;
	}
	public AAttack ApplyOffset(AAttack attack, int offset) {
		return attack.ApplyOffset(offset);
	}
	public int? GetOffset(AAttack attack) {
		return attack.GetOffset();
	}

	public Deck TuckerDeck => Main.Instance.TuckerDeck.Deck;
	public Status BufferStatus => Main.Instance.BufferStatus.Status;
	public Status FuelLeakStatus => Main.Instance.FuelLeakStatus.Status;
}
