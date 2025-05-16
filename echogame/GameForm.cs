using echogame.Controls;
using echogame.Models;
using echogame.Views;
using echogame;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;

public class GameForm : Form
{
    private BallsManager ballsManager;
    private Timer gameLoopTimer;
    private DateTime lastUpdateTime;

    private int cellWidth = 50;
    private int cellHeight = 50;

    private LevelManager levelManager;

    private Player player;
    private Darkness darkness;

    public GameForm()
    {
        InitializeGameComponents();
        InitializeSounds();

        this.DoubleBuffered = true;
        this.ClientSize = new Size(800, 600);
        this.BackColor = Color.Black;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        this.ControlBox = true; 
        this.MaximizeBox = false; 
        this.MinimizeBox = true; 


        gameLoopTimer = new Timer { Interval = 1 }; // ~60 fps
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

        levelManager = new LevelManager(new List<Level>() { zeroLevel ,firstLevel });

        var playerModel = new PlayerModel(zeroLevelModel, new Point(100, 100), 1, 1, 10, 10);
        var playerView = new PlayerView(playerModel);
        var playerController = new PlayerController(playerModel, playerView);

        player = new Player(playerModel, playerView, playerController);

        var darknessModel = new DarknessModel(player.Model);
        var darknessView = new DarknessView(darknessModel, "../../Textures/darkness_mask.png");
        darkness = new Darkness(darknessModel, darknessView, new DarknessController());

        ballsManager = new BallsManager(playerModel, levelManager);
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
            Console.WriteLine("balls created");
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
        levelManager.Draw(e.Graphics);
        player.View.Draw(e.Graphics);
        darkness.View.Draw(e.Graphics);
        ballsManager.DrawAllBalls(e.Graphics);
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