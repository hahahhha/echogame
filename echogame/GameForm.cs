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
    private const int WIDTH = 1920;
    private const int HEIGHT = 1200;

    private BallsManager ballsManager;
    private Timer gameLoopTimer;
    private DateTime lastUpdateTime;

    private int cellWidth = 50;
    private int cellHeight = 50;

    private LevelManager levelManager;

    private Player player;
    private DarknessBM darkness;

    private ComboBox darknessStateComboBox;
    private TextBox darknessText;
    private TextBox lightersText;

    private echogame.Menu menu;
    public GameForm()
    {
        InitializeGameComponents();
        InitializeSounds();

        this.DoubleBuffered = true;
        this.BackColor = Color.Black;
        this.KeyPreview = true;
        this.Size = new Size(WIDTH, HEIGHT);

        gameLoopTimer = new Timer { Interval = 1 };
        lastUpdateTime = DateTime.Now;

        gameLoopTimer.Tick += (s, e) => OnTimerTick(s, e);
        gameLoopTimer.Start();
    }

    protected void InitializeGameComponents()
    {
        var zeroLevelModel = new LevelModel("../../Levels/1.txt", cellWidth, cellHeight, new Point(50, 50));
        var zeroLevelView = new LevelView(zeroLevelModel);
        var zeroLevel = new Level(zeroLevelModel, zeroLevelView, 0);

        var firstLevelModel = new LevelModel("../../Levels/2.txt", cellWidth, cellHeight, new Point(800, 50));
        var firstLevelView = new LevelView(firstLevelModel);
        var firstLevel = new Level(firstLevelModel, firstLevelView, 1);
        levelManager = new LevelManager(new List<Level>() { zeroLevel, firstLevel });


        var playerModel = new PlayerModel(levelManager, 1, 1, 5, 5);
        var playerView = new PlayerView(playerModel);
        var playerController = new PlayerController(playerModel, playerView);

        player = new Player(playerModel, playerView, playerController);

        ballsManager = new BallsManager(playerModel, levelManager);

        var darknessModel = new DarknessBMModel(new Size(WIDTH, HEIGHT), playerModel, ballsManager, levelManager);
        var darknessView = new DarknessBMView(playerModel, darknessModel);
        darkness = new DarknessBM(darknessModel, darknessView, null);

        var menuModel = new MenuModel(new Size(WIDTH, HEIGHT), MakeGameControlsVisible);
        var menuView = new MenuView(menuModel);
        menu = new echogame.Menu(menuModel, menuView);

        InitializeGameControls();

        menu.Model.InitializeControls(Controls);
    }

    private void InitializeLightersCounterText()
    {
        lightersText = new TextBox();
        lightersText.Location = new Point(200, 50);
        lightersText.Text = "Собрано: 0";
        levelManager.TakenLightersAmountChanged += () =>
        {
            lightersText.Text = "Собрано: " + Convert.ToString(levelManager.TakenLightersAmount);
        };
        lightersText.Visible = false;
        this.Controls.Add(lightersText);
    }

    private void InitializeDarknessComboBox()
    {
        darknessText = new TextBox();
        darknessText.Location = new Point(0, 0);
        darknessText.Text = "Темнота";
        darknessText.Font = new Font("Times New Roman", 12);
        darknessText.ReadOnly = true;
        darknessText.BorderStyle = BorderStyle.None;
        darknessText.BackColor = Color.Black;
        darknessText.ForeColor = Color.White;
        darknessText.Visible = false;

        darknessStateComboBox = new ComboBox();
        darknessStateComboBox.Location = new Point(0, 20);
        darknessStateComboBox.Size = new Size(100, 100);
        darknessStateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        darknessStateComboBox.Items.Add("Выключена");
        darknessStateComboBox.Items.Add("Включена");
        darknessStateComboBox.SelectedIndexChanged += OnDarknessBoxChanged;
        darknessStateComboBox.SelectedIndex = 1;

        darknessStateComboBox.TabStop = false;
        darknessStateComboBox.KeyDown += (sender, e) => {
            OnKeyDown(e);
        };
        darknessStateComboBox.Visible = false;

        this.Controls.Add(darknessText);
        this.Controls.Add(darknessStateComboBox);
    }
    private void InitializeGameControls()
    {
        InitializeDarknessComboBox();
        InitializeLightersCounterText();
    }

    public void MakeGameControlsVisible()
    {
        darknessStateComboBox.Visible = true;
        darknessText.Visible = true;
        lightersText.Visible = true;
    }

    private void OnDarknessBoxChanged(object sender, EventArgs e)
    {
        bool newState = Convert.ToBoolean(darknessStateComboBox.SelectedIndex);
        darkness.Model.IsActive = newState;

        if (newState)
        {
            darkness.Model.ForceRedraw();
        }
        else
        {
            darkness.Model.ClearBitmap();
        }

        Invalidate(); 
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
        //else if (e.KeyCode == Keys.Space)
        //{
        //    ballsManager.CreateBullet(100, 100);
        //}
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
        if (menu.Model.IsActive)
        {
            menu.View.Draw(e.Graphics);
        }
        else
        {
            levelManager.Draw(e.Graphics);
            player.View.Draw(e.Graphics);
            ballsManager.DrawAllBalls(e.Graphics);
            if (darkness.Model.IsActive)
                darkness.View.Draw(e.Graphics);
            e.Graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(new Point(1200, 800), new Size(50, 50)));
        }    
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