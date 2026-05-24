using SnakeGame.Controller;
using SnakeGame.Model;

namespace SnakeGame;

public class MainForm : Form
{
    private const int CellSize = 25;
    private const int PanelSize = GameModel.FieldSize * CellSize;

    private readonly GameModel model;
    private readonly GameController controller;

    private readonly DoubleBufferedPanel gamePanel;
    private readonly Label scoreLabel;
    private readonly Button restartButton;

    public MainForm()
    {
        model = new GameModel();

        Text = "Snake Game";
        ClientSize = new Size(PanelSize + 40, PanelSize + 95);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(30, 30, 30);
        KeyPreview = true;

        scoreLabel = CreateScoreLabel();
        restartButton = CreateRestartButton();
        gamePanel = CreateGamePanel();

        Controls.Add(scoreLabel);
        Controls.Add(restartButton);
        Controls.Add(gamePanel);

        controller = new GameController(model, this);

        KeyDown += MainForm_KeyDown;
        restartButton.Click += RestartButton_Click;

        RefreshGame();
    }

    public void RefreshGame()
    {
        scoreLabel.Text = $"Счет: {model.Score}";
        gamePanel.Invalidate();
    }

    private Label CreateScoreLabel()
    {
        return new Label
        {
            Text = "Счет: 0",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(20, 15),
            AutoSize = true
        };
    }

    private Button CreateRestartButton()
    {
        return new Button
        {
            Text = "Новая игра",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Location = new Point(PanelSize - 110, 12),
            Size = new Size(130, 34),
            BackColor = Color.FromArgb(55, 55, 55),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            TabStop = false
        };
    }

    private DoubleBufferedPanel CreateGamePanel()
    {
        DoubleBufferedPanel panel = new()
        {
            Location = new Point(20, 60),
            Size = new Size(PanelSize, PanelSize),
            BackColor = Color.FromArgb(18, 18, 18),
            BorderStyle = BorderStyle.FixedSingle,
            TabStop = false
        };

        panel.Paint += GamePanel_Paint;

        return panel;
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        controller.HandleKeyDown(e.KeyCode);
    }

    private void RestartButton_Click(object? sender, EventArgs e)
    {
        controller.RestartGame();
        ActiveControl = null;
    }

    private void GamePanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics graphics = e.Graphics;

        DrawBackground(graphics);
        DrawFood(graphics);
        DrawSnake(graphics);

        if (model.Status == GameStatus.GameOver)
        {
            DrawGameOver(graphics);
        }
    }

    private static void DrawBackground(Graphics graphics)
    {
        using Pen gridPen = new(Color.FromArgb(35, 35, 35));

        for (int i = 0; i <= GameModel.FieldSize; i++)
        {
            int position = i * CellSize;
            graphics.DrawLine(gridPen, position, 0, position, PanelSize);
            graphics.DrawLine(gridPen, 0, position, PanelSize, position);
        }
    }

    private void DrawFood(Graphics graphics)
    {
        using Brush foodBrush = new SolidBrush(Color.FromArgb(220, 70, 70));

        Rectangle foodRectangle = new(
            model.Food.X * CellSize + 4,
            model.Food.Y * CellSize + 4,
            CellSize - 8,
            CellSize - 8);

        graphics.FillEllipse(foodBrush, foodRectangle);
    }

    private void DrawSnake(Graphics graphics)
    {
        IReadOnlyList<Cell> snakeBody = model.Snake.Body;

        for (int i = 0; i < snakeBody.Count; i++)
        {
            Cell cell = snakeBody[i];

            Color color = i == 0
                ? Color.FromArgb(120, 230, 120)
                : Color.FromArgb(70, 180, 90);

            using Brush snakeBrush = new SolidBrush(color);

            Rectangle snakeRectangle = new(
                cell.X * CellSize + 2,
                cell.Y * CellSize + 2,
                CellSize - 4,
                CellSize - 4);

            graphics.FillRectangle(snakeBrush, snakeRectangle);
        }
    }

    private static void DrawGameOver(Graphics graphics)
    {
        using Brush backgroundBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        using Brush textBrush = new SolidBrush(Color.White);
        using Font titleFont = new("Segoe UI", 28, FontStyle.Bold);
        using Font hintFont = new("Segoe UI", 12, FontStyle.Regular);

        graphics.FillRectangle(backgroundBrush, 0, 0, PanelSize, PanelSize);

        string title = "GAME OVER";
        string hint = "Нажмите «Новая игра»";

        SizeF titleSize = graphics.MeasureString(title, titleFont);
        SizeF hintSize = graphics.MeasureString(hint, hintFont);

        graphics.DrawString(
            title,
            titleFont,
            textBrush,
            (PanelSize - titleSize.Width) / 2,
            PanelSize / 2 - 45);

        graphics.DrawString(
            hint,
            hintFont,
            textBrush,
            (PanelSize - hintSize.Width) / 2,
            PanelSize / 2 + 10);
    }
}

public class DoubleBufferedPanel : Panel
{

    public DoubleBufferedPanel()
    {
        DoubleBuffered = true;
    }
}
