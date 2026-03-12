using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f,0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;
    public SquareTextureData squareTextureData;
    public List<int[]> lastCanCompletedLine;
    public int comboIndex = 0;
    public int comboChecker = 0;


    private LineIndicator _lineIndicator;

    private Vector2 _offSet = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();

    private Config.SquareColor currentActiveSquareColor_ = Config.SquareColor.NotSet;

    
    void Start()
    {
        _lineIndicator = this.GetComponent<LineIndicator>();
        CreateGrid();
        currentActiveSquareColor_ = squareTextureData.activeSquareTextures[0].squareColor;
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanPlaced += CheckIfShapeCanPlaced;
        GameEvents.CheckIfAnyLineCanCompeleted += CheckIfAnyLineCanCompleted;
        GameEvents.UncheckIfAnyLineCanCompeleted += UncheckIfAnyLineCanCompleted;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanPlaced -= CheckIfShapeCanPlaced;
        GameEvents.CheckIfAnyLineCanCompeleted -= CheckIfAnyLineCanCompleted;
        GameEvents.UncheckIfAnyLineCanCompeleted -= UncheckIfAnyLineCanCompleted;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        currentActiveSquareColor_ = color;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;

        for(var row = 0; row < rows; row++)
        {
            for(var column = 0; column < columns; column++)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                square_index++;
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offSet.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offSet.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach(GameObject square in _gridSquares)
        {
            if(column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                //go to the next column;
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var pos_x_offset = _offSet.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offSet.y * row_number + (square_gap_number.y * squaresGap);

            if(column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset,
                    startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset,
                    startPosition.y - pos_y_offset, 0.0f);

            column_number++;
        }
    }

    private void CheckIfShapeCanPlaced()
    {
        var squareIndexes = new List<int>();
        foreach(var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                //gridSquare.ActivateSquare();
                squareIndexes.Add(gridSquare.SquareIndex);
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();


        if (currentSelectedShape == null)
            return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach(var index in squareIndexes)
            {
                _gridSquares[index].GetComponent<GridSquare>().PlaceShapeOnBoard(currentActiveSquareColor_);
            }

            int shapeLeft = 0;

            foreach(var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }


            if(shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineCompleted();

        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }

    }

    private void CheckIfAnyLineCompleted()
    {
        List<int[]> lines = new List<int[]>();

        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for(int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for(int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        for(int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>();
            for(int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.square_data[square, index]);
            }
            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if(completedLines > 0)
        {
            comboIndex += completedLines;
            comboChecker = 0;
            if(comboIndex > 1 && comboChecker < 2)
            {
                GameEvents.ComboActivate(comboIndex);
            }
        }
        else if(completedLines == 0 && comboChecker < 2)
        {
            comboChecker++;
            if(comboChecker == 2)
            {
                comboIndex = 0;
            }
        }

        var totalScores = 10 * completedLines;

        GameEvents.AddScores(totalScores);

        CheckIfPlayerLost();

    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach(var line in data)
        {
            var lineCompleted = true;
            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if(comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }

        }

        foreach(var line in completedLines)
        {
            var completed = false;

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.PlayExplodeEffect();
                completed = true;
            }
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckIfAnyLineCanCompleted(Config.SquareColor color)
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                //gridSquare.ActivateSquare();
                squareIndexes.Add(gridSquare.SquareIndex);
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();

        if (currentSelectedShape == null)
            return;


        if (currentSelectedShape.TotalSquareNumber != squareIndexes.Count)
        {
            return;
        }
        
        List<int[]> lines = new List<int[]>();

        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        for (int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>();
            for (int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.square_data[square, index]);
            }
            lines.Add(data.ToArray());
        }

        List<int[]> canCompletedLines =  CheckIfSquareCanCompleted(lines);

        lastCanCompletedLine = canCompletedLines;


        foreach (var line in canCompletedLines)
        {
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponentInChildren<ActiveSquareImageSelector>();
                if (comp != null)
                {
                    comp.ChangeSquareColorBaseOnLineCanCompleted(color);
                }
            }
        }
    }

    private List<int[]> CheckIfSquareCanCompleted(List<int[]> data)
    {
        List<int[]> canCompletedLines = new List<int[]>();

        foreach(var line in data)
        {
            bool lineCanCompleted = true;

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if(comp.SquareOccupied == false && comp.Selected == false)
                {
                    lineCanCompleted = false;
                }
            }

            if (lineCanCompleted)
            {
                canCompletedLines.Add(line);
            }
        }

        return canCompletedLines;      
    }

    private void UncheckIfAnyLineCanCompleted()
    {
        if (lastCanCompletedLine == null) return;
        List<int[]> canCompletedLines = lastCanCompletedLine;
        foreach(var line in canCompletedLines)
        {
            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponentInChildren<ActiveSquareImageSelector>();
                
                if (comp != null)
                {
                    Config.SquareColor lastColor = _gridSquares[squareIndex].GetComponent<GridSquare>().GetCurrentColor();
                    comp.ChangeSquareColorBaseOnLineCanCompleted(lastColor);
                }
            }
        }
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;
        for(var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            if(CheckIfShapeCanBePlaceOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActiveShape();
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            GameEvents.GameOver(false);
            Debug.Log("Game Over");
        }
    }

    private bool CheckIfShapeCanBePlaceOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for(var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);

                }
                squareIndex++;
            }

        }

        if(currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Number of filled up squares are not the same as the original shape have.");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach(var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach(var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        var safeIndex = 0;
        
        while(lastRowIndex + (rows - 1) < 9)
        {
            var rowData = new List<int>();
            for(var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for(var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if(lastColumnIndex + (columns - 1) >= 9)
            {
                lastColumnIndex = 0;
                lastRowIndex++;
            }

            safeIndex++;

            if(safeIndex > 100)
            {
                break;
            }

        }

        return squareList;

    }

}
