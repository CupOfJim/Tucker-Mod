using System;
using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket;

public interface IDuoArtifactsApi
{
	Deck DuoArtifactVanillaDeck { get; }

	void RegisterDuoArtifact(Type type, IEnumerable<Deck> combo);
	void RegisterDuoArtifact<TArtifact>(IEnumerable<Deck> combo) where TArtifact : Artifact;
	IReadOnlySet<Deck>? GetDuoArtifactOwnership(Artifact artifact);
}