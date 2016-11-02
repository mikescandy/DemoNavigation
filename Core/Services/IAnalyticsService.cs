namespace Knuj.Interfaces.Services
{
    public interface IAnalyticsService
    {
        void RecordEvent(string eventName);
        void TrackPushNotificationInteraction(bool elapsed);
    }
}
