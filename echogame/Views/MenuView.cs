using echogame.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace echogame.Views
{
    public class MenuView : IDisposable
    {
        private readonly MenuModel model;
        
        private Bitmap background;
        private Bitmap logo;


        public MenuView(MenuModel model)
        {
            this.model = model;

            background = new Bitmap(model.BackgroundImgPath);
            logo = new Bitmap(model.EchoLogoImgPath);
        }

        public void Draw(Graphics g)
        {
            // Отрисовываем предзагруженные битмапы
            g.DrawImage(background, new Rectangle(new Point(0, 0), model.BgSize));
            g.DrawImage(logo, new Rectangle(model.LogoPosition, model.LogoSize));
        }

        public void Dispose()
        {
            background?.Dispose();
            logo?.Dispose();
        }
    }
}