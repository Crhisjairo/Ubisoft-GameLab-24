using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitFunction : MonoBehaviour
{
    public void QuitGame()
    {
        // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // For use in the Unity Editor
#else
        Application.Quit(); // For use in a built game
#endif
    }
}
