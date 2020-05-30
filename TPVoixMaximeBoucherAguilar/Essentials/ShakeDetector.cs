using System;
using Xamarin.Essentials;
using Android.Media;

namespace TPVoixMaximeBoucherAguilar.Essentials
{
    public class ShakeDetector
    {
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.Game;
        public MediaPlayer ShockMedia1 { get; set; }
        public MediaPlayer ShockMedia2 { get; set; }
        public MediaPlayer ShockMedia3 { get; set; }
        public bool Shock1 { get; set; }
        public bool Shock2 { get; set; }
        public bool Shock3 { get; set; }
        public bool Start { get; set; }

        public ShakeDetector()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
        }
        void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            if (Start)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Shock1 && !ShockMedia2.IsPlaying && !ShockMedia3.IsPlaying)
                    {
                        ShockMedia1.Start();
                        Shock1 = false;
                        Shock2 = true;
                    }
                    else if (Shock2 && !ShockMedia1.IsPlaying && !ShockMedia3.IsPlaying)
                    {
                        ShockMedia2.Start();
                        Shock2 = false;
                        Shock3 = true;
                    }
                    else if (Shock3 && !ShockMedia1.IsPlaying && !ShockMedia2.IsPlaying)
                    {
                        ShockMedia3.Start();
                        Shock3 = false;
                        Shock1 = true;
                    }
                });
            }
        }
        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public void ResetShakePlayer()
        {
            Start = true;
            Shock1 = true;
            Shock2 = false;
            Shock3 = false;
        }
    }
}