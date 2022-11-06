using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Google.Maps.Coord;
using Google.Maps.Event;
using Google.Maps.Examples.Shared;
using UnityEngine;
using TMPro;
public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }
    public float latitude;
    public float longitude;
    public float test_lat;
    public float test_long;
    public bool test;
    public GameObject coordinateTextGameObject;
    public TMP_Text coordinateText;
    [SerializeField] public LatLng _latLng;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        coordinateText = coordinateTextGameObject.GetComponent<TMP_Text>();
        DontDestroyOnLoad(gameObject);
        if (!test)
        {
            StartCoroutine(StartLocationService());
        }
        else
        {
            _latLng = new LatLng(test_lat, test_long);
        }
    }

        // Update is called once per frame
        void Update()
    {
        if (!test)
        {
            _latLng = new LatLng(latitude, longitude);
        }
        coordinateText.text = "latitude: " + latitude.ToString() + "\nlongitude: " + longitude.ToString();
    }
    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            UnityEngine.Debug.Log("User not enabled GPS");
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait <= 0)
        {
            UnityEngine.Debug.Log("Timed out");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            UnityEngine.Debug.Log("Unable to determine device location");
            yield break;
        }
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
    }
}