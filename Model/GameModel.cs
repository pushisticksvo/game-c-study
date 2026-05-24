namespace SnakeGame.Model;

public class GameModel
{
    private readonly Random random = new();

    public const int FieldSize = 20;

    public GameModel()
    {
        StartNewGame();
    }

    public Snake Snake { get; private set; } = null!;

    public Cell Food { get; private set; }

    public Direction CurrentDirection { get; private set; }

    public int Score { get; private set; }

    public GameStatus Status { get; private set; }

    public void StartNewGame()
    {
        int center = FieldSize / 2;

        Snake = new Snake(new[]
        {
            new Cell(center, center),
            new Cell(center - 1, center),
            new Cell(center - 2, center)
        });

        CurrentDirection = Direction.Right;
        Score = 0;
        Status = GameStatus.Playing;

        SpawnFood();
    }

    public void ChangeDirection(Direction newDirection)
    {
        if (IsOppositeDirection(CurrentDirection, newDirection))
        {
            return;
        }

        CurrentDirection = newDirection;
    }

    public void Update()
    {
        if (Status != GameStatus.Playing)
        {
            return;
        }

        Cell newHead = GetNextHead();
        bool willEatFood = newHead == Food;

        if (IsOutsideField(newHead) || IsCollisionWithSnake(newHead, willEatFood))
        {
            Status = GameStatus.GameOver;
            return;
        }

        Snake.Move(newHead, willEatFood);

        if (willEatFood)
        {
            Score++;
            SpawnFood();
        }
    }

    private Cell GetNextHead()
    {
        Cell head = Snake.Head;

        return CurrentDirection switch
        {
            Direction.Up => new Cell(head.X, head.Y - 1),
            Direction.Down => new Cell(head.X, head.Y + 1),
            Direction.Left => new Cell(head.X - 1, head.Y),
            Direction.Right => new Cell(head.X + 1, head.Y),
            _ => head
        };
    }

    private static bool IsOutsideField(Cell cell)
    {
        return cell.X < 0
            || cell.Y < 0
            || cell.X >= FieldSize
            || cell.Y >= FieldSize;
    }

    private bool IsCollisionWithSnake(Cell newHead, bool willEatFood)
    {
        int cellsToCheck = willEatFood
            ? Snake.Body.Count
            : Snake.Body.Count - 1;

        for (int i = 0; i < cellsToCheck; i++)
        {
            if (Snake.Body[i] == newHead)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnFood()
    {
        List<Cell> freeCells = new();

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                Cell cell = new(x, y);

                if (!Snake.Contains(cell))
                {
                    freeCells.Add(cell);
                }
            }
        }

        if (freeCells.Count == 0)
        {
            Status = GameStatus.GameOver;
            return;
        }

        Food = freeCells[random.Next(freeCells.Count)];
    }

    private static bool IsOppositeDirection(Direction first, Direction second)
    {
        return first == Direction.Up && second == Direction.Down
            || first == Direction.Down && second == Direction.Up
            || first == Direction.Left && second == Direction.Right
            || first == Direction.Right && second == Direction.Left;
    }
}
