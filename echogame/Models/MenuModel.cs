using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace echogame.Models
{
    public class MenuModel
    {
        public Point BgPosition { get; private set; }
        public Size BgSize { get; private set; }
        public Point StartButtonPosition { get; private set; }
        public Size StartButtonSize { get; private set; } = new Size(150, 75);
        public Point LogoPosition { get; private set; }
        public Size LogoSize { get; private set; } = new Size(600, 300);
        public bool IsActive { get; private set; } = true;

        public readonly string BackgroundImgPath = "../../Textures/menu_background.jpeg";
        public readonly string StartBtnImgPath = "../../Textures/darkstartbtn_bg.png";
        public readonly string EchoLogoImgPath = "../../Textures/echologo.png";

        public Button StartButton { get; private set; }
        private Action makeGameControlsVisible;

        public MenuModel(Size menuSize, Action makeGameControlsVisible)
        {
            BgPosition = new Point(0, 0);
            BgSize = new Size(1550, 900);

            StartButtonPosition = new Point(BgSize.Width / 2 - StartButtonSize.Width / 2, 
                BgSize.Height / 2 - StartButtonSize.Height / 2);
           

            LogoPosition = new Point(BgSize.Width / 2 - LogoSize.Width / 2, 100);
            this.makeGameControlsVisible = makeGameControlsVisible;
            InitializeStartButton();
        }
        public void InitializeControls(Control.ControlCollection controls)
        {
            controls.Add(StartButton);
        }

        public void InitializeStartButton()
        {
            StartButton = new Button();
            StartButton.Location = StartButtonPosition;
            StartButton.Size = StartButtonSize;

            StartButton.BackgroundImage = new Bitmap(StartBtnImgPath);
            StartButton.BackgroundImageLayout = ImageLayout.Stretch;
            StartButton.Text = "";
            StartButton.Visible = true;
            // Убираем все рамки и стили
            StartButton.FlatStyle = FlatStyle.Flat;
            StartButton.FlatAppearance.BorderSize = 0;
            StartButton.BackColor = Color.Transparent;
            StartButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            StartButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            StartButton.Click += OnStartBtnClick;
        }

        public void OnStartBtnClick(object s, EventArgs e)
        {
            Console.WriteLine("changed");
            StartButton.Visible = false;
            IsActive = false;
            makeGameControlsVisible();
        }

        public void Dispose()
        {
            StartButton?.BackgroundImage?.Dispose();
        }
    }
}
