using SnakeGame.Model;

namespace SnakeGame.Controller;

public class GameController
{
    private readonly GameModel model;
    private readonly MainForm view;
    private readonly System.Windows.Forms.Timer timer;

    public GameController(GameModel model, MainForm view)
    {
        this.model = model;
        this.view = view;

        timer = new System.Windows.Forms.Timer();
        timer.Interval = 120;
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    public void HandleKeyDown(Keys key)
    {
        if (model.Status == GameStatus.GameOver)
        {
            return;
        }

        switch (key)
        {
            case Keys.W:
                model.ChangeDirection(Direction.Up);
                break;

            case Keys.S:
                model.ChangeDirection(Direction.Down);
                break;

            case Keys.A:
                model.ChangeDirection(Direction.Left);
                break;

            case Keys.D:
                model.ChangeDirection(Direction.Right);
                break;
        }
    }

    public void RestartGame()
    {
        model.StartNewGame();
        timer.Start();
        view.RefreshGame();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        model.Update();
        view.RefreshGame();

        if (model.Status == GameStatus.GameOver)
        {
            timer.Stop();
        }
    }
}
