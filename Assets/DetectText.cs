using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class DetectText : MonoBehaviour
{
    public LoginHandlerScript loginHandler;
    public bool Password;
    public TMP_InputField field;
    void Start()
    {
    }
    public void displayText()
    {
        string textInField = field.text;
        if (Password)
        {
            loginHandler.password = textInField;
            UnityEngine.Debug.Log(textInField);
        }
        else
        {
            loginHandler.username = textInField;
            UnityEngine.Debug.Log(textInField);
        }
    }
}
