using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one Instance of Player found.");
        }
        Instance = this;
        SelectedGameVariableImage = "Default Image URL String";
        SelectedGameVariableString = "Default Text String";
    }

    public GameObject loadButtonPrefab;
    public Transform loadArea;
    public GameObject gameUIImagePanel;
    public GameObject gameUIStringPanel;
    public GameObject gameUIImageButtonPrefab;
    public GameObject gameUIStringButtonPrefab;
    string[] saveFiles;

    //Game logic variables
    public string SelectedGameVariableImage;
    public string SelectedGameVariableString;


    public void OnSave()
    {
        SerializationManager.Save(SaveData.Instance.match.loadMatchName, SaveData.Instance);
    }

    void OnLoad(string saveFile)
    {
        SaveData.Instance.OnLoadGame(saveFile); // loads the correct one.

        Debug.Log("Loading same: " + SaveData.Instance.match.loadMatchName);
        foreach (Transform child in gameUIImagePanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in gameUIStringPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Creates all of the empty locations
        List<GameObject> listInstantiatedImages = new List<GameObject>();
        List<GameObject> listInstantiatedTexts = new List<GameObject>();
        for (int i = 0; i < SaveData.Instance.match.savedMatches.Count; i++)
        {
            GameObject gameUIImage = Instantiate(gameUIImageButtonPrefab);
            gameUIImage.transform.SetParent(gameUIImagePanel.transform, false);
            listInstantiatedImages.Add(gameUIImage);

            GameObject gameUIstring = Instantiate(gameUIStringButtonPrefab);
            gameUIstring.transform.SetParent(gameUIStringPanel.transform, false);
            listInstantiatedTexts.Add(gameUIstring);
        }

        //Randomize locations of image and strings
        foreach (KeyValuePair<string, string> pair in SaveData.Instance.match.savedMatches)
        {
            //Need to find a random image location from the panel
            GameObject randomImage = listInstantiatedImages[Random.Range(0, (listInstantiatedImages.Count))];
            randomImage.transform.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(LoadUrlImageCo(randomImage.transform.GetChild(0).GetComponent<Image>(), pair.Key, randomImage.GetComponentInChildren<StoreUrl>()));
            listInstantiatedImages.Remove(randomImage);
            listInstantiatedImages.OrderByDescending(v => v);
            randomImage.transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectedGameVariableImage = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<StoreUrl>().buttonUrl;
                Debug.Log("Selected image url: " + SelectedGameVariableImage);
                foreach (Transform image in gameUIImagePanel.transform)
                {
                    image.transform.GetChild(1).gameObject.SetActive(false);
                }
                EventSystem.current.currentSelectedGameObject.transform.GetChild(1).gameObject.SetActive(true);
            });

            //need to find a random text location from the panel
            GameObject randomString = listInstantiatedTexts[Random.Range(0, (listInstantiatedTexts.Count))];
            randomString.transform.GetChild(1).gameObject.SetActive(false);
            randomString.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = pair.Value;
            listInstantiatedTexts.Remove(randomString);
            listInstantiatedTexts.OrderByDescending(v => v);
            randomString.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectedGameVariableString = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
                Debug.Log("Selected string is: " + SelectedGameVariableString);
                foreach (Transform textString in gameUIStringPanel.transform)
                {
                    textString.transform.GetChild(1).gameObject.SetActive(false);
                }
                EventSystem.current.currentSelectedGameObject.transform.GetChild(1).gameObject.SetActive(true);
            });
        }
    }

    IEnumerator LoadUrlImageCo(Image targetImage, string Url, StoreUrl buttonUrlHolder)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Url);
        var asyncOperation = www.SendWebRequest();
        float progress;
        while (!www.isDone)
        {
            progress = asyncOperation.progress;
            yield return null;
        }
        progress = 1f;


        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            Url = null;
        }

        var texture = DownloadHandlerTexture.GetContent(www);

        targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        buttonUrlHolder.buttonUrl = Url;
    }


    public void ShowLoadScreen()
    {
        GetLoadFiles();
        foreach (Transform button in loadArea)
        {
            Destroy(button.gameObject);
        }

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject buttonObject = Instantiate(loadButtonPrefab);
            buttonObject.transform.SetParent(loadArea.transform, false);
            var index = i;
            buttonObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                OnLoad(saveFiles[index]);
                FindObjectOfType<MenuStateManager>().StartGamePage();
            });
            buttonObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                SerializationManager.Delete(saveFiles[index]);
                ShowLoadScreen();
            });
            buttonObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
        }
    }

    void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }
        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
}
