using echogame.Controls;
using echogame.Models;
using echogame.Views;
using echogame;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;
// ... добить чтобы фонарь пропадал после столкновения с игроком в lightermodel
public class GameForm : Form
{
    private const int WIDTH = 800;
    private const int HEIGHT = 600;

    private BallsManager ballsManager;
    private Timer gameLoopTimer;
    private DateTime lastUpdateTime;

    private int cellWidth = 50;
    private int cellHeight = 50;

    private LevelManager levelManager;

    private Player player;
    private DarknessBM darkness;

    private ComboBox darknessStateComboBox;

    public GameForm()
    {
        InitializeGameComponents();
        InitializeSounds();

        this.DoubleBuffered = true;
        this.BackColor = Color.Black;
        this.KeyPreview = true; // Это важно - форма будет получать события клавиатуры первой

        gameLoopTimer = new Timer { Interval = 1 };
        lastUpdateTime = DateTime.Now;

        gameLoopTimer.Tick += (s, e) => OnTimerTick(s, e);
        gameLoopTimer.Start();
    }

    protected void InitializeGameComponents()
    {
        var zeroLevelModel = new LevelModel("../../Levels/fullLevel.txt", cellWidth, cellHeight);
        var zeroLevelView = new LevelView(zeroLevelModel);
        var zeroLevel = new Level(zeroLevelModel, zeroLevelView, 0, new PointF(50, 600));

        var firstLevelModel = new LevelModel("../../Levels/manyCollisionsLevel.txt", cellWidth, cellHeight);
        var firstLevelView = new LevelView(firstLevelModel);
        var firstLevel = new Level(firstLevelModel, firstLevelView, 1, new PointF(50, 50));

        levelManager = new LevelManager(new List<Level>() { zeroLevel, firstLevel });

        var playerModel = new PlayerModel(zeroLevelModel, new Point(100, 100), 1, 1, 10, 10);
        var playerView = new PlayerView(playerModel);
        var playerController = new PlayerController(playerModel, playerView);

        player = new Player(playerModel, playerView, playerController);

        ballsManager = new BallsManager(playerModel, levelManager);

        var darknessModel = new DarknessBMModel(new Size(WIDTH, HEIGHT), playerModel, ballsManager, 50);
        var darknessView = new DarknessBMView(playerModel, darknessModel);
        darkness = new DarknessBM(darknessModel, darknessView, null);

        var text = new TextBox();
        text.Location = new Point(0, 0);
        text.Text = "Темнота";
        text.Font = new Font("Times New Roman", 12);
        text.ReadOnly = true;
        text.BorderStyle = BorderStyle.None;
        text.BackColor = Color.Black;
        text.ForeColor = Color.White;

        darknessStateComboBox = new ComboBox();
        darknessStateComboBox.Location = new Point(0, 20);
        darknessStateComboBox.Size = new Size(100, 100);
        darknessStateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        darknessStateComboBox.Items.Add("Выключена");
        darknessStateComboBox.Items.Add("Включена");
        darknessStateComboBox.SelectedIndexChanged += OnDarknessBoxChanged;
        darknessStateComboBox.SelectedIndex = 1;

        // Важные настройки для корректной работы управления
        darknessStateComboBox.TabStop = false;
        darknessStateComboBox.KeyDown += (sender, e) => {
            // Передаем все события клавиатуры форме
            OnKeyDown(e);
        };

        this.Controls.Add(darknessStateComboBox);
        this.Controls.Add(text);
    }

    private void OnDarknessBoxChanged(object sender, EventArgs e)
    {
        bool newState = Convert.ToBoolean(darknessStateComboBox.SelectedIndex);
        darkness.Model.IsActive = newState;

        if (newState)
        {
            // При включении темноты - полная перерисовка
            darkness.Model.ForceRedraw();
        }
        else
        {
            // При выключении - очищаем маску (если нужно)
            darkness.Model.ClearBitmap();
        }

        Invalidate(); // Принудительная перерисовка формы
    }

    protected void InitializeSounds()
    {
        SoundEngine.PreloadSound("ball_hit", "../../Sounds/ball_hit.wav");
    }

    protected void OnTimerTick(object sender, EventArgs evt)
    {
        var currentTime = DateTime.Now;
        var deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
        lastUpdateTime = currentTime;

        deltaTime = Math.Min(deltaTime, 0.1f);

        ballsManager.Update(deltaTime);
        Invalidate();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        player.Controller.HandleInput(e.KeyCode);
        if (e.KeyCode == Keys.Escape)
        {
            levelManager.ChangeLevelByNum(1);
        }
        else if (e.KeyCode == Keys.Space)
        {
            ballsManager.CreateBullet(100, 100);
        }
        else if (e.KeyCode == Keys.Delete)
        {
            Console.WriteLine("balls deleted");
            ballsManager.Clear();
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        levelManager.Draw(e.Graphics);
        player.View.Draw(e.Graphics);
        ballsManager.DrawAllBalls(e.Graphics);
        LightersManager.DrawAllLighters(e.Graphics);
        if (darkness.Model.IsActive)
            darkness.View.Draw(e.Graphics);
        e.Graphics.DrawEllipse(new Pen(Brushes.Red),
            new Rectangle(LightersManager.GetActiveLighters().First().Model.Position, new Size(50, 50)));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            gameLoopTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}