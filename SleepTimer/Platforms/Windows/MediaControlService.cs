using System.Runtime.InteropServices;

namespace SleepTimer.Platforms.Windows
{
    public class MediaControlService : IMediaControlService
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const byte VK_MEDIA_STOP = 0xB2;

        public void PauseOtherApps()
        {
            // Simulate media play/pause key press
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            // Simulate Media Stop key press
            keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
    }
}
