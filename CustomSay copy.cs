namespace TuckerTheSaboteur;

internal sealed class CustomTitleCard : TitleCard
{
	public required string Text { get; set; }

	public override bool Execute(G g, IScriptTarget target, ScriptCtx ctx)
	{
		if (target is Dialogue dialogue)
		{
			dialogue.titleCard = Text;
		}
		return true;
	}
}