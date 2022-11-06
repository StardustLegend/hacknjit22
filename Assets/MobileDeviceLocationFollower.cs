using System.Collections;
using System.Globalization;
using UnityEngine;
using Google.Maps;
using Google.Maps.Coord;
using System.Diagnostics;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
namespace TestApp.MobileDevice
{
    public class MobileDeviceLocationFollower : MonoBehaviour
    {
        [SerializeField] private MapsService _mapsService;
        [SerializeField] private LatLng _latLng;
        private bool _isPaused = false;
        private Vector3 _pausedPosition;
        private Vector3 _north;
        private void Start()
        {
            GetPermissions();
            StartCoroutine(Follow());
        }

        private IEnumerator Follow()
        {
#if UNITY_IOS
            Input.location.Start();
#endif
#if UNITY_EDITOR
            gameObject.AddComponent<MobileDeviceEditorController>();
            UnityEngine.Debug.Log( "Mocking location in editor. Origin starting at MapService Location from Map Preview Options" );
#else
            while (!Input.location.isEnabledByUser)
            {
                UnityEngine.Debug.Log("Location services not enabled..");
                yield return new WaitForSeconds(1f);
            }
            UnityEngine.Debug.Log("Location services enabled.");
#endif
#if !UNITY_IOS
            Input.location.Start();
#endif
#if !UNITY_EDITOR
            Input.compass.enabled = true;
            while (true)
            {
                if (Input.location.status == LocationServiceStatus.Initializing)
                {
                    yield return new WaitForSeconds(1f);
                }
                else if (Input.location.status == LocationServiceStatus.Failed)
                {
                    UnityEngine.Debug.LogError("Location Services failed to start.");
                    yield break;
                }
                else if (Input.location.status == LocationServiceStatus.Running)
                {
                    break;
                }
            }
            _latLng = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
#endif
            yield return new WaitWhile(() => _mapsService == null);
            _mapsService.Events.MapEvents.Loaded.AddListener(OnMapLoaded);

            _mapsService.InitFloatingOrigin(_latLng);
            _mapsService.LoadMap(
                new Bounds(Vector3.zero, new Vector3(1000, 1, 1000)),
                Google.Maps.Examples.Shared.ExampleDefaults.DefaultGameObjectOptions
            );
        }

        private void OnMapLoaded(Google.Maps.Event.MapLoadedArgs _args)
        {
            LatLng _ll = GetLatLng();
            LatLng _ll2 = new LatLng(_ll.Lat + 0.0005, _ll.Lng);
            _north = GetPosition(_ll2) - GetPosition(_ll);
            CheckIfAtWorldOrigin();
#if !UNITY_EDITOR
            StartCoroutine(UpdateDevice());
#endif
        }
        private IEnumerator UpdateDevice()
        {
            while (true)
            {
                Vector3 _newForward = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up) * _north;
                transform.rotation = Quaternion.LookRotation(_newForward, Vector3.up);
                transform.position = Position();
                yield return new WaitForEndOfFrame();
            }
        }
        private void CheckIfAtWorldOrigin()
        {
            if (DistanceFromOther(new LatLng(0.0, 0.0)) > 5)
            {
                //The user does not have location settings on.
                //This could mean that the location permission is
                //granted to the app, but the general location services is off.
                //What should we do??
            }
        }
        public float DistanceFromOther(LatLng _latLong)
        {
            return Vector3.Distance(
                GetPosition(_latLong),
                Position()
            );
        }
        public float DistanceFromOther(string _latLongStr)
        {
            string[] _latLongArr = _latLongStr.Split(',');
            return DistanceFromOther(
                new LatLng(
                    double.Parse(_latLongArr[0].Trim(), new CultureInfo("en-US")),
                    double.Parse(_latLongArr[1].Trim(), new CultureInfo("en-US"))
                )
            );
        }
        public LatLng GetLatLng()
        {
            LatLng _latLng;
#if UNITY_EDITOR
            _latLng = _mapsService.Projection.FromVector3ToLatLng(transform.position);
#else
            _latLng = new LatLng(
                Input.location.lastData.latitude,
                Input.location.lastData.longitude
            );
#endif
            return _latLng;
        }
        private Vector3 GetPosition(LatLng _latLng)
        {
            return _mapsService.Projection.FromLatLngToVector3(_latLng);
        }
        public Vector3 Position()
        {
            return GetPosition(GetLatLng());
        }
        private void OnApplicationPause(bool _pauseStatus)
        {
            if (!_isPaused && _pauseStatus)
            {
                _isPaused = true;
                _pausedPosition = transform.localPosition;
            }
            else if (_isPaused && !_pauseStatus)
            {
                StartCoroutine(CheckLocationVarianceDuringPause());
                _isPaused = false;
            }
        }
        private IEnumerator CheckLocationVarianceDuringPause()
        {
            yield return null;
            if (Vector3.Distance(_pausedPosition, Position()) > 10)
            {
                //The GPS offset is more than 10 meters from the paused position,
                //this likely happened because the user paused and came back after
                //moving the said distance
            }
        }
        private void GetPermissions()
        {
#if UNITY_ANDROID
            if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                Permission.RequestUserPermission(Permission.FineLocation);
#endif
        }
    }
}