using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhraseInputHandler : InputHandler
{
    public void HandlePhrase(string phrase)
    {
        if (phrase != null && phrase != "")
        {
            Debug.Log("Entered phrase: " + phrase);
            savedString = phrase;
        }
    }
}
