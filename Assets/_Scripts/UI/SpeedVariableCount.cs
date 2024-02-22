using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedVariableCount : MonoBehaviour
{
    public static int speedVariableCount = 4;

    public Text speedText; // Reference to the UI Text element

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //speedText.text = "Speed Variable Count: " + speedVariableCount;
        int count = SpeedVariableCount.speedVariableCount;
        Debug.Log("Speed Variable Count: " + count);
        if (speedText != null)
        {
            speedText.text = "Speed Stage: " + speedVariableCount;
        }
        else
        {
            Debug.LogWarning("Speed Text reference is not set!");
        }
    }

}
