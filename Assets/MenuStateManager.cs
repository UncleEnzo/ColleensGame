using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuStateManager : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject startMenu;
    public GameObject SaveNameMenu;
    public GameObject CreateGameMenu;
    public GameObject LoadSaveMenu;
    public GameObject resultsPanel;


    //internal variables
    bool canStartCreatingGame = false;
    int matchNum = 1;
    string goodJob = "Good Job!!!";
    string noMatch = "Close but not a match. Try again!";
    string youWin = "You Win!";

    // Start is called before the first frame update
    void Start()
    {
        StartMenu();
        resultsPanel.SetActive(false);
    }

    //------------utility button methods------

    public void CreateNewSave(string saveName)
    {
        if (saveName != null && saveName != "")
        {
            Debug.Log("Creating a save. SaveName:" + saveName);
            if (SaveData.Instance.match == null)
            {
                SaveData.Instance.match = new SavedMatch();
            }
            SaveData.Instance.match.loadMatchName = saveName;
            canStartCreatingGame = true;
        }
        else
        {
            canStartCreatingGame = false;
        }
    }

    public void CreateMatchButton()
    {
        string imgURL = FindObjectOfType<ImageLoader>().url;
        string phrase = FindObjectOfType<PhraseInputHandler>().GetInput();
        if (imgURL != null && phrase != null && imgURL != "" && phrase != "")
        {
            Debug.Log("Saving Match. Img:" + imgURL + " Phrase: " + phrase);
            SaveData.Instance.match.savedMatches.Add(imgURL, phrase);
            FindObjectOfType<ImgInputHandler>().gameObject.GetComponent<TMP_InputField>().text = "";
            FindObjectOfType<PhraseInputHandler>().gameObject.GetComponent<TMP_InputField>().text = "";
            FindObjectOfType<ImageLoader>().gameObject.GetComponent<Image>().sprite = null;

            if (matchNum >= 9)
            {
                CreateGameMenu.transform.Find("CreateMatchButton").gameObject.SetActive(false);
                CreateGameMenu.transform.Find("DoneButton").gameObject.SetActive(true);
            }
            if (matchNum < 9)
            {
                matchNum++;
                CreateGameMenu.transform.Find("MatchesCompleteText").gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("Match # {0} out of 9", matchNum);
            }
        }
        else
        {
            Debug.Log("Supply an image and string");
            return;
        }
    }

    public void TryMatchButton()
    {
        if (SaveManager.Instance.SelectedGameVariableImage == "Default Image URL String"
            || SaveManager.Instance.SelectedGameVariableString == "Default Text String")
        {
            Debug.Log("You need to select an image and a text string");
            return;
        }

        KeyValuePair<string, string> dictEntry = new KeyValuePair<string, string>(SaveManager.Instance.SelectedGameVariableImage, SaveData.Instance.match.savedMatches[SaveManager.Instance.SelectedGameVariableImage]);
        if (dictEntry.Key == SaveManager.Instance.SelectedGameVariableImage
            && dictEntry.Value == SaveManager.Instance.SelectedGameVariableString)
        {
            Debug.Log("YOU HAVE A MATCH!");

            foreach (Transform image in SaveManager.Instance.gameUIImagePanel.transform)
            {
                if (image.gameObject.GetComponentInChildren<StoreUrl>().buttonUrl == SaveManager.Instance.SelectedGameVariableImage)
                {
                    Destroy(image.gameObject);
                }

            }
            foreach (Transform stringText in SaveManager.Instance.gameUIStringPanel.transform)
            {
                if (stringText.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == SaveManager.Instance.SelectedGameVariableString)
                {
                    Destroy(stringText.gameObject);
                }
            }

            //resetting 
            SaveManager.Instance.SelectedGameVariableImage = "Default Image URL String";
            SaveManager.Instance.SelectedGameVariableString = "Default Text String";

            if (SaveManager.Instance.gameUIImagePanel.transform.childCount == 1
                && SaveManager.Instance.gameUIStringPanel.transform.childCount == 1)
            {
                resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = youWin;
                StartCoroutine(ResultsCountdownCo(true));
            }
            else
            {
                resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = goodJob;
                StartCoroutine(ResultsCountdownCo());
            }

        }
        else
        {
            Debug.Log("The image and string don't match :(");
            resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = noMatch;
            StartCoroutine(ResultsCountdownCo());
        }
    }

    IEnumerator ResultsCountdownCo(bool lastScreen = false)
    {
        resultsPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        resultsPanel.SetActive(false);
        if (lastScreen)
        {
            StartMenu();
        }
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    //------------Open Menu Methods-----------
    public void StartMenu()
    {
        canStartCreatingGame = false;
        gameUI.SetActive(false);
        startMenu.SetActive(true);
        SaveNameMenu.SetActive(false);
        CreateGameMenu.SetActive(false);
        LoadSaveMenu.SetActive(false);
    }

    public void OpenCreateGameNameMatch()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        SaveNameMenu.SetActive(true);
        CreateGameMenu.SetActive(false);
        LoadSaveMenu.SetActive(false);
    }

    public void StartGamePage()
    {
        gameUI.SetActive(true);
        startMenu.SetActive(false);
        SaveNameMenu.SetActive(false);
        CreateGameMenu.SetActive(false);
        LoadSaveMenu.SetActive(false);
    }

    public void OpenCreateGame()
    {
        if (canStartCreatingGame)
        {
            gameUI.SetActive(false);
            startMenu.SetActive(false);
            SaveNameMenu.SetActive(false);
            CreateGameMenu.SetActive(true);
            CreateGameMenu.transform.Find("DoneButton").gameObject.SetActive(false);
            LoadSaveMenu.SetActive(false);
            matchNum = 1;
            CreateGameMenu.transform.Find("MatchesCompleteText").gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("Match # {0} out of 9", matchNum);
        }
        else
        {
            Debug.Log("Need to enter a name for save");
        }
    }

    public void DoneAndSaveGame()
    {
        FindObjectOfType<SaveManager>().OnSave();
        gameUI.SetActive(false);
        startMenu.SetActive(true);
        SaveNameMenu.SetActive(false);
        CreateGameMenu.SetActive(false);
        LoadSaveMenu.SetActive(false);
    }

    public void LoadASaveMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        SaveNameMenu.SetActive(false);
        CreateGameMenu.SetActive(false);
        LoadSaveMenu.SetActive(true);
    }
}
