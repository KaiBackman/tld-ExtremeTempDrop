using Il2Cpp;
using HarmonyLib;
using MelonLoader;

namespace ExtremeTempDrop
{
    public class Implementation : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"[{Info.Name}] Version {Info.Version} loaded!");
            Settings.instance.AddToModSettings("Extreme Temp Drop");
        }
    }

    [HarmonyPatch(typeof(ExperienceModeManager), nameof(ExperienceModeManager.GetOutdoorTempDropCelcius))]
    public class ExperienceModeManager_GetOutdoorTempDropCelcius
    {
        private static void Postfix(ref float __result, float numDays)
        {
            if (numDays <= Settings.instance.declineStartDay) __result = Settings.instance.initialDrop;
            else if (numDays >= Settings.instance.declineEndDay) __result = Settings.instance.maxDrop;
            else
            {
                int startDay = Settings.instance.declineStartDay;
                int endDay = Settings.instance.declineEndDay;
                float startDrop = Settings.instance.initialDrop;
                float endDrop = Settings.instance.maxDrop;
                __result = (numDays - startDay) / (endDay - startDay) * (endDrop - startDrop) + startDrop;
            }
        }
    }
}
