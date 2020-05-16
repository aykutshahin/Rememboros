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
    //private BoxCollider2D cameraBox;
    [SerializeField] Player player = null;
    #endregion
    #endregion

    void Start()
    {
        mainCamera = GetComponentInParent<Camera>();
    }

  
    private void Update()
    {
        //SetGridSize();
    }


    private void LateUpdate()
    {
        
    }


    #region Functions
    #region Public Functions
   /* public static Vector2 GetCameraLowerBounds()
    {
        if(mainCamera != null)
        {
            Vector2 cameraBounds = new Vector2(mainCamera.transform.position.x - (mainCamera.orthographicSize * 2), mainCamera.transform.position.y - (mainCamera.orthographicSize * 2));
            return cameraBounds;
        }
        return Vector2.zero;
    }
    public static void SetGridSize()
    {
        //TO DO  Getgrid map
        GraphWidth = (int)mainCamera.transform.position.x + Mathf.CeilToInt(mainCamera.orthographicSize * 2f);
        GraphHeight = (int)mainCamera.transform.position.y + Mathf.CeilToInt(mainCamera.orthographicSize * 2f);
    }*/
    #endregion
    #region Private Functions

    #endregion
    #endregion

    #region Boundary Camera Effect
    /*void AspectRatioBoxChange()
{
    //16:10 ratio
    if(Camera.main.aspect >= 1.6f && Camera.main.aspect < 1.7f)
    {
        cameraBox.size = new Vector2(12f, 7.43f);
    }
    //16:9 ratio
    if (Camera.main.aspect >= 1.7f && Camera.main.aspect < 1.8f)
    {
        cameraBox.size = new Vector2(13.29f, 7.43f);
    }
    //5:4 ratio
    if (Camera.main.aspect >= 1.25f && Camera.main.aspect < 1.3f)
    {
        cameraBox.size = new Vector2(9.4f, 7.43f);
    }
    //4:3 ratio
    if (Camera.main.aspect >= 1.3f && Camera.main.aspect < 1.4f)
    {
        cameraBox.size = new Vector2(9.94f, 7.43f);
    }
    //3:2 ratio
    if (Camera.main.aspect >= 1.5f && Camera.main.aspect < 1.6f)
    {
        cameraBox.size = new Vector2(11.21f, 7.43f);
    }

}

void FollowPlayer()
{
    if (GameObject.Find("Boundary"))
    {
        transform.position = new Vector3(Mathf.Clamp(player.position.x, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.x + cameraBox.size.x / 2, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.x - cameraBox.size.x / 2),
                                                                Mathf.Clamp(player.position.y, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.y + cameraBox.size.y / 2, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.y - cameraBox.size.y / 2),
                                                                transform.position.z);
    }
}*/
    #endregion
}