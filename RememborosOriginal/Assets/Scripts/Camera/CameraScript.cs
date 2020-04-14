using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Variables
    #region Public Variables
    // GridGraph's dimensions
    public static int GraphWidth;
    public static int GraphHeight;
    #endregion
    #region Private Variables
    private static Camera mainCamera; // Our camera component
    #endregion
    #endregion

    void Start()
    {
        mainCamera = GetComponentInParent<Camera>();
    }

    void Update()
    {
        SetGridSize();
    }

    #region Functions
    #region Public Functions
    public static Vector2 GetCameraLowerBounds()
    {
        Vector2 cameraBounds = new Vector2(mainCamera.transform.position.x - (mainCamera.orthographicSize * 2), mainCamera.transform.position.y - (mainCamera.orthographicSize * 2));
        return cameraBounds;
    }
    public static void SetGridSize()
    {
        GraphWidth = (int)mainCamera.transform.position.x + Mathf.CeilToInt(mainCamera.orthographicSize * 2f);
        GraphHeight = (int)mainCamera.transform.position.y + Mathf.CeilToInt(mainCamera.orthographicSize * 2f);
    }
    #endregion
    #region Private Functions
    #endregion
    #endregion
}