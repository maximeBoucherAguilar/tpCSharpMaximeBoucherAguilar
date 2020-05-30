using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Essentials;
using System;
using Android.Media;
using Android.Content.PM;
using TPVoixMaximeBoucherAguilar.Essentials;

namespace TPVoixMaximeBoucherAguilar
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private MediaPlayer player1, player2;
        private AccelerometerReader accelerometerReader;
        private ShakeDetector shakeDetector;
        private AccelerometerPosition accStart, accBefore, accNow;
        private int button1, button2;
        private int cpt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Button buttonVoix1 = FindViewById<Button>(Resource.Id.buttonVoix1);
            Button buttonVoix2 = FindViewById<Button>(Resource.Id.buttonVoix2);
            Button buttonVoixOff = FindViewById<Button>(Resource.Id.buttonVoixoff);
            button1 = 1;
            button2 = 2;
            cpt = 0;

            accelerometerReader = new AccelerometerReader();
            shakeDetector = new ShakeDetector();
            accStart = new AccelerometerPosition();
            accBefore = new AccelerometerPosition();
            accNow = new AccelerometerPosition();

            buttonVoix1.Click += (sender, e) =>
            {
                Setvoices(button1);
                SetAccPosition(accStart, accelerometerReader);
                shakeDetector.ResetShakePlayer();
                accelerometerReader.ResetAccPlayer();
                cpt = 0;
                if (player2 != null) player2.Reset();
                StartTimer(accelerometerReader, player1);
                if(!Accelerometer.IsMonitoring) Accelerometer.Start(SensorSpeed.Game);
                player1.Start();
            };

            buttonVoix2.Click += (sender, e) =>
            {
                Setvoices(button2);
                SetAccPosition(accStart, accelerometerReader);
                shakeDetector.ResetShakePlayer();
                accelerometerReader.ResetAccPlayer();
                cpt = 0;
                if (player1 != null)  player1.Reset();
                StartTimer(accelerometerReader, player2);
                if (!Accelerometer.IsMonitoring) Accelerometer.Start(SensorSpeed.Game);
                player2.Start();
            };

            buttonVoixOff.Click += (sender, e) =>
            {
                if(Accelerometer.IsMonitoring) Accelerometer.Stop();
                accelerometerReader.Start = false;
                shakeDetector.Start = false;
            };
        }

        private void SetAccPosition(AccelerometerPosition accPos, AccelerometerReader accelerometerReader)
        {
            accPos.accX = accelerometerReader.AccX;
            accPos.accY = accelerometerReader.AccY;
            accPos.accZ = accelerometerReader.AccZ;
        }

        private void Setvoices(int button)
        {
            switch (button)
            {
                case 1:
                    player1 = MediaPlayer.Create(this, Resource.Raw.Voice01_01);
                    accelerometerReader.AccInclinedMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice01_02);
                    accelerometerReader.AccInclinedMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice01_03);
                    accelerometerReader.AccInclinedMedia3 = MediaPlayer.Create(this, Resource.Raw.Voice01_04);
                    shakeDetector.ShockMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice01_05);
                    shakeDetector.ShockMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice01_06);
                    shakeDetector.ShockMedia3 = MediaPlayer.Create(this, Resource.Raw.Voice01_07);
                    accelerometerReader.AccStableMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice01_08);
                    accelerometerReader.AccStableMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice01_09);
                    break;
                case 2:
                    player2 = MediaPlayer.Create(this, Resource.Raw.Voice02_01);
                    accelerometerReader.AccInclinedMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice02_02);
                    accelerometerReader.AccInclinedMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice02_03);
                    accelerometerReader.AccInclinedMedia3 = MediaPlayer.Create(this, Resource.Raw.Voice02_04);
                    shakeDetector.ShockMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice02_05);
                    shakeDetector.ShockMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice02_06);
                    shakeDetector.ShockMedia3 = MediaPlayer.Create(this, Resource.Raw.Voice02_07);
                    accelerometerReader.AccStableMedia1 = MediaPlayer.Create(this, Resource.Raw.Voice02_08);
                    accelerometerReader.AccStableMedia2 = MediaPlayer.Create(this, Resource.Raw.Voice02_09);
                    break;
                default:
                    break;
            }
        }

        private void StartTimer(AccelerometerReader accelerometerReader, MediaPlayer mp)
        {
            System.Timers.Timer Timer1 = new System.Timers.Timer();
            Timer1.Start();
            Timer1.Interval = 500;
            Timer1.Enabled = true;
            Timer1.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    accBefore.accX = accNow.accX;
                    accBefore.accY = accNow.accY;
                    accBefore.accZ = accNow.accZ;
                    SetAccPosition(accNow, accelerometerReader);
                    if (accBefore.accX != 0)
                    {
                        if (DisplayVoices(accBefore, accNow, mp, accelerometerReader))
                        {
                            cpt = 0;
                        } else
                        {
                            cpt += 1;
                        }

                        if (cpt > 20) cpt = 0;
                    }
                });
            };
        }

        private bool DisplayVoices(AccelerometerPosition accBefore, AccelerometerPosition accNow, MediaPlayer mp, AccelerometerReader accelerometerReader)
        {
            if(Math.Abs(accBefore.accX-accNow.accX) > 0.3 || Math.Abs(accBefore.accY - accNow.accY) > 0.3 || Math.Abs(accBefore.accZ - accNow.accZ) > 0.3)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    accelerometerReader.PlayInclinedSongs(shakeDetector);
                });
                return true;
            } else if (cpt == 10 || cpt == 20)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    accelerometerReader.PlayStableSongs(shakeDetector);
                });
            }
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}