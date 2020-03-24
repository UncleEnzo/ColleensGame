using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    protected string savedString = "";
    public string GetInput()
    {
        return savedString;
    }
}
