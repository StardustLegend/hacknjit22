using UnityEngine;
namespace TestApp.MobileDevice
{
    public class MobileDeviceEditorController : MonoBehaviour
    {
        float _speed = 20.0f;

        void Update()
        {
            float _step = Time.deltaTime * _speed;

            if (Input.GetKey(KeyCode.UpArrow))
                transform.Translate(transform.forward * _step, Space.World);
            if (Input.GetKey(KeyCode.DownArrow))
                transform.Translate(-transform.forward * _step, Space.World);
            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(0, -3, 0);
            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(0, 3, 0);
        }
    }
}