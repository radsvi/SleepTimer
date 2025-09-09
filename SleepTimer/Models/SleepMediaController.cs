using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class SleepMediaController(
        IVolumeService volumeService,
        IMediaControlService mediaService,
        AppPreferences appPreferences)
    {
        private readonly IVolumeService volumeService = volumeService;
        private readonly IMediaControlService mediaService = mediaService;
        private readonly AppPreferences appPreferences = appPreferences;
        private int startingVolume;

        public void CaptureStartingVolume() => startingVolume = volumeService.GetVolume();
        public void HandleFadeOut(TimeSpan remainingTime)
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 1) return;

            int newVolume = (startingVolume * remainingTime.Seconds / appPreferences.FadeOutDuration);
            volumeService.SetVolume(newVolume);
        }
        public void EnterStandby()
        {
            volumeService.SetVolume(0);
            mediaService.StopPlayback();
        }
        public void SetStartingVolume()
        {
            startingVolume = volumeService.GetVolume();
        }
        public void RestoreVolume()
        {
            volumeService.SetVolume(startingVolume);
        }
        public void StopPlayback()
        {
            mediaService.StopPlayback();
            RestoreVolume();
        }
    }
}
