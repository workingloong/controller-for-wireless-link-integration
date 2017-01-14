using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace Cshapshiyan_1
{

    public partial class plot : Form
    {
      //  FileStream fs = new FileStream("mytext.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw;
        Bitmap BTMap = new Bitmap(600, 800);
        Bitmap BTMap2 = new Bitmap(600, 800);
        Bitmap BitMap1 = new Bitmap(300, 400);
        PointF CenterPt = new PointF(40, 460);
        Graphics Gph;
       // System.Timers.Timer timer;
        Graphics G;
       // int duibi = 0;
        private Thread thread4;
        public plot()
        {
            InitializeComponent();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //StreamWriter sw = new StreamWriter(fs);
            lock (Form1.obj4)
            {
                System.Threading.Thread.Sleep(10);
                Gph = Graphics.FromImage(BTMap);
                //背景
                Gph.Clear(Color.AliceBlue);

                MyDraw();
                Gph.DrawString("蓝色：WLAN网络丢包率", new Font("宋体", 8), Brushes.Blue, new PointF(280, 20));
                Gph.DrawString("红色：WLAN网络分流比例", new Font("宋体", 8), Brushes.Red, new PointF(280, 10));
                Gph.DrawString("黑色：LTE网络丢包率", new Font("宋体", 8), Brushes.Black, new PointF(280, 30));
                double[] d = new double[Form1.jiezhi + 1];
                double[] lostd = new double[Form1.lostjiezhi + 1];
                double[] lostd_2 = new double[Form1.lostjiezhi + 1];
                //double[] d = new double[10];
                //d[1] = 0.5; d[2] = 0.5; d[3] = 0.5; d[4] = 0.5; d[5] = 0.5; d[6] = 0.5; d[7] = 0.5; d[8] = 0.5; d[9] = 0.5; d[0] = 0.5;
                float xjiange = 30;
                int ziti = 8;
                int size = 9;
                if (Form1.jiezhi > 1)
                // if (size > 3)
                {
                    int i = 0;
                    int j = 0;
                    for (i = 0; i < Form1.jiezhi; i++)
                    {
                        d[i] = Convert.ToDouble(Form1.bilijihe[i]);
                        lostd[i] = Convert.ToDouble(Form1.lostbilijihe[i]);
                        lostd_2[i] = Convert.ToDouble(Form1.lostbilijihe_2[i]);
                        //Gph.DrawString(Month[i - 1], new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X + i * 30 - 5, CenterPt.Y + 5));
                        //曲线的交叉点
                        //sw.WriteLine(lostd[i]);
                        Gph.FillEllipse(new SolidBrush(Color.Black), CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 150, 3, 3);
                        Gph.FillEllipse(new SolidBrush(Color.Orange), CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)lostd[i] * 150, 3, 3);
                        Gph.DrawString(Math.Round(d[i], 2).ToString(), new Font("宋体", ziti), Brushes.Black, new PointF(CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 150 - 10));
                        Gph.DrawString(Math.Round(lostd[i], 2).ToString(), new Font("宋体", ziti), Brushes.Black, new PointF(CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)lostd[i] * 150 - 10));
                        Gph.DrawString(Math.Round(lostd_2[i], 2).ToString(), new Font("宋体", ziti), Brushes.Black, new PointF(CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)lostd_2[i] * 150 - 10));
                        //Gph.DrawString(
                        if (i > 0)
                        {
                            Gph.DrawLine(Pens.Red, CenterPt.X + (i - j * 15 - 1) * xjiange, CenterPt.Y - 260 - (float)d[i - 1] * 150, CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 150);
                            Gph.DrawLine(Pens.Blue, CenterPt.X + (i - j * 15 - 1) * xjiange, CenterPt.Y - 260 - (float)lostd[i - 1] * 150, CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)lostd[i] * 150);
                            Gph.DrawLine(Pens.Black, CenterPt.X + (i - j * 15 - 1) * xjiange, CenterPt.Y - 260 - (float)lostd_2[i - 1] * 150, CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)lostd_2[i] * 150);
                        }
                        if ((i % 15 == 0) & (i != 0))
                        {
                            j++;
                           // Form1.bilijihe = new double[1000];
                           // Form1.lostbilijihe = new double[1000];
                            Gph.Clear(Color.AliceBlue);
                            MyDraw();
                            Gph.DrawString("蓝色：WLAN网络丢包率", new Font("宋体", 8), Brushes.Blue, new PointF(280, 20));
                            Gph.DrawString("红色：WLAN网络分流比例", new Font("宋体", 8), Brushes.Red, new PointF(280, 10));
                            Gph.DrawString("黑色：LTE网络丢包率", new Font("宋体", 8), Brushes.Black, new PointF(280, 30));
                        }
                    }
                }
                //pictureBox1.Image = BTMap;
                pictureBox2.Image = BTMap;
                //pictureBox1.Refresh();
            }
        }

        private void heihei(object sender, System.Timers.ElapsedEventArgs e)
        {
            pictureBox1_Paint(null, new PaintEventArgs(Gph, new Rectangle(0, 0, 0, 0)));
        }

        private void plot_Load(object sender, EventArgs e)
        {

         //   thread4 = new Thread(new ThreadStart(Move1));
            //启动线程
          //  thread4.Start();
            //timer = new System.Timers.Timer();
            //timer.Enabled = true;
            //timer.AutoReset = true;
            //timer.Interval = 1000;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(Move);
            //timer.Start();
            //this.Location = new Point(100, 100);
            //move();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            lock (Form1.obj4)
            {
                System.Threading.Thread.Sleep(10);
                //System.Threading.Thread.Sleep(100);
                double net1bili = Form1.realbili_single;
                //double net1bili = 1;
                double net2bili;
                net2bili = 1 - net1bili;
                //票数比例 小数点
                //if (Form1.jiezhi > 3)
                //{
                G = Graphics.FromImage(BitMap1);
                G.Clear(Color.AliceBlue);
                Brush Brush_Bg = new SolidBrush(Color.AliceBlue);
                Brush Brush_Word = new SolidBrush(Color.Black);
                Brush Brush_net1 = new SolidBrush(Color.Red);
                Brush Brush_net2 = new SolidBrush(Color.Green);

                Font FontTitle = new Font("Courier New", 12, FontStyle.Bold);
                Font font2 = new Font("Courier New", 8);
                G.FillRectangle(Brush_Bg, 0, 0, 300, 300);    //绘制背景图
                G.DrawString("分流比例变化情况", FontTitle, Brush_Word, new Point(60, 20));

                //Point p1 = new Point(50, 50);
                //Point p2 = new Point(190, 50);
                //G.DrawLine(new Pen(Color.Black), p1, p2);
                //绘制文字
                G.DrawString("TD-LTE：", font2, Brush_Word, new Point(55, 50));
                G.DrawString("WIFI：", font2, Brush_Word, new Point(55, 90));
                //绘制柱形图
                G.FillRectangle(Brush_net1, 115, 90, (float)net1bili * 100, 17);
                G.FillRectangle(Brush_net2, 115, 50, (float)net2bili * 100, 17);
                //绘制所有选项的票数显示
                G.DrawRectangle(new Pen(Color.Green), 55, 130, 180, 100);  //绘制范围框
                G.DrawString("WIFI分流比例：" + (float)net1bili, font2, Brush_Word, new Point(70, 150));
                G.DrawString("TD-LTE分流比例：" + (float)net2bili, font2, Brush_Word, new Point(70, 190));
                pictureBox2.Image = BitMap1;
            }
        }
        private void MyDraw()
        {
                //中心点
                PointF CenterPt = new PointF(40,200);
                //X轴三角形
                PointF[] XPt = new PointF[3] { new PointF(CenterPt.Y + 10, CenterPt.Y), new PointF(CenterPt.Y, CenterPt.Y - 4), new PointF(CenterPt.Y, CenterPt.Y + 4) };
                //Y轴三角形
                PointF[] YPt = new PointF[3] { new PointF(CenterPt.X, CenterPt.X - 10), new PointF(CenterPt.X - 4, CenterPt.X), new PointF(CenterPt.X + 4, CenterPt.X) };
                PointF[] YPt1 = new PointF[3] { new PointF(CenterPt.X + 420, CenterPt.X - 10), new PointF(CenterPt.X + 420 - 4, CenterPt.X), new PointF(CenterPt.X + 420 + 4, CenterPt.X) };

                Gph.DrawLine(Pens.Black, CenterPt.X, CenterPt.Y+5, CenterPt.Y+260, CenterPt.Y+5);

                Gph.DrawString("时间", new Font("宋体", 10), Brushes.Black, new PointF(CenterPt.X+190 , CenterPt.Y+30 - 20));
                Gph.DrawString("分流比例", new Font("宋体", 10), Brushes.Black, new PointF(CenterPt.X-25, CenterPt.Y - 180));
                Gph.DrawString("丢包率", new Font("宋体", 10), Brushes.Black, new PointF(CenterPt.X +400, CenterPt.Y - 180));

                Gph.DrawLine(Pens.Black, CenterPt.X, CenterPt.Y + 5, CenterPt.X, CenterPt.X);
                Gph.DrawLine(Pens.Black, CenterPt.X + 420, CenterPt.Y + 5, CenterPt.X + 420, CenterPt.X);
                //填充箭头
                Gph.FillPolygon(new SolidBrush(Color.Black), YPt);
                Gph.FillPolygon(new SolidBrush(Color.Black), YPt1);
                for (float i = 0; i <= 10; i++)
                {
                    //Y轴刻度
                    if (i <= 10)
                    {
                        Gph.DrawString((i / 10).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(CenterPt.X - 40, CenterPt.Y - i * 15 - 6));
                        Gph.DrawLine(Pens.Black, CenterPt.X - 3, CenterPt.Y - i * 15, CenterPt.X, CenterPt.Y - i * 15);
                        Gph.DrawString((i / 10).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(CenterPt.X + 470 - 40, CenterPt.Y - i * 15 - 6));
                        Gph.DrawLine(Pens.Black, CenterPt.X + 420- 3, CenterPt.Y - i * 15, CenterPt.X + 420, CenterPt.Y - i * 15);
                    }
                }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //pictureBox1.Focus();
            //pictureBox2.Focus();
            pictureBox2.Refresh();
            pictureBox1.Refresh();
            pictureBox3.Refresh();
        }
        private void MyDraw1()
        {

            //中心点
            PointF CenterPt = new PointF(40, 200);
            //X轴三角形
            PointF[] XPt = new PointF[3] { new PointF(CenterPt.Y + 10, CenterPt.Y), new PointF(CenterPt.Y, CenterPt.Y - 4), new PointF(CenterPt.Y, CenterPt.Y + 4) };
            //Y轴三角形
            PointF[] YPt = new PointF[3] { new PointF(CenterPt.X, CenterPt.X - 10), new PointF(CenterPt.X - 4, CenterPt.X), new PointF(CenterPt.X + 4, CenterPt.X) };
            PointF[] YPt1 = new PointF[3] { new PointF(CenterPt.X + 420, CenterPt.X - 10), new PointF(CenterPt.X + 420 - 4, CenterPt.X), new PointF(CenterPt.X + 420 + 4, CenterPt.X) };
            //Gph.DrawString("分流比例情况", new Font("宋体", 14), Brushes.Black, new PointF(CenterPt.X + 140, CenterPt.X - 10));//图表标题
            //X轴
            Gph.DrawLine(Pens.Black, CenterPt.X, CenterPt.Y + 5, CenterPt.Y, CenterPt.Y + 5);
            //Gph.DrawPolygon(Pens.Black, XPt);
            // Gph.FillPolygon(new SolidBrush(Color.Black), XPt);
            Gph.DrawString("时间", new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X + 200, CenterPt.Y + 30 - 20));
            //Gph.DrawString("分流比例", new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X -10, CenterPt.Y - 200));
            //Gph.DrawString("丢包率", new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X + 4300, CenterPt.Y - 200));
            //Y轴
            Gph.DrawLine(Pens.Black, CenterPt.X, CenterPt.Y + 5, CenterPt.X, CenterPt.X);
            Gph.DrawLine(Pens.Black, CenterPt.X + 420, CenterPt.Y + 5, CenterPt.X + 420, CenterPt.X);
            //填充箭头
            Gph.FillPolygon(new SolidBrush(Color.Black), YPt);
            Gph.FillPolygon(new SolidBrush(Color.Black), YPt1);
            Gph.DrawString("WIFI链路分流比例", new Font("宋体", 12), Brushes.Black, new PointF(0, 7));
            Gph.DrawString("WIFI链路丢包率", new Font("宋体", 12), Brushes.Black, new PointF(420, 7));
            for (float i = 0; i <= 10; i++)
            {
                //Y轴刻度
                if (i <= 10)
                {
                    Gph.DrawString((i / 10).ToString(), new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X - 40, CenterPt.Y - i * 40 - 6));
                    Gph.DrawLine(Pens.Black, CenterPt.X - 3, CenterPt.Y - i * 40, CenterPt.X, CenterPt.Y - i * 40);
                    Gph.DrawString((i / 10).ToString(), new Font("宋体", 12), Brushes.Black, new PointF(CenterPt.X + 470 - 40, CenterPt.Y - i * 40 - 6));
                    Gph.DrawLine(Pens.Black, CenterPt.X + 420 - 3, CenterPt.Y - i * 40, CenterPt.X + 420, CenterPt.Y - i * 40);
                }
            }

        }
        private void plot_FormClosed(object sender, FormClosedEventArgs e)
        {
           // thread4.Abort();
            sw.Close();
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {

            //MyDrawClock();
            lock (Form1.obj4)
            {
                lock (Form1.obj3_single)
                {
                    System.Threading.Thread.Sleep(10);
                    Gph = Graphics.FromImage(BTMap2);
                    //背景
                    Gph.Clear(Color.AliceBlue);

                    MyDraw();
                    Gph.DrawString("蓝色：WLAN网络丢包率", new Font("宋体", 8), Brushes.Blue, new PointF(280, 20));
                 //   Gph.DrawString("红色：WLAN网络分流比例", new Font("宋体", 8), Brushes.Red, new PointF(350, 7));
                 //   Gph.DrawString("黑色：LTE网络丢包率", new Font("宋体", 8), Brushes.Black, new PointF(350, 27));
                    double[] d = new double[Form1.jiezhi_single + 1];
                    //double[] lostd = new double[Form1.lostjiezhi+ 1];
                    //double[] d = new double[10];
                    //d[1] = 0.5; d[2] = 0.5; d[3] = 0.5; d[4] = 0.5; d[5] = 0.5; d[6] = 0.5; d[7] = 0.5; d[8] = 0.5; d[9] = 0.5; d[0] = 0.5;
                    float xjiange = 30;
                    int ziti = 8;
                    int size = 9;
                   // Form1.jiezhi_single = 4;
                    if (Form1.jiezhi_single > 1)
                    // if (size > 3)
                    {
                        int i = 0;
                        int j = 0;
                        for (i = 0; i <= Form1.jiezhi_single; i++)
                        //for (i = 0; i < size; i++)
                        {
                             d[i] = Convert.ToDouble(Form1.bilijihe_single[i]);
                            //d[i] = 0;
                            //曲线的交叉点

                            Gph.FillEllipse(new SolidBrush(Color.Black), CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 200, 3, 3);
                            //   Gph.FillEllipse(new SolidBrush(Color.Orange), CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y -210- (float)lostd[i] * 200, 3, 3);
                            Gph.DrawString(Math.Round(d[i], 2).ToString(), new Font("宋体", ziti), Brushes.Black, new PointF(CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 200 - 10));
                            //   Gph.DrawString(Math.Round(lostd[i], 2).ToString(), new Font("宋体", ziti), Brushes.Black, new PointF(CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 210 - (float)lostd[i] * 200 - 10));
                            //Gph.DrawString(
                            if (i > 0)
                            {
                                Gph.DrawLine(Pens.Red, CenterPt.X + (i - j * 15 - 1) * xjiange, CenterPt.Y - 260 - (float)d[i - 1] * 200, CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 260 - (float)d[i] * 200);
                                //  Gph.DrawLine(Pens.Blue, CenterPt.X + (i - j * 15 - 1) * xjiange, CenterPt.Y - 210 - (float)lostd[i - 1] * 200, CenterPt.X + (i - j * 15) * xjiange, CenterPt.Y - 210 - (float)lostd[i] * 200);
                            }
                            if ((i % 15 == 0) & (i != 0))
                            {
                                j++;
                                //Gph = Graphics.FromImage(BTMap);
                                //xjiange = xjiange;
                                //ziti = ziti - 1;
                                Gph.Clear(Color.AliceBlue);
                                MyDraw();
                                Gph.DrawString("蓝色：WLAN网络丢包率", new Font("宋体", 8), Brushes.Blue, new PointF(280, 20));
                            //    Gph.DrawString("红色：WLAN网络分流比例", new Font("宋体", 8), Brushes.Red, new PointF(350, 7));
                           //     Gph.DrawString("黑色：LTE网络丢包率", new Font("宋体", 8), Brushes.Black, new PointF(350, 27));
                            }
                        }
                    }
                    pictureBox3.Image = BTMap2;
                    //pictureBox1.Refresh();
                }
            }
       }

        private void plot_FormClosing(object sender, FormClosingEventArgs e)
        {
            //thread4.Abort();
               sw.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        }

}
