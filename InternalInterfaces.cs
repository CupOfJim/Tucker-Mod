using Nickel;

namespace TuckerTheSaboteur;

internal interface IRegisterableCard
{
	static abstract void Register(IModHelper helper);
}

internal interface IRegisterableArtifact
{
	static abstract void Register(IModHelper helper);
}