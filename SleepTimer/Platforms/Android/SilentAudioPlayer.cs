using global::Android.App;
using global::Android.Media;
using global::Android.Content;
using System;

namespace SleepTimer.Platforms.Android
{
    public class SilentAudioPlayer
    {
        private MediaPlayer? _player;

        public void Start()
        {
            var context = global::Android.App.Application.Context;

            _player = new MediaPlayer();

            // Open silent audio from Assets
            //var afd = context.Assets.OpenFd("silence.wav");
            //var afd = context.Assets.OpenFd("Surrender.wav");
            //using var afd = context.Assets.OpenFd("silence.mp3");
            //using var afd = context.Assets.OpenFd("backgroundsound.mp3");
            using var afd = context.Assets.OpenFd("winamp.mp3");
            _player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            //afd.Close();

            _player.Looping = true; // repeat indefinitely
            //_player.SetVolume(0f, 0f); // completely silent
            _player.Prepare();
            _player.Start();
        }

        public void Stop()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Release();
                _player = null;
            }
        }
    }
}
