using Android.Media;
using Android.App;
using Android.Content;
using Android.OS;
using Java.Lang;
using Java.Lang.Reflect;

namespace SleepTimer.Platforms.Android
{
    public class AudioFocusHelper : IAudioFocusHelper
    {
        private static Java.Lang.Object _focusRequest;

        public void RequestAudioFocus()
        {
            var context = global::Android.App.Application.Context;
            var audioManager = (AudioManager?)context.GetSystemService(Context.AudioService)
                ?? throw new InvalidOperationException("AudioManager not available");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                try
                {
                    // Java reflection
                    var builderType = Class.ForName("android.media.AudioFocusRequest$Builder");
                    var constructor = builderType.GetConstructor(new Class[] { Class.FromType(typeof(int)) });
                    var builderInstance = constructor.NewInstance(new Java.Lang.Integer((int)AudioFocus.Gain));

                    // Build AudioFocusRequest
                    var buildMethod = builderType.GetMethod("build");
                    _focusRequest = buildMethod.Invoke(builderInstance);

                    // Get the class of the built AudioFocusRequest
                    var afrClass = _focusRequest.Class;

                    // requestAudioFocus(AudioFocusRequest)
                    //var requestMethod = audioManager.Class.GetMethod("requestAudioFocus", new Class[] { afrClass });
                    //requestMethod.Invoke(audioManager, new object[] { _focusRequest });
                    var requestMethod = audioManager.Class.GetMethod("requestAudioFocus", new Class[] { _focusRequest.Class });
                    requestMethod.Invoke(audioManager, new Java.Lang.Object[] { _focusRequest });
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AudioFocusRequest reflection failed: {ex}");
                }
            }
            else
            {
                audioManager.RequestAudioFocus(null, global::Android.Media.Stream.Music, AudioFocus.Gain);
            }
        }

        public void AbandonAudioFocus()
        {
            var context = global::Android.App.Application.Context;
            var audioManager = (AudioManager)context.GetSystemService(Context.AudioService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && _focusRequest != null)
            {
                try
                {
                    var abandonMethod = audioManager.Class.GetMethod("abandonAudioFocusRequest", _focusRequest.Class);
                    abandonMethod.Invoke(audioManager, _focusRequest);
                    _focusRequest = null;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AbandonAudioFocusRequest failed: {ex}");
                }
            }
            else
            {
                audioManager.AbandonAudioFocus(null);
            }
        }
    }
}
