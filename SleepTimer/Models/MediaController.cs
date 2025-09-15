using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class MediaController(
        IVolumeService volumeService,
        IMediaControlService mediaService,
        AppPreferences appPreferences)
    {
        private readonly IVolumeService volumeService = volumeService;
        private readonly IMediaControlService mediaService = mediaService;
        private readonly AppPreferences appPreferences = appPreferences;
        private int startingVolume;

        
        public void HandleFadeOut(TimeSpan remainingTime)
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 1) return;

            // Note: 100% volume equals to value 16 on Android
            int newVolume = (startingVolume * (int)remainingTime.TotalSeconds / appPreferences.FadeOutSeconds);
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
        public void HandleFinished()
        {
            mediaService.StopPlayback();
            //RestoreVolume();
            volumeService.SetVolume(startingVolume);
        }
    }
}
