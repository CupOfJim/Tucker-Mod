using System;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;

namespace TuckerTheSaboteur;

public static class Extensions {
    public static AAttack ApplyOffset(this AAttack attack, State s, int offset) {
		if (offset == 0) return attack;
        // handle attack offset
        // int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

        // // return final result
        // attack.fromX = cannonX + offset;
        Main.Instance.Helper.ModData.SetModData(attack, OffsetAttackController.key, offset);
        return attack;
    }

	private static void WarnOnDebugAssembly(ILogger logger, Assembly? assembly)
	{
		if (assembly?.IsBuiltInDebugConfiguration() == true)
			logger.LogWarning("{Assembly} was built in debug configuration - patching may fail. If it does fail, please ask that mod's developer to build it in release configuration.", assembly.GetName().Name);
	}

	public static bool TryPatch(
		this Harmony self,
		MethodInfo? original,
		ILogger logger,
		LogLevel problemLogLevel = LogLevel.Error,
		LogLevel successLogLevel = LogLevel.Trace,
		HarmonyMethod? prefix = null,
		HarmonyMethod? postfix = null,
		HarmonyMethod? transpiler = null,
		HarmonyMethod? finalizer = null,
		string message = ""
	)
	{
		var originalMethod = original;
		if (originalMethod == null)
		{
			logger.Log(problemLogLevel, "Could not patch method - the mod may not work correctly.\nReason: Unknown method to patch ({0}).", message);
#if DEBUG
			Debugger.Break();
#endif
			return false;
		}

		try
		{

			if (transpiler is not null)
				WarnOnDebugAssembly(logger, originalMethod.DeclaringType?.Assembly);
			self.Patch(originalMethod, prefix, postfix, transpiler, finalizer);
			logger.Log(successLogLevel, "Patched method {Method}.", originalMethod.FullDescription());
			return true;
		}
		catch (Exception ex)
		{
			logger.Log(problemLogLevel, "Could not patch method {Method} - the mod may not work correctly.\nReason: {Exception}", originalMethod, ex);
#if DEBUG
			Debugger.Break();
#endif
			return false;
		}
	}
}