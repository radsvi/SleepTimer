using System;
using System.Runtime.InteropServices;

namespace SleepTimer.Platforms.Windows;

public class VolumeService// : IVolumeService // Originally created with the old volumeService that were supposed to be paralel to the VolumeService for Android, but that one didn't work so I disabled both for now.
{
    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    private class MMDeviceEnumerator { }

    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDeviceEnumerator
    {
        int NotImpl1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

        // other methods not needed
    }

    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams,
            [MarshalAs(UnmanagedType.Interface)] out IAudioEndpointVolume ppInterface);
    }

    [ComImport]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioEndpointVolume
    {
        // Only methods we need
        void RegisterControlChangeNotify(IntPtr pNotify);
        void UnregisterControlChangeNotify(IntPtr pNotify);
        void GetChannelCount(out uint pnChannelCount);
        void SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
        void SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
        void GetMasterVolumeLevel(out float pfLevelDB);
        void GetMasterVolumeLevelScalar(out float pfLevel);
        void SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext);
        void GetMute(out bool pbMute);
        // ... (methods omitted for brevity)
    }

    private enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
    }

    private enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
    }

    private static IAudioEndpointVolume? GetAudioEndpointVolume()
    {
        var enumerator = new MMDeviceEnumerator() as IMMDeviceEnumerator;
        if (enumerator == null) return null;

        if (enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out var device) != 0)
            return null;

        Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;

        device.Activate(ref IID_IAudioEndpointVolume, 23 /* CLSCTX_ALL */, IntPtr.Zero, out var volume);
        return volume;
    }

    public void SetVolume(int level)
    {
        var endpoint = GetAudioEndpointVolume();
        if (endpoint == null) return;

        float scalar = Math.Clamp(level, 0, 100) / 100f;
        endpoint.SetMasterVolumeLevelScalar(scalar, Guid.Empty);
    }

    public int GetVolume()
    {
        var endpoint = GetAudioEndpointVolume();
        if (endpoint == null) return -1;

        endpoint.GetMasterVolumeLevelScalar(out float scalar);
        return (int)(scalar * 100);
    }
}