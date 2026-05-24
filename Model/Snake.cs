namespace SnakeGame.Model;

public class Snake
{
    private readonly List<Cell> body;

    public Snake(IEnumerable<Cell> startBody)
    {
        body = new List<Cell>(startBody);
    }

    public IReadOnlyList<Cell> Body => body;

    public Cell Head => body[0];

    public bool Contains(Cell cell)
    {
        return body.Contains(cell);
    }

    public void Move(Cell newHead, bool grow)
    {
        body.Insert(0, newHead);

        if (!grow)
        {
            body.RemoveAt(body.Count - 1);
        }
    }
}
