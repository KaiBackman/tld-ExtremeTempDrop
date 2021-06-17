using ModSettings;
using System;
using System.Reflection;

namespace ExtremeTempDrop
{
    class Settings : JsonModSettings
    {
        internal static Settings instance = new Settings();

        [Name("Preset")]
        [Description("The default preset values from the base game. If you're using Relentless Night and don't want this to stack with its temperature degradation, you need to use the Pilgram preset.")]
        [Choice("Custom", "Pilgram", "Voyager", "Stalker", "Interloper")]
        public int preset = 1;

        [Name("Initial Temperature Drop")]
        [Description("The global temperature \"drop\" on day 0.")]
        [Slider(0, 100, 101)]
        public float initialDrop = 0f;

        [Name("Decline Starting Day")]
        [Description("The day at which global temperatures start dropping.")]
        [Slider(0, 200)]
        public int declineStartDay = 0;

        [Name("Maximum Temperature Drop")]
        [Description("The maximum decrease in global temperatures.")]
        [Slider(0, 100, 101)]
        public float maxDrop = 0f;

        [Name("Decline Ending Day")]
        [Description("The day at which global temperatures do not drop any farther.")]
        [Slider(0, 200)]
        public int declineEndDay = 0;
        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            base.OnChange(field, oldValue, newValue);
            if (field.Name == nameof(preset)) UsePreset((int)newValue);
            else preset = 0;

            if (field.Name == nameof(initialDrop)) maxDrop = Math.Max((float)newValue, maxDrop);
            else if (field.Name == nameof(maxDrop)) initialDrop = Math.Min((float)newValue, initialDrop);
            else if (field.Name == nameof(declineStartDay)) declineEndDay = Math.Max((int)newValue, declineEndDay);
            else if (field.Name == nameof(declineEndDay)) declineStartDay = Math.Min((int)newValue, declineStartDay);

            RefreshGUI();
        }
        private void UsePreset(int preset)
        {
            switch (preset)
            {
                case 1:
                    initialDrop = 0f;
                    declineStartDay = 0;
                    maxDrop = 0f;
                    declineEndDay = 0;
                    break;
                case 2:
                    initialDrop = 0f;
                    declineStartDay = 20;
                    maxDrop = 5f;
                    declineEndDay = 200;
                    break;
                case 3:
                    initialDrop = 0f;
                    declineStartDay = 20;
                    maxDrop = 10f;
                    declineEndDay = 200;
                    break;
                case 4:
                    initialDrop = 0f;
                    declineStartDay = 5;
                    maxDrop = 20f;
                    declineEndDay = 50;
                    break;
            }
        }
    }
}
