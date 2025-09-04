using Android.Media;
using Android.Content;
using Android.App;
using Android.Runtime;


namespace SleepTimer.Platforms.Android.Services
{
    //public class AudioFocusService : IAudioFocusService
    //{
    //    private readonly AudioManager _audioManager;
    //    private readonly AudioFocusRequest _audioFocusRequest;
    //    private readonly AudioFocusChangeListener _audioFocusListener;

    //    public AudioFocusService()
    //    {
    //        _audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(Context.AudioService)
    //            ?? throw new InvalidOperationException("AudioManager not available");


    //        _audioFocusListener = new AudioFocusChangeListener();
    //        _audioFocusRequest = new AudioFocusRequest.Builder(AudioFocus.Gain)
    //            .SetOnAudioFocusChangeListener(_audioFocusListener)
    //            .SetAudioAttributes(new AudioAttributes.Builder()
    //                .SetUsage(AudioUsageKind.Media)
    //                .SetContentType(AudioContentType.Music)
    //                .Build())
    //            .Build();
    //    }

    //    public void RequestAudioFocus()
    //    {
    //        var result = _audioManager.RequestAudioFocus(_audioFocusRequest);
    //        if (result == AudioFocusRequestResult.Granted)
    //        {
    //            // Audio focus granted, you can start playing audio.
    //            // Other apps will receive a focus loss event and should stop.
    //        }
    //    }

    //    public void AbandonAudioFocus()
    //    {
    //        _audioManager.AbandonAudioFocusRequest(_audioFocusRequest);
    //    }

    //    private class AudioFocusChangeListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    //    {
    //        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
    //        {
    //            switch (focusChange)
    //            {
    //                case AudioFocus.Loss:
    //                case AudioFocus.LossTransient:
    //                    // Another app gained focus, pause your playback.
    //                    // You can handle this event here.
    //                    break;
    //                case AudioFocus.Gain:
    //                    // We regained focus, resume playback if it was paused.
    //                    break;
    //            }
    //        }
    //    }
    //}
}
