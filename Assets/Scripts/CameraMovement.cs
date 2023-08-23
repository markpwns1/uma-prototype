using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Global mouse sensitivity modifier, to be changed in the options menu
    public static float sensitivity = 1.0f;
    
    [Header("References")]
    // The object to be rotated horizontally
    public GameObject player;
    
    // The object to be rotated vertically
    public new GameObject camera;
    
    [Header("Settings")]
    // Relative to the global sensitivity
    [Range(0.01f, 10f)]
    public float xSensitivityMult = 1;
    [Range(0.01f, 10f)]
    public float ySensitivityMult = 1;

    // The maximum angle the camera can look up or down, in degrees, to prevent
    // the camera from flipping over
    public float yLimit = 90;

    // Buffer for the camera rotation, to avoid setting it directly, which gets wacky
    // due to Unity automatically changing coordinates when the rotation goes over 180 degrees
    private Vector2 _virtualEulerAngles;
    
    public void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        if (!camera && GetComponent<Camera>()) camera = GetComponent<Camera>().gameObject;
    }

    public void Update()
    {
        // If no UI is open, allow the camera to be moved
        if (MouseLockManager.IsMouseLocked)
            _virtualEulerAngles += new Vector2(Input.GetAxisRaw("Mouse X") * xSensitivityMult * sensitivity, -Input.GetAxisRaw("Mouse Y") * ySensitivityMult * sensitivity);
        
        // Clamp the X axis rotation to prevent the camera from flipping over
        _virtualEulerAngles = new Vector2(_virtualEulerAngles.x, Mathf.Clamp(_virtualEulerAngles.y, -yLimit, yLimit));

        // Set the rotation of the player (Y axis) and camera (X axis)
        
        player.transform.localRotation = Quaternion.Euler(
            player.transform.localRotation.eulerAngles.x,
            _virtualEulerAngles.x,
            player.transform.localRotation.eulerAngles.z);

        camera.transform.localRotation = Quaternion.Euler(
            _virtualEulerAngles.y,
            camera.transform.localRotation.eulerAngles.y,
            camera.transform.localRotation.eulerAngles.z);
    }
}