using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public RectTransform letter1display;
    public RectTransform letter2display;
    public RectTransform letter3display;
    public RectTransform letter4display;
    public Canvas canvas;

    public Vector3 letter1pos;
    public Vector3 letter2pos;
    public Vector3 letter3pos;
    public Vector3 letter4pos;

    private void Start()
    {

        float x = (float)(FindObjectOfType<Board>().GetComponent<Board>().width-1) / 2;
        float y = (float)(FindObjectOfType<Board>().GetComponent<Board>().height / 2)+1;
        
        transform.position = new Vector3(x,y, transform.position.z);
        gameObject.GetComponent<Camera>().orthographicSize = ((FindObjectOfType<Board>().GetComponent<Board>().width)+ (FindObjectOfType<Board>().GetComponent<Board>().height) ) / 2 ;

        SetLetterPos();
    }

    public Vector3 GetWorldPositionFromOverlay(RectTransform rectTransform, Canvas canvas)
    {
        // Canvas'ın RectTransform'unu al
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // UI elementinin dünya pozisyonunu Canvas üzerindeki oranlara göre hesapla
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 pivotOffset = canvasRect.pivot * canvasSize;
        Vector2 anchoredPosition = rectTransform.anchoredPosition + pivotOffset;

        // Dünya pozisyonunu hesapla
        Vector3 worldPosition = new Vector3(anchoredPosition.x, anchoredPosition.y, 0);
        return worldPosition;
    }
    void SetLetterPos()
    {
        letter1pos = GetWorldPositionFromOverlay(letter1display, canvas);
        letter2pos = GetWorldPositionFromOverlay(letter2display, canvas);
        letter3pos = GetWorldPositionFromOverlay(letter3display, canvas);
        letter4pos = GetWorldPositionFromOverlay(letter4display, canvas);
    }

}
