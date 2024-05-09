using C3.ModKit;
using HarmonyLib;
using Unfoundry;
using UnityEngine;

namespace FastResearch
{
    [UnfoundryMod(GUID)]
    public class Plugin : UnfoundryPlugin
    {
        public const string
            MODNAME = "FastResearch",
            AUTHOR = "erkle64",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "0.1.0";

        public static LogSource log;

        public static TypedConfigEntry<float> speedMultiplier;

        public Plugin()
        {
            log = new LogSource(MODNAME);

            new Config(GUID)
                .Group("Multipliers")
                    .Entry(out speedMultiplier, "speedMultiplier", 4.0f, true, "Research speed multiplication factor.")
                .EndGroup()
                .Load()
                .Save();
        }

        public override void Load(Mod mod)
        {
            log.Log($"Loading {MODNAME}");
        }

        [HarmonyPatch]
        public static class Patch
        {
            [HarmonyPatch(typeof(ResearchTemplate), nameof(ResearchTemplate.onLoad))]
            [HarmonyPostfix]
            public static void ResearchTemplateOnLoad(ResearchTemplate __instance)
            {
                var speedMultiplier = Plugin.speedMultiplier.Get();
                if (speedMultiplier > 0.0f && speedMultiplier != 1.0f)
                {
                    var newSecondsPerScienceItem = (uint)Mathf.CeilToInt(__instance.secondsPerScienceItem / speedMultiplier);
                    log.LogFormat("Fastinating {0} from {1} to {2}", __instance.identifier, __instance.secondsPerScienceItem, newSecondsPerScienceItem);
                    __instance.secondsPerScienceItem = newSecondsPerScienceItem;
                }
            }
        }
    }
}
