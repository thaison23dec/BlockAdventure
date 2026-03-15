using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    public List<ShapeData> shapeSmallList;
    public Sprite lastShapeSprite;
    public Grid grid;

    public GameOverPopUp gameOverPopUp;

    void Start()
    {
        foreach(var shape in shapeList)
        {
            int shapeIndex = Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            gameOverPopUp.gameOverPopUp.SetActive(false);
            RequestAllSmallShapes();
        }
    }

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach(var shape in shapeList)
        {
            if(shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }
        return null;
    }

    public void RequestNewShapes()
    {
        foreach (var shape in shapeList)
        {
            int shapeIndex = Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }

    public void RequestSmallShapes(int smallShapeIndex)
    {
        RequestNewShapes();
        int attempts = 0;
        while (attempts < 50)
        {
            int randIndex = Random.Range(0, shapeList.Count - 1);
            if (grid.CheckIfShapeDataCanBePlaceOnGrid(shapeSmallList[smallShapeIndex]))
            {
                shapeList[randIndex].RequestNewShape(shapeSmallList[smallShapeIndex]);
                break;
            }
            attempts++;
        }
    }

    public void RequestAllSmallShapes()
    {
        RequestNewShapes();
        foreach (var shape in shapeList)
        {
            int attempts = 0;
            while (attempts < 50)
            {
                int shapeIndex = Random.Range(0, shapeSmallList.Count - 1);
                if (grid.CheckIfShapeDataCanBePlaceOnGrid(shapeSmallList[shapeIndex]))
                {
                    shape.RequestNewShape(shapeSmallList[shapeIndex]);
                    break;
                }
                attempts++;
            }
        }
    }

    public void SetShapeSprite(Sprite spriteToSet)
    {
        lastShapeSprite = spriteToSet;
    }

}
