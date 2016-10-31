namespace Knuj.Interfaces
{
    public interface IPersistentCache
    {
        bool PersistSaveKeyValue(string key, string value);
        string ReadStringByKey(string key, string defaultValue);
        bool PersistSaveKeyValue(string key, bool value);
        bool ReadBoolByKey(string key, bool defaultValue);
        void RemoveItem(string key);
        void Clear();
    }
}