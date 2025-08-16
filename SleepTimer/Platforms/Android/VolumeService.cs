using Android.Content;
using Android.Media;

namespace SleepTimer.Platforms.Android;

public class VolumeService : IVolumeService
{
    private readonly AudioManager _audioManager;

    public VolumeService()
    {
        _audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService)!;
    }

    public void SetVolume(int level)
    {
        int maxVolume = _audioManager.GetStreamMaxVolume(global::Android.Media.Stream.Music);
        int newVolume = (int)(Math.Clamp(level, 0, 100) / 100.0 * maxVolume);
        //_audioManager.SetStreamVolume(global::Android.Media.Stream.Music, newVolume, VolumeNotificationFlags.ShowUi);
        _audioManager.SetStreamVolume(global::Android.Media.Stream.Music, newVolume, 0);
    }

    public int GetVolume()
    {
        int maxVolume = _audioManager.GetStreamMaxVolume(global::Android.Media.Stream.Music);
        int current = _audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        return (int)(current * 100.0 / maxVolume);
    }
}
