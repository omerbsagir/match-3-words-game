using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Adjust the position according to board dimensions

    private void Start()
    {
        float x = FindObjectOfType<Board>().GetComponent<Board>().width / 2;
        float y = FindObjectOfType<Board>().GetComponent<Board>().height / 2;

        transform.position = new Vector3(x,y, transform.position.z);
        gameObject.GetComponent<Camera>().orthographicSize = (x + y)+1;
    }

}
