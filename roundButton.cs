using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Cshapshiyan_1
{
    public partial class roundButton : UserControl
    {
        public roundButton()
        {
            InitializeComponent();

        }
        private string txt;
        private Color bkcolor, bdcolor;
        public string Txt
        {
            get { return txt; }
            set { txt = value; }
        }
        public Color ButtonBackColor
        {
            get { return bkcolor; }
            set { bkcolor = value; }
        }
        public Color BorderColor
        {
            get { return bdcolor; }
            set { bdcolor = value; }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(SystemColors.ButtonFace);
            Color clr = this.BackColor;
            clr = Color.LightBlue;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            GraphicsPath path1 = new GraphicsPath();
            path1.AddEllipse(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            LinearGradientBrush br1 = new LinearGradientBrush(rc, BorderColor, ButtonBackColor, LinearGradientMode.Vertical);
            Rectangle rc2 = rc;

            GraphicsPath path2 = new GraphicsPath();
            path2.AddEllipse(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            PathGradientBrush br2 = new PathGradientBrush(path2);
            br2.CenterColor = Color.Gold;
            br2.SurroundColors = new Color[] { SystemColors.ButtonFace };
            Rectangle rc3 = rc;
            GraphicsPath path3 = new GraphicsPath();
            int x = (int)(ClientSize.Width * 0.81);
            int y = (int)(ClientSize.Height * 0.6);
            int xx = (int)((this.ClientSize.Width) - x) / 2;
            int yy = (int)((this.ClientSize.Height) - y) / 2;
            path3.AddEllipse(0, 0, this.ClientSize.Width, y);
            LinearGradientBrush br3 = new LinearGradientBrush(rc3, Color.FromArgb(255, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);

            //g.FillPath(br2, path2);//绘制阴影
            g.FillPath(br1, path1);//绘制按钮
            //g.FillPath(br3, path3);//绘制顶部白色泡泡
            Region rgn = new Region(path1); //将region赋值给button
            rgn.Union(path2);
            this.Region = rgn;
            if (this.Txt != null)  // 绘制文本 
            {
                using (StringFormat f = new StringFormat())
                {
                    f.Alignment = System.Drawing.StringAlignment.Center;// 水平居中对齐 
                    f.LineAlignment = System.Drawing.StringAlignment.Center;   // 垂直居中对齐 
                    f.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;// 设置为单行文本 
                    SolidBrush fb = new SolidBrush(this.ForeColor); // 绘制文本 
                    e.Graphics.DrawString(this.Txt, this.Font, fb, new System.Drawing.RectangleF(0, 0, this.ClientSize.Width, this.ClientSize.Height), f);
                }
            }

        }
        protected override void OnClick(EventArgs e)
        {
            Point p = System.Windows.Forms.Control.MousePosition;
            p = base.PointToClient(p);
            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddEllipse(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
                bool flag = path.IsVisible(p.X, p.Y);
                if (!flag)
                    this.Invalidate();
                else
                    base.OnClick(e);
            }

        }
    }
}
