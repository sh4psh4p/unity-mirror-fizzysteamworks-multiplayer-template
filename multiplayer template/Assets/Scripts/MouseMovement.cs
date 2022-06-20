using UnityEngine;
using Mirror;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public NetworkIdentity networkidentity;

    float xRotation = 0f;
    Transform playerBody;

    void Start()
    {
        if (networkidentity.isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerBody = transform.root;
        }

        else
        {
            gameObject.transform.GetComponent<Camera>().enabled = false;
            gameObject.transform.GetComponent<AudioListener>().enabled = false;
            gameObject.transform.tag = "Untagged";
        }
    }

    void Update()
    {
        if (networkidentity.isLocalPlayer)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}