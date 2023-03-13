using UnityEngine;
/// <summary>
/// attach this script to the camera you want to flip
/// </summary>
public class FlipCamera : MonoBehaviour
{

    private Camera cam;

    /// <summary>
    /// try to flip if Camera class exists, else destroy this instance
    /// </summary>
    private void Start()
    {
        if (!TryGetComponent<Camera>(out cam))
        {
            Destroy(this);
        }
    }

    void OnPreCull()
    {
        cam.ResetWorldToCameraMatrix();
        cam.ResetProjectionMatrix();
        cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }

    void OnPreRender()
    {
        GL.invertCulling =true;
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }

}