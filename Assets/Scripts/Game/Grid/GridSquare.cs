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
    public ActiveSquareImageSelector activeSquareImageSelector;

    public Config.SquareColor currentSquareColor_ = Config.SquareColor.NotSet;
    private Config.SquareColor lastSquareColor_ = Config.SquareColor.NotSet;


    public Config.SquareColor GetCurrentColor()
    {
        return currentSquareColor_;
    }

    private Config.SquareColor GetCurrentActiveColor()
    {
        SquareTextureData currentActiveSquareTextureData = gameObject.GetComponentInParent<Grid>().squareTextureData;
        return currentActiveSquareTextureData.currentColor;
    }

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

    public void PlaceShapeOnBoard(Config.SquareColor color)
    {
        ActivateSquare();
        currentSquareColor_ = color;
        lastSquareColor_ = currentSquareColor_;
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
        currentSquareColor_ = Config.SquareColor.NotSet;
        activeImage.gameObject.SetActive(false);
        ClearOccupied();
    }

    public void ClearOccupied()
    {
        currentSquareColor_ = Config.SquareColor.NotSet;
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
            
            GameEvents.CheckIfAnyLineCanCompeleted(GetCurrentActiveColor());
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
            GameEvents.UncheckIfAnyLineCanCompeleted();
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupiedImage();
        }
    }
}
