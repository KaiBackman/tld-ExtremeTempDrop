using ModSettings;
using System;
using System.Reflection;

namespace ExtremeTempDrop
{
    class Settings : JsonModSettings
    {
        internal static Settings instance = new Settings();

        [Name("Enable")]
        [Description("If enabled will replace the base game temperature drop with a custom profile. Might stack with other mods like Relentless Night, disable if in doubt.")]
        public bool enableExtremeTempDrop = false;

        [Name("Preset")]
        [Description("The default preset values from the base game. Extreme is a progressive preset where blizzards are almost immediately Interloper deadly but mild weather remains possible.")]
        [Choice("Custom", "Pilgrim", "Voyager", "Stalker", "Interloper", "Extreme")]
        public int preset = 1;

        [Name("Initial Temperature Drop")]
        [Description("The global temperature \"drop\" on day 0.")]
        [Slider(0, -100, 101)]
        public float initialDrop = 0f;

        [Name("Maximum Temperature Drop")]
        [Description("The maximum decrease in global temperatures.")]
        [Slider(0, -100, 101)]
        public float maxDrop = 0f;

        [Name("Decline Starting Day")]
        [Description("The day at which global temperatures start dropping.")]
        [Slider(0, 200)]
        public int declineStartDay = 0;

        [Name("Decline Ending Day")]
        [Description("The day at which global temperatures do not drop any farther.")]
        [Slider(0, 200)]
        public int declineEndDay = 0;

        [Section("Progressive")]

        [Name("Progressive")]
        [Description("Enabling progressive will apply the temperature drop along the given range between the Warm and Cold points. Close to the Warm Point temperatures remain mostly unchanged, close to the Cold Point most of the drop will be felt.")]
        public bool enableProgressive = false;

        [Name("Warm Point")]
        [Description("Temperatures warmer than this point will not be adjusted")]
        [Slider(0, -100, 101)]
        public float warmPoint = -10f;

        [Name("Cold Point")]
        [Description("Temperatures colder than this will receive full adjustment")]
        [Slider(0, -100, 101)]
        public float coldPoint = -40f;

        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue)
        {
            base.OnChange(field, oldValue, newValue);
            if (field.Name == nameof(preset)) UsePreset((int)newValue);
            else preset = 0;

            if (field.Name == nameof(initialDrop)) maxDrop = Math.Min((float)newValue, maxDrop);
            else if (field.Name == nameof(maxDrop)) initialDrop = Math.Max((float)newValue, initialDrop);
            else if (field.Name == nameof(declineStartDay)) declineEndDay = Math.Max((int)newValue, declineEndDay);
            else if (field.Name == nameof(declineEndDay)) declineStartDay = Math.Min((int)newValue, declineStartDay);
            else if (field.Name == nameof(warmPoint)) coldPoint = Math.Min((float)newValue, coldPoint);
            else if (field.Name == nameof(coldPoint)) warmPoint = Math.Max((float)newValue, warmPoint);
            else if (field.Name == nameof(enableExtremeTempDrop))
            {
//                bool enabled = (bool)newValue;
// FIXME toggle visibility
            }
            else if (field.Name == nameof(enableProgressive))
            {
 //               bool enabled = (bool)newValue;
 //               SetFieldVisible(warmPoint, enabled);
 //               SetFieldVisible(coldPoint, enabled);
            }

            RefreshGUI();
        }
        private void UsePreset(int preset)
        {
            switch (preset)
            {
                case 1:
                    initialDrop = 0f;
                    maxDrop = 0f;
                    declineStartDay = 0;
                    declineEndDay = 0;
                    enableProgressive = false;
                    break;
                case 2:
                    initialDrop = 0f;
                    maxDrop = -5f;
                    declineStartDay = 20;
                    declineEndDay = 200;
                    enableProgressive = false;
                    break;
                case 3:
                    initialDrop = 0f;
                    maxDrop = -10f;
                    declineStartDay = 20;
                    declineEndDay = 200;
                    enableProgressive = false;
                    break;
                case 4:
                    initialDrop = 0f;
                    maxDrop = -20f;
                    declineStartDay = 5;
                    declineEndDay = 50;
                    enableProgressive = false;
                    break;
                case 5:
                    initialDrop = -5f;
                    maxDrop = -20f;
                    declineStartDay = 1;
                    declineEndDay = 5;
                    enableProgressive = true;
                    warmPoint = -10.0f;
                    coldPoint = -40.0f;
                    break;
            }
        }
    }
}
