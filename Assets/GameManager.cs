using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static readonly int boardSize = 4;
    [SerializeField] private TMP_Text hintText;

    [SerializeField] private TMP_Text ScoreText;

    public Cell cellPrefab;
    private readonly List<List<Cell>> boardView = new();

    private int _score;

    private List<List<(int value, Color color)>> board = new();

    private int highScore;


    private bool mergeHappen;

    private int score

    {
        get => _score;
        set
        {
            if (value > highScore)
            {
                highScore = value;
                hintText.text = "New High Score: " + highScore;
            }
            else if (value > 20)
            {
                hintText.text = "You are 灰五 大師";
            }

            _score = value;
            ScoreText.text = "Score: " + _score + ", High Score: " + highScore;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        hintText.text = "Use W, A, S, D to move, R to restart, Esc to quit.";
        NewGame();
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveBoard(Direction.Up);
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBoard(Direction.Down);
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveBoard(Direction.Left);
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveBoard(Direction.Right);
            UpdateView();
        }

        if (mergeHappen)
        {
            mergeHappen = false;

            for (var i = 0; i < boardSize; i++)
            for (var j = 0; j < boardSize; j++)
                if (board[i][j].value == 6 && board[i][j].color == Color.gray)
                {
                    score += 5;
                    board[i][j] = (0, Color.blue);
                }
                else if (board[i][j].value == 6)
                {
                    hintText.text = "You have merged 5!, but the gray is " + board[i][j].color.r;
                    board[i][j] = (0, Color.blue);
                }

            UpdateView();
        }

        if (Input.GetKeyDown(KeyCode.R)) NewGame();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void NewGame()
    {
        score = 0;
        mergeHappen = false;

        foreach (var row in boardView)
        foreach (var cell in row)
            Destroy(cell.gameObject);

        boardView.Clear();
        board = new List<List<(int value, Color color)>>();
        for (var i = 0; i < boardSize; i++)
        {
            var row = new List<(int, Color)>();
            var rowView = new List<Cell>();
            for (var j = 0; j < boardSize; j++)
            {
                row.Add((0, Color.blue));
                var cell = Instantiate(cellPrefab);
                cell.transform.position = new Vector3(i, j, 0);
                rowView.Add(cell);
            }

            board.Add(row);
            boardView.Add(rowView);
        }

        GenNewNumber();
        GenNewNumber();
        UpdateView();
    }

    private void UpdateView()
    {
        for (var i = 0; i < boardSize; i++)
        for (var j = 0; j < boardSize; j++)
            boardView[i][j].SetView(board[i][j]);
    }

    private void MoveBoard(Direction direction)
    {
        var directionVector = Vector2.zero;
        switch (direction)
        {
            case Direction.Up:
                directionVector = Vector2.up;
                break;
            case Direction.Down:
                directionVector = Vector2.down;
                break;
            case Direction.Left:
                directionVector = Vector2.left;
                break;
            case Direction.Right:
                directionVector = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        var newBoard = new List<List<(int value, Color color)>>();
        for (var i = 0; i < boardSize; i++)
        {
            var row = new List<(int, Color)>();
            for (var j = 0; j < boardSize; j++) row.Add((0, Color.blue));

            newBoard.Add(row);
        }

        // move to left or down
        if (direction == Direction.Left || direction == Direction.Down)
            for (var x = 0; x < boardSize; x++)
            for (var y = 0; y < boardSize; y++)
                MoveCell(x, y, directionVector, newBoard);
        else
            for (var x = boardSize - 1; x >= 0; x--)
            for (var y = boardSize - 1; y >= 0; y--)
                MoveCell(x, y, directionVector, newBoard);

        board = newBoard;
        // choose a random empty cell to spawn a new number
        GenNewNumber();
    }

    private void GenNewNumber()
    {
        var emptyCells = new List<Vector2Int>();
        for (var i = 0; i < boardSize; i++)
        for (var j = 0; j < boardSize; j++)
            if (board[i][j].value == 0)
                emptyCells.Add(new Vector2Int(i, j));

        if (emptyCells.Count > 0)
        {
            if (!mergeHappen)
            {
                var randomGen = Random.Range(0, 2);
                if (randomGen == 0) return;
            }

            var randomIndex = Random.Range(0, emptyCells.Count);
            var randomCell = emptyCells[randomIndex];

            // random to gen 1 or 2
            var randomColor = Random.Range(0, 2);
            var randomValue = Random.Range(0, 2);
            var co = Color.blue;
            var v = 0;
            if (randomValue == 1)
                v = 1;
            else if (randomValue == 0) v = 2;
            if (randomColor == 0)
                co = Color.black;
            else if (randomColor == 1) co = Color.white;

            var valueTuple = board[randomCell.x][randomCell.y];
            valueTuple.value = v;
            valueTuple.color = co;
            board[randomCell.x][randomCell.y] = valueTuple;
        }
    }

    private void MoveCell(int x, int y, Vector2 directionVector, List<List<(int value, Color color)>> newBoard)
    {
        var c = board[x][y];
        if (c.value == 0) return;

        if (x + (int)directionVector.x < 0 || x + (int)directionVector.x >= boardSize ||
            y + (int)directionVector.y < 0 || y + (int)directionVector.y >= boardSize)
        {
            newBoard[x][y] = c;
            return;
        }

        var nextValue = newBoard[x + (int)directionVector.x][y + (int)directionVector.y];
        if (nextValue.value != 0)
        {
            if (nextValue.value == c.value)
            {
                newBoard[x + (int)directionVector.x][y + (int)directionVector.y] =
                    (c.value + 1, (c.color + nextValue.color) / 2);
                mergeHappen = true;
            }
            else
            {
                newBoard[x][y] = c;
            }
        }
        else
        {
            newBoard[x + (int)directionVector.x][y + (int)directionVector.y] = c;
        }
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}