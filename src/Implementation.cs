using Il2Cpp;
using HarmonyLib;
using MelonLoader;
using Il2CppTLD.Gameplay;
using UnityEngine;
using Il2CppTMPro;

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
        public static float VanillaAdjust(float numDays)
        {
            ExperienceMode expMode = GameManager.m_ExperienceModeManager.GetCurrentExperienceMode();

            int num = expMode.m_OutdoorTempDropDayFinal - expMode.m_OutdoorTempDropDayStart;
            // revert vanilla chnages
            if (numDays >= expMode.m_OutdoorTempDropDayFinal)
            {
                return -expMode.m_OutdoorTempDropCelsiusMax;
            } else if (numDays < expMode.m_OutdoorTempDropDayFinal && numDays >= expMode.m_OutdoorTempDropDayStart && num > 0)
            {
                return -Mathf.Lerp(0f, expMode.m_OutdoorTempDropCelsiusMax, (numDays - expMode.m_OutdoorTempDropDayStart) / num);
            } else
            {
                return 0f;
            }
        }

        public static float ScheduledAdjust(float numDays)
        {
            if (numDays <= Settings.instance.declineStartDay) return Settings.instance.initialDrop;
            else if (numDays >= Settings.instance.declineEndDay) return Settings.instance.maxDrop;

            int startDay = Settings.instance.declineStartDay;
            int endDay = Settings.instance.declineEndDay;
            float startDrop = Settings.instance.initialDrop;
            float endDrop = Settings.instance.maxDrop;
            return (numDays - startDay) / (endDay - startDay) * (endDrop - startDrop) + startDrop;
        }

        public static float ProgressiveAdjust(float refTemp, float maxAdjust)
        {
            float warmPoint = Settings.instance.warmPoint;
            float coldPoint = Settings.instance.coldPoint;
            if (refTemp > warmPoint) return 0;
            else if (refTemp < coldPoint) return maxAdjust;

            // increase drop across range to make weather more extreme but still occasionally benign
            float pos = (refTemp - warmPoint) / (coldPoint - warmPoint);
            return Mathf.Lerp(0f, maxAdjust, pos);
        }

        // return positive number of how many deg temp should be reduced
        // this will keep in logic of GetOutdoorTempDropCelcius()
        public static float GetTempDropC()
        {
            // TODO: what should this do?
            return 0.0f;
        }
    }


    [HarmonyPatch(typeof(Weather), nameof(Weather.CalculateCurrentTemperature))]
    internal class Weather_CalculateCurrentTemperature
    {
        public static void Postfix(Weather __instance)
        {
            if (!Settings.instance.enableExtremeTempDrop) return;
            if (GameManager.m_IsPaused)
            {
                return;
            }
            if (!InterfaceManager.IsMainMenuEnabled() && !__instance.IsIndoorEnvironment())
            {
                float t0 = __instance.m_CurrentTemperature;
                float numDays = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() / 24f;
                float vanilla = Common.VanillaAdjust(numDays);
                float scheduled = Common.ScheduledAdjust(vanilla);
                if (Settings.instance.enableProgressive)
                {
                    float ambient = __instance.GetCurrentTemperatureWithoutHeatSources();
                    float progressive = Common.ProgressiveAdjust(ambient, scheduled);
                    float t1 = t0 - vanilla + progressive;
#if DEBUG
                    MelonLogger.Msg("Extreme temp: t1 " + t1 +
                        " = t0 " + t0 +
                        " - vanilla " + vanilla +
                        " + progressive " + progressive +
                        " (ambient " + ambient + " scheduled " + scheduled + ")");
#endif
                    __instance.m_CurrentTemperature = t1;
                }
                else
                {
                    float t1 = t0 - vanilla + scheduled;
#if DEBUG
                    MelonLogger.Msg("Extreme temp: t1 " + t1 +
                        " = t0 " + t0 +
                        " - vanilla " + vanilla +
                        " + scheduled " + scheduled);
#endif
                    __instance.m_CurrentTemperature = t1;
                }
            }
        }
    }
}
