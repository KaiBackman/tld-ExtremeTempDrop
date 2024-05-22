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
                float numDays = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() / 24f;
                ExperienceMode p = GameManager.m_ExperienceModeManager.GetCurrentExperienceMode();
                int num = p.m_OutdoorTempDropDayFinal - p.m_OutdoorTempDropDayStart;
                // revert vanilla chnages
                //MelonLogger.Msg("Extreme temp vanilla:" + __instance.m_CurrentTemperature);
                if (numDays >= p.m_OutdoorTempDropDayFinal) __instance.m_CurrentTemperature += p.m_OutdoorTempDropCelsiusMax;
                else if (numDays < p.m_OutdoorTempDropDayFinal && numDays >= p.m_OutdoorTempDropDayStart && num > 0) __instance.m_CurrentTemperature += Mathf.Lerp(0f, p.m_OutdoorTempDropCelsiusMax, (numDays - p.m_OutdoorTempDropDayStart) / num);

                if (numDays <= Settings.instance.declineStartDay) __instance.m_CurrentTemperature -= Settings.instance.initialDrop;
                else if (numDays >= Settings.instance.declineEndDay) __instance.m_CurrentTemperature -= Settings.instance.maxDrop;
                else
                {
                    int startDay = Settings.instance.declineStartDay;
                    int endDay = Settings.instance.declineEndDay;
                    float startDrop = Settings.instance.initialDrop;
                    float endDrop = Settings.instance.maxDrop;
                    __instance.m_CurrentTemperature -= (numDays - startDay) / (endDay - startDay) * (endDrop - startDrop) + startDrop;
                }
                //MelonLogger.Msg("Extreme temp:" + __instance.m_CurrentTemperature);
            }
        }
    }
}
