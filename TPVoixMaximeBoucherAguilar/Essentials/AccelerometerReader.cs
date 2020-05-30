using Android.Media;
using System;
using Xamarin.Essentials;

namespace TPVoixMaximeBoucherAguilar.Essentials
{
    public class AccelerometerReader
    {
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public MediaPlayer AccInclinedMedia1 { get; set; }
        public MediaPlayer AccInclinedMedia2 { get; set; }
        public MediaPlayer AccInclinedMedia3 { get; set; }
        public MediaPlayer AccStableMedia1 { get; set; }
        public MediaPlayer AccStableMedia2 { get; set; }
        public bool Acc1 { get; set; }
        public bool Acc2 { get; set; }
        public bool Acc3 { get; set; }
        public bool Acc4 { get; set; }
        public bool Acc5 { get; set; }
        public bool Start { get; set; }

        public AccelerometerReader()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }
        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            AccX = data.Acceleration.X;
            AccY = data.Acceleration.Y;
            AccZ = data.Acceleration.Z;

            Console.WriteLine($"Reading: X: {data.Acceleration.X}, Y: {data.Acceleration.Y}, Z: { data.Acceleration.Z}");
            // Process Acceleration X, Y, and Z
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

        public void PlayInclinedSongs()
        {
            if (Start)
            {
                if (Acc1 && !AccInclinedMedia2.IsPlaying && !AccInclinedMedia3.IsPlaying)
                {
                    AccInclinedMedia1.Start();
                    Acc1 = false;
                    Acc2 = true;
                }
                else if (Acc2 && !AccInclinedMedia1.IsPlaying && !AccInclinedMedia3.IsPlaying)
                {
                    AccInclinedMedia2.Start();
                    Acc2 = false;
                    Acc3 = true;
                }
                else if (Acc3 && !AccInclinedMedia1.IsPlaying && !AccInclinedMedia2.IsPlaying)
                {
                    AccInclinedMedia3.Start();
                    Acc3 = false;
                    Acc1 = true;
                }
            }
        }

        public void PlayStableSongs()
        {
            if (Start)
            {
                if (Acc4 && !AccStableMedia2.IsPlaying)
                {
                    AccStableMedia1.Start();
                    Acc4 = false;
                    Acc5 = true;
                }
                else if (Acc5 && !AccStableMedia1.IsPlaying)
                {
                    AccStableMedia2.Start();
                    Acc5 = false;
                    Acc4 = true;
                }
            }
        }

        public void ResetAccPlayer()
        {
            Start = true;
            Acc1 = true;
            Acc2 = false;
            Acc3 = false;
            Acc4 = true;
            Acc5 = false;
        }
    }
}