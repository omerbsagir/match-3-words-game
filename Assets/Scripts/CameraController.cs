using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // her level için kamera değerleri belirle ona göre ayarla

    private void Start()
    {
        float width = LevelManager.Instance.GetLevelDimensions()[0];
        float height = LevelManager.Instance.GetLevelDimensions()[1];

        float x = (width - 1) / 2f;
        float y = (height / 2f) + 1f;

        transform.position = new Vector3(x, y, transform.position.z);
        gameObject.GetComponent<Camera>().orthographicSize = (width + height) / 2f;

    }

}
