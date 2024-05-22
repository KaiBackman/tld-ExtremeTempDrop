using Il2Cpp;
using HarmonyLib;
using MelonLoader;
using Il2CppTLD.Gameplay;
using UnityEngine;

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
    public class Common
    {
        // return positive number of how many deg temp should be reduced
        // this will keep in logic of GetOutdoorTempDropCelcius()
        public float GetTempDropC()
        {

            float tempOffset = 0;
            float numDays = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() / 24f;
            ExperienceMode expMode = GameManager.m_ExperienceModeManager.GetCurrentExperienceMode();

            int num = expMode.m_OutdoorTempDropDayFinal - expMode.m_OutdoorTempDropDayStart;
            // revert vanilla chnages
            if (numDays >= expMode.m_OutdoorTempDropDayFinal) tempOffset -= expMode.m_OutdoorTempDropCelsiusMax;
            else if (numDays < expMode.m_OutdoorTempDropDayFinal && numDays >= expMode.m_OutdoorTempDropDayStart && num > 0) tempOffset -= Mathf.Lerp(0f, expMode.m_OutdoorTempDropCelsiusMax, (numDays - expMode.m_OutdoorTempDropDayStart) / num);

            if (numDays <= Settings.instance.declineStartDay) tempOffset += Settings.instance.initialDrop;
            else if (numDays >= Settings.instance.declineEndDay) tempOffset += Settings.instance.maxDrop;
            else
            {
                int startDay = Settings.instance.declineStartDay;
                int endDay = Settings.instance.declineEndDay;
                float startDrop = Settings.instance.initialDrop;
                float endDrop = Settings.instance.maxDrop;
                tempOffset += (numDays - startDay) / (endDay - startDay) * (endDrop - startDrop) + startDrop;
            }
            return tempOffset;
        }
    }
    [HarmonyPatch(typeof(Weather), nameof(Weather.CalculateCurrentTemperature))]
    internal class Weather_CalculateCurrentTemperature
    {
        public static void Postfix(Weather __instance)
        {
            if (GameManager.m_IsPaused)
            {
                return;
            }
            if (!InterfaceManager.IsMainMenuEnabled() && !__instance.IsIndoorEnvironment())
            {
                //MelonLogger.Msg("Extreme temp (pre):" + __instance.m_CurrentTemperature);
                Common p = new Common();
                __instance.m_CurrentTemperature -= p.GetTempDropC();
                //MelonLogger.Msg("Extreme temp:" + __instance.m_CurrentTemperature);
            }
        }
    }

}
