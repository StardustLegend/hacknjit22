using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginHandlerScript : MonoBehaviour
{
    public string url;
    public string username;
    public string password;
    public TMP_Text login_text;
    public string NextScene;

    public void Request()
    {
        StartCoroutine(Upload());
    }
    IEnumerator Upload()
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityEngine.Debug.Log("username received: " + username);
        UnityEngine.Debug.Log("password received: " + password);
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        UnityEngine.Debug.Log(request.result.ToString());
        UnityEngine.Debug.Log(request.downloadHandler.text);
        if (request.downloadHandler.text == "YOU ARE IN")
        {
            SceneManager.LoadScene(NextScene);
        }
        else
        {
            login_text.text = "Incorrect Username or Password: Please Try Again";
        }
    }
}
