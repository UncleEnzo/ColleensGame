[System.Serializable]
public class SaveData
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public SavedMatch match;

    public void OnLoadGame(string path)
    {
        SaveData.Instance = (SaveData)SerializationManager.Load(path);
    }
}
