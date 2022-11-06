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
public class PostRequestScript : MonoBehaviour
{
    public string url;
    public TMP_Text artifact_text;
    public GPS gps;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Request()
    {
        StartCoroutine(Upload());
    }
    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("latitude", gps.latitude.ToString());
        form.AddField("longitude", gps.longitude.ToString());
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        UnityEngine.Debug.Log(request.result.ToString());
        if (request.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log("OOPS!!!! :(");
        }
        else
        {
            //UnityEngine.Debug.Log(request.downloadHandler.text);
        }
        artifact_text.text = "Aritfact Saved! :)";
        yield return new WaitForSeconds(2.5f);
        artifact_text.text = "";

    }
}
