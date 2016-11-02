namespace Knuj.Interfaces
{
    public interface ICoreCache : IPersistentCache
    {
        bool ReadIsFirstRun();
        void SaveIsFirstRun(bool value);
        string ReadDeviceNotificationsToken();
        void SaveDeviceNotificationsToken(string value);
        string ReadUserId();
        void SaveUserId(string value);
        bool PersistSaveKeyEncryptingValue(string key, string value);
        string ReadStringByKeyDecryptingValue(string key, string defaultValue);
    }
}