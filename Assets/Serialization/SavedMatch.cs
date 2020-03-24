using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SavedMatch
{
    public string loadMatchName;
    public Dictionary<string, string> savedMatches = new Dictionary<string, string>();
}
