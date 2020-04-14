using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Variables
    #region Public Variables
    #endregion
    #region Private Variables
    private static Camera mainCamera; // Our camera component
    #endregion
    #endregion

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        mainCamera.transform.position = new Vector3(0,0, mainCamera.transform.position.z);
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + (mainCamera.orthographicSize * 1.7f), mainCamera.transform.position.y + (mainCamera.orthographicSize), mainCamera.transform.position.z);
    }

    void Update()
    {
        
    }

    #region Functions
    #region Public Functions
    public static Vector2 GetCameraLowerBounds()
    {
        Vector2 cameraBounds = new Vector2(mainCamera.transform.position.x - (mainCamera.orthographicSize * 1.7f), mainCamera.transform.position.y - (mainCamera.orthographicSize));
        cameraBounds.x = Mathf.Clamp(cameraBounds.x, 0, int.MaxValue);
        cameraBounds.y = Mathf.Clamp(cameraBounds.y, 0, int.MaxValue);
        return cameraBounds;
    }
    #endregion
    #region Private Functions
    #endregion
    #endregion
}
