using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace ExtremeTempDrop
{
    public class Implementation : MelonMod
    {
        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
            Settings.OnLoad();
        }
    }

    [HarmonyPatch(typeof(ExperienceModeManager), "GetOutdoorTempDropCelcius")]
    public class ExperienceModeManager_GetOutdoorTempDropCelcius_Post
    {
        private static void Postfix(ref float __result, float numDays)
        {
            if (numDays <= Settings.options.declineStartDay) __result = Settings.options.initialDrop;
            else if (numDays >= Settings.options.declineEndDay) __result = Settings.options.maxDrop;
            else
            {
                int startDay = Settings.options.declineStartDay;
                int endDay = Settings.options.declineEndDay;
                float startDrop = Settings.options.initialDrop;
                float endDrop = Settings.options.maxDrop;
                __result = (numDays - startDay) / (endDay - startDay) * (endDrop - startDrop) + startDrop;
            }
        }
    }
}
