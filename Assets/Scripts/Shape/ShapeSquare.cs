using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;

    private void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }
}
