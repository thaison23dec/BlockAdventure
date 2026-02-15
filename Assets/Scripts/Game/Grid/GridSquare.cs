using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hooverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    private void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    //public bool CanWeUsePlaceSquare()
    //{
    //    return hooverImage.gameObject.activeSelf;
    //}

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void DeactivateSquare()
    {
        activeImage.gameObject.SetActive(false);
        ClearOccupied();
    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.SquareOccupied)
        {
            this.Selected = true;
            hooverImage.gameObject.SetActive(true);
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupiedImage();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        this.Selected = true;
        if (!this.SquareOccupied)
        {
            hooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupiedImage();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!this.SquareOccupied)
        {
            this.Selected = false;
            hooverImage.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupiedImage();
        }
    }
}
