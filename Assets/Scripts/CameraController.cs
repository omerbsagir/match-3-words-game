using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // her level için kamera değerleri belirle ona göre ayarla

    private void Start()
    {
        SetCamera();

    }
    public void SetCamera()
    {
        CameraDetails cd = LevelManager.Instance.GetCameraDetails();
        transform.position = new Vector3(cd.x, cd.y, transform.position.z);
        gameObject.GetComponent<Camera>().orthographicSize = cd.ortoSize;
    }

}
