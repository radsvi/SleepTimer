using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;

namespace SleepTimer.Models
{
    public static class NotificationLevelHelper
    {
#pragma warning disable CA1416
        public static NotificationImportance MapToImportance(NotificationLevel level) => level switch
        {
            NotificationLevel.Min => NotificationImportance.Min,
            NotificationLevel.Low => NotificationImportance.Low,
            NotificationLevel.Default => NotificationImportance.Default,
            NotificationLevel.High => NotificationImportance.High,
            NotificationLevel.Max => NotificationImportance.High, // no direct Max
            _ => NotificationImportance.Default
        };
#pragma warning restore CA1416

        public static int MapToPriority(NotificationLevel level) => level switch
        {
            NotificationLevel.Min => (int)NotificationPriority.Min,
            NotificationLevel.Low => (int)NotificationPriority.Low,
            NotificationLevel.Default => (int)NotificationPriority.Default,
            NotificationLevel.High => (int)NotificationPriority.High,
            NotificationLevel.Max => (int)NotificationPriority.Max,
            _ => (int)NotificationPriority.Default
        };
    }
}
