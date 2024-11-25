using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    

    private void Start()
    {
        
        Board board = FindObjectOfType<Board>();

        if (board != null)
        {
            float x = (board.width - 1) / 2f;
            float y = (board.height / 2f) + 1f;

            transform.position = new Vector3(x, y, transform.position.z);
            gameObject.GetComponent<Camera>().orthographicSize = (board.width + board.height) / 2f;
        }

        
    }

}
