using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool updateImageOnReachedThreshold = false;
    public bool updateImageOnLineCanCompleted = false;

    private void OnEnable()
    {
        UpdateSquareColorBaseOnCurrentScores();
        if (updateImageOnReachedThreshold)
        {
            GameEvents.UpdateSquareColor += UpdateSquareColor;
        }
        if (updateImageOnLineCanCompleted)
        {
            GameEvents.CheckIfAnyLineCanCompeleted += ChangeSquareColorBaseOnLineCanCompleted;
        }
    }

    private void OnDisable()
    {
        if (updateImageOnReachedThreshold)
        {
            GameEvents.UpdateSquareColor -= UpdateSquareColor;
        }
    }

    private void UpdateSquareColorBaseOnCurrentScores()
    {
        foreach(var squareTexture in squareTextureData.activeSquareTextures)
        {
            if(squareTextureData.currentColor == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }

    public void ChangeSquareColorBaseOnLineCanCompleted(Config.SquareColor color)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        foreach(var squareTexture in squareTextureData.activeSquareTextures)
        {
            if(color == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }

    private void UpdateSquareColor(Config.SquareColor color)
    {
        foreach(var squareTexture in squareTextureData.activeSquareTextures)
        {
            if(color == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }
}
