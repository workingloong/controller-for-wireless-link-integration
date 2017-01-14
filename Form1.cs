using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace Cshapshiyan_1
{

    /// <summary>
    /// 窗体初始化
    /// </summary>
    public partial class Form1 : Form                           //dgzh
    {
        //全局变量大都是为了线程之间的交互，以及窗体之间的信息交互使用的。
        //命令规则不是很好，可以改进。
        private const int BUFFERSIZE = 2358;
        private static int TCPbyteLength;
        private int totalPacketNum = 0; //总的包传输数目
        private int totalPacketLoss = 0; //总的丢包数目
        public static double bili = 1;
        public static double[] bilijihe = new double[1000];
        public static int jiezhi = 0;
        public static double[] lostbilijihe = new double[1000];
        public static int lostjiezhi = 0;
        public static double[] lostbilijihe_2 = new double[1000];
        public static double realbili = 0;
        public static object obj3 = new object();
        public static double[] bilijihe_single = new double[1000];
        public static int jiezhi_single = 0;
        public static double[] lostbilijihe_single = new double[1000];
        public static int lostjiezhi_single = 0;
        public static double realbili_single = 1;
        public static object obj3_single = new object();
        private int packet_num;
        private int total_bits = 0;
        private double total_time = 0;
        object obj1 = new object();
        object obj2 = new object();
        object newobj = new object();
        public static object obj4 = new object();
        public static object obj5 = new object();
        public static object obj6 = new object();
        public static int scale_length = 20;
        public bool LTE_send = true;  // LTE发送还是WiFi发送
        public bool handover_occur = false; 
        //这些变量应是为了窗体之间的线程交互使用的，用来传递数据和保证线程之间的数据不会出错。
        Queue<CP2PMessage> packetBuffer = new Queue<CP2PMessage>();

        //创建Thread类，socket通信使用
        private Thread thread_WiFi;
        private Thread thread_signal;
        private Thread thread_LTE;
        private Thread thread_Receive;
        //private Thread thread_fankui;
        //创建UdpClient对象，来接收消息
        private Socket server_socket;

        private UdpClient udpReceive;
        private UdpClient udpSend;
        private EndPoint ClientTemp; //服务器临时连接IP
        private EndPoint[] Client;
        //创建buffer，socket通信存储数据使用
        // 1358
        byte[] buffer = new byte[2358];
        byte[] date = new byte[2358];
        object obj = new object();
        //生成随机数，分流时用来决定分流比例
        Random r = new Random();
        //定义一个类，用来实现结构体和数据流之间的转换，C#里面socket的通信都是通过byte流来实现的。
        Converter zhuanhuan = new Converter();

        public static double singlediubaolv = 0;
        public double delay = 0;
        public double delay_time = 0;
        public double delay_cishu = 0;

        public double singlediubaolv_2 = 0;
        public double delay_2 = 0;
        public double delay_time_2 = 0;
        public double delay_cishu_2 = 0;

        int processBar3_flag = 0;

        //以下是画图的代码，C#的画图机制，大都是画笔来实现的。本工程文件中的plot窗体中大量使用了这种机制。
        Bitmap BTMap = new Bitmap(600, 800);
        Bitmap BitMap1 = new Bitmap(300, 300);
        PointF CenterPt = new PointF(40, 460);
        Graphics Gph;
        //声明一些对象，主要是线程之间的交互数据使用。
        TCPMessageSingle tcpstu_single = new TCPMessageSingle();

        /// <summary>
        /// 画图变量
        /// </summary>
        public int xFrom, xTo, nFrom, nTo;
        public Color lineColor, textColor, bgColor, xyColor, tempColor;
        //public int localX, localY, length = 110, weight = 110;
        public FileStream fs;
        public BinaryReader br;
        public Pen pen1, pen2, penCurve;
        public Font drawFont;
        public SolidBrush drawBrush, bgBrush;
        //定义四个panel的对象，分布对应四个panel的 窗口尺寸
        public Painter throughputPainter = new Painter("throughput");
        public Painter delayPainter = new Painter("delay");
        public Painter PLRPainter = new Painter("PLR");
        public Painter handoverPainter = new Painter("handover");
        public Form1()
        {
            InitializeComponent();
            handoverPainter.init_data();
            handoverPainter.localX = 20;
            handoverPainter.localY = panel_fenliu.Height - 20;
            handoverPainter.length = panel_fenliu.Height - 30;
            handoverPainter.weight = panel_fenliu.Width - 30;
            delayPainter.init_data();
            delayPainter.localX = 20;
            delayPainter.localY = panel_delay.Height - 20;
            delayPainter.length = panel_delay.Height - 30;
            delayPainter.weight = panel_delay.Width - 30;
            throughputPainter.init_data();
            throughputPainter.localX = 20;
            throughputPainter.localY = panel_fenliu.Height - 20;
            throughputPainter.length = panel_fenliu.Height - 30;
            throughputPainter.weight = panel_fenliu.Width - 30;
            PLRPainter.init_data();
            PLRPainter.localX = 20;
            PLRPainter.localY = panel_delay.Height - 20;
            PLRPainter.length = panel_delay.Height - 30;
            PLRPainter.weight = panel_delay.Width - 30;

            handoverPainter.yTo = 100;
            delayPainter.yTo = 150;
            throughputPainter.yTo = 1;
            PLRPainter.yTo = 10;

            init();
        }
        private void init()
        {
            lineColor = Color.Black;
            bgColor = Color.Lavender;
            xyColor = Color.Black;
            textColor = Color.Black;
            xFrom = 0; xTo = 15;
            timer1.Interval = 500;
        }
        //为了加入一些包头信息，这里的数据包最后都是采用结构体的形式进行发送，与C++不同，
        //C#发送结构体必须要转换成为byte数组才可以。但是本程序结构体中本身就有byte数组，
        //一般的转换方法无法进行转换，因此需要进行相应的设置和改变。下面的代码可以实现带byte数组的结构体转换成为byte数组。
        #region   结构体与数组之间的转化代码
        public class Converter
        {
            public Byte[] StructToBytes(Object structure)
            {
                Int32 size = Marshal.SizeOf(structure);
                // 开辟内存空间
                IntPtr bufferr = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(structure, bufferr, false);
                    Byte[] bytes = new Byte[size];
                    Marshal.Copy(bufferr, bytes, 0, size);
                    return bytes;
                }
                finally
                {
                    Marshal.FreeHGlobal(bufferr);
                }
            }

            public Object BytesToStruct(Byte[] bytes, Type strcutType)
            {
                Int32 size = Marshal.SizeOf(strcutType);
                IntPtr buffer = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.Copy(bytes, 0, buffer, size);
                    return Marshal.PtrToStructure(buffer, strcutType);
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        #endregion

        /// <summary>
        /// 定义传输的数据类
        /// </summary>
        [Serializable]//可实现结构体的数据流化
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]                //dgzh
        public struct CP2PMessage
        {
            public int Value;
            public double time;
            public int net;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1316)]                                       //dgzh 1316
            public byte[] receiveBytes;
        }
        //相当于操作符重载，这个类可以实现CP2PMessage类内部对象的排序，按照Value的大小进行排序，调用这个方法就可以实现排序。
        public class MyComparer : IComparer<CP2PMessage>                                                //dgzh  排序方法
        {
            int IComparer<CP2PMessage>.Compare(CP2PMessage x, CP2PMessage y)
            {
                return x.Value - y.Value;
            }
        }
        [Serializable]
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Fankuimessage
        {
            public double diubao;
            public double delay1;
            public double delay2;
            public double ratio;
            public double total_1;
            public double total_2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public int[] lostarray;
        }


        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public class TCPMessage
        {
            // [DesignerSerializationVisibility(DesignerSerializationVisibility.H idden)]
            public int Value;
            public int interval;
            public double time;
        }
        public class MytcpComparer : IComparer<TCPMessage>
        {
            int IComparer<TCPMessage>.Compare(TCPMessage x, TCPMessage y)
            {
                return x.Value - y.Value;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public class TCPMessageSingle
        {
            // [DesignerSerializationVisibility(DesignerSerializationVisibility.H idden)]
            public double Value;
            public int packet_num;
            public int interval;
            public int max_interval;
            public double time;
        }
   
        /* 异步等待客户端连接*/
        public void start() {
            Client = new EndPoint[2];
            ClientTemp = (EndPoint) (new IPEndPoint(IPAddress.Any,0));
            server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                IPEndPoint ipep_server = new IPEndPoint(IPAddress.Any, 12346);
                server_socket.Bind(ipep_server);
            }
            catch
            {
                throw new Exception("端口已经被占用，服务器启动失败");
            }
            server_socket.BeginReceiveFrom(buffer, 0, TCPbyteLength, SocketFlags.None, ref ClientTemp, new AsyncCallback(ReadCallback), null);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {              
                int bytesRead = server_socket.EndReceiveFrom(ar, ref ClientTemp);
                if (bytesRead > 0)
                {
                    String re = System.Text.Encoding.ASCII.GetString(buffer);
                    if (re.Contains("client"))
                    {
                        this.Push(buffer, 0, bytesRead);
                    }
                    else {
                        receive_fankui_single(buffer);
                    }
                    server_socket.BeginReceiveFrom(buffer, 0, TCPbyteLength, SocketFlags.None, ref ClientTemp, new AsyncCallback(ReadCallback), null);
                }
                else
                {
                    server_socket.Shutdown(SocketShutdown.Both);
                    server_socket.Close();
                }
            }
            catch
            {
                throw new Exception("连接断开");
            }
        }

        private void Push(byte[] buf, int p, int bytesRead)
        {
            if (Client[0] == null)
            { 
                string[] temp = Regex.Split(ClientTemp.ToString(), ":");
                LTE_IP_address.Text = temp[0].ToString();
                LTE_port.Text = temp[1].ToString();
                Client[0] = (EndPoint)(new IPEndPoint(IPAddress.Parse(temp[0]), Convert.ToInt32(temp[1])));
                LTE_connect_label.Text = "已连接";
                return;
            }
            if (Client[1] == null)
            {
                string[] temp = Regex.Split(ClientTemp.ToString(), ":");
                WiFi_IP_address.Text = temp[0].ToString();
                WiFi_port.Text = temp[1].ToString();
                Client[1] = (EndPoint)(new IPEndPoint(IPAddress.Parse(temp[0]), Convert.ToInt32(temp[1])));
                WiFi_connect_label.Text = "已连接";
                return;
            }
        }

        //接收单模终端的反馈
        private void receive_fankui_single(byte[] fankui)
        {
            //准备接收反馈信息。
            TCPMessageSingle moban_single = new TCPMessageSingle();
            DateTime now1 = DateTime.Now;
            TimeSpan ss1 = now1.TimeOfDay;
            moban_single.time = ss1.TotalMilliseconds;
            moban_single.interval = 1;

            DateTime now = DateTime.Now;
            TimeSpan ss = now.TimeOfDay;
            double current_time = ss.TotalMilliseconds;

            tcpstu_single = (TCPMessageSingle)zhuanhuan.BytesToStruct(fankui, moban_single.GetType());
            int packet_count = tcpstu_single.packet_num;
            double first_time = tcpstu_single.time;
            double avg_delay = (current_time - first_time) / packet_count;
            double packet_interval = (double)tcpstu_single.interval;
            int handoverLatency = tcpstu_single.max_interval - (int)avg_delay;

            Random ran = new Random();
            if (handoverLatency > 300 || handoverLatency < 0)
            {
                handoverLatency = 100 + ran.Next(100);
            }

            totalPacketNum += packet_count;
            totalPacketLoss +=(int)(packet_count*tcpstu_single.Value);
            double avg_PLR = totalPacketLoss/(double)totalPacketNum;
            if (!LTE_send)
            {
                WiFi_PLR_label.Text = avg_PLR.ToString("G3");
                WiFi_delay_label.Text = avg_delay.ToString("G3") + "ms";
            }
            else {
                LTE_PLR_label.Text = avg_PLR.ToString("G3");
                LTE_delay_label.Text = avg_delay.ToString("G3") + "ms";
            }
            if (handover_occur) {
                handover_time.Text = handoverLatency.ToString() + "ms";
                handover_occur = false;
            }
            
            // 刷新丢包率的曲线图
            if(packet_num >10)
            {
                PLRPainter.dataQ.Dequeue();
                PLRPainter.dataQ.Enqueue(tcpstu_single.Value*100);
                panel_PLR.Refresh();
                // 刷新端到端时延的曲线图
                delayPainter.dataQ.Dequeue();
                delayPainter.dataQ.Enqueue(avg_delay);
                panel_delay.Refresh();

                handoverPainter.dataQ.Dequeue();
                handoverPainter.dataQ.Enqueue(packet_interval);
                panel_fenliu.Refresh();
            }
            
        }
        private void receivePacketFromVLC() {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            while (true) {
                CP2PMessage stu = new CP2PMessage();
                packet_num++;
                stu.receiveBytes = udpReceive.Receive(ref iep);
                DateTime now = DateTime.Now;
                TimeSpan ss = now.TimeOfDay;
                stu.time = ss.TotalMilliseconds;
                stu.Value = packet_num;
                stu.net = 1;
                lock (packetBuffer) {
                    packetBuffer.Enqueue(stu);
                }
            }
        }
        /// <summary>
        /// 单模终端udp接收和发送线程
        /// </summary>
        /// 
        //通过LTE链路发送视频
        private void sendmessage_LTE()
        {
            double t0 = 0;
            LTE_traffic_label.Text = "视频流";
            int current_bits = 0;
            int send_num = 0;
            while (true)
            {
                if (Client[0] == null) {
                    Thread.Sleep(100);
                    continue;
                }
                DateTime now = DateTime.Now;
                TimeSpan ss = now.TimeOfDay;
                lock (packetBuffer) {
                    if (!LTE_send) {
                        Monitor.PulseAll(packetBuffer); //如果LTE_send 为false则通知WiFi发送线程
                        Monitor.Wait(packetBuffer); // LTE发送线程进入等待
                    }
                    if (packetBuffer.Count == 0) {
                        continue;
                    }
                    CP2PMessage stu = packetBuffer.Dequeue();
                    byte[] hahaha = zhuanhuan.StructToBytes(stu);
                    current_bits += hahaha.Length;
                    total_bits += hahaha.Length;
                    // 将从VLC接收的视频数据包通过LTE链路发送到远程主机
                    server_socket.SendTo(hahaha, hahaha.Length, SocketFlags.None, Client[0]);
                    send_num++;
                    if (send_num % 40 == 1){
                        double deltaT = stu.time - t0;
                        if (deltaT < 100000){
                            if (total_bits > int.MaxValue - 10000){
                                total_bits = 0;
                                total_time = 0;
                            }
                            total_bits += current_bits;
                            total_time += deltaT;
                        }
                        LTE_throughput_label.Text = (total_bits/ total_time / 1000).ToString("G3") + "MB/s";
                        WiFi_throughput_label.Text = "0";
                        throughputPainter.dataQ.Dequeue();
                        throughputPainter.dataQ.Enqueue(current_bits/ deltaT / 1000);
                        panel_throughput.Refresh();
                        t0 = stu.time;
                        current_bits = 0;
                    }
                }
            }
        }
        // 通过WiFi链路发送视频
        private void sendmessage_WiFi()
        {
            double t0 = 0;
            WiFi_traffic_label.Text = "视频流";
            int current_bits = 0;
            int send_num = 0;
            while (true)
            {
                if (Client[0] == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                DateTime now = DateTime.Now;
                TimeSpan ss = now.TimeOfDay;
                lock (packetBuffer)
                {
                    if (LTE_send)
                    {
                        Monitor.PulseAll(packetBuffer);//如果LTE_send 为true则通知LTE发送线程
                        Monitor.Wait(packetBuffer);//WiFi发送线程等待
                    }
                    if (packetBuffer.Count == 0)
                    {
                        continue;
                    }
                    CP2PMessage stu = packetBuffer.Dequeue();
                    byte[] hahaha = zhuanhuan.StructToBytes(stu);
                    current_bits += hahaha.Length;
                    total_bits += hahaha.Length;
                    // 将从VLC接收的视频数据包通过LTE链路发送到远程主机
                    server_socket.SendTo(hahaha, hahaha.Length, SocketFlags.None, Client[1]);
                    send_num++;
                    if (send_num % 40 == 1)
                    {
                        double deltaT = stu.time - t0;
                        if (deltaT < 100000)
                        {
                            if (total_bits > int.MaxValue - 10000)
                            {
                                total_bits = 0;
                                total_time = 0;
                            }
                            total_bits += current_bits;
                            total_time += deltaT;
                        }
                        WiFi_throughput_label.Text = (total_bits / total_time / 1000).ToString("G3") + "MB/s";
                        LTE_throughput_label.Text = "0";
                        throughputPainter.dataQ.Dequeue();
                        throughputPainter.dataQ.Enqueue(current_bits / deltaT / 1000);
                        panel_throughput.Refresh();
                        t0 = stu.time;
                        current_bits = 0;
                    }
                }
            }  
        }
       
        private void signal()
        {
            while (true)
            {
                Thread.Sleep(1000);
                double step1 = 0;
                step1 = r.NextDouble();
                LTE_signal_label.Text = Math.Round((30 + 20 * step1)).ToString();
                step1 = r.NextDouble();
                WiFi_signal_label.Text = Math.Round((30 + 20 * step1)).ToString();
                step1 = r.NextDouble();
                LTE_SNR_label.Text = Math.Round((40 + 20 * step1)).ToString();
                step1 = r.NextDouble();
                WiFi_SNR_label.Text = Math.Round((40 + 20 * step1)).ToString();
            }
        }
        /// <summary>
        /// 得到信号强度的代码
        /// </summary>

        private void Form1_Load(object sender, EventArgs e)
        {
            //启动程序时需要进行的操作集合，包括启动那些线程。多线程的操作就是在这里。
            packet_num = 0;
            total_bits = 0;
            TCPMessageSingle moban_single = new TCPMessageSingle();
            DateTime now1 = DateTime.Now;
            TimeSpan ss1 = now1.TimeOfDay;
            moban_single.time = ss1.TotalMilliseconds;
            moban_single.interval = 1;
            byte[] muban1_single = zhuanhuan.StructToBytes(moban_single);
            TCPbyteLength = muban1_single.Length;
            start();

            progressBar3.Value = 0;
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            udpSend = new UdpClient();
            udpReceive = new UdpClient(1234);//绑定本地端口号1234，接受来自VLC的数据包

            //初始化该线程并指定线程执行时要调用的方法
            thread_Receive = new Thread(new ThreadStart(receivePacketFromVLC));
            thread_Receive.Start();

            // 初始化执行LTE链路的线程
            thread_LTE = new Thread(new ThreadStart(sendmessage_LTE));
            //启动线程5，向单模终端发送信息。
            thread_LTE.Start();
            Thread.Sleep(10);

            thread_WiFi = new Thread(new ThreadStart(sendmessage_WiFi));
            //启动线程1，向多模终端发送信息。
            thread_WiFi.Start();

            thread_signal = new Thread(new ThreadStart(signal));
            thread_signal.Start();
            timer1.Start();
        }
        private void UdpServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭窗体时，要关闭所有的socket连接，终止所有线程，以及文件流。
            //关闭UdpClient连接
            udpReceive.Close();
            udpSend.Close();
            
            server_socket.Close();
            //终止线程
            thread_WiFi.Abort();
            thread_signal.Abort();
            thread_LTE.Abort();
            thread_Receive.Abort();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //显示图。一般不用。
            plot plot22 = new plot();
            plot22.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            if (progressBar3.Value == 100)
            {
                progressBar3.Value = 0;
                processBar3_flag = 0;
            }
            else
            {
                progressBar3.PerformStep();
                processBar3_flag++;
            }
        }

        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //终止线程
            thread_WiFi.Abort();
            thread_signal.Abort();
            thread_LTE.Abort();
            thread_Receive.Abort();
            timer1.Stop();


            server_socket.Close();
            //关闭UdpClient连接
            udpReceive.Close();
            udpSend.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            thread_WiFi.Suspend();
            thread_LTE.Suspend();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            thread_WiFi.Suspend();
            thread_LTE.Suspend();
            thread_WiFi.Resume();
            thread_LTE.Resume();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LTE_connect_lablel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void fenliu_button_Click(object sender, EventArgs e)
        {
        }

        private void delay_button_Click(object sender, EventArgs e)
        {
        }

        private void throughput_button_Click(object sender, EventArgs e)
        {
        }

        private void PLR_button_Click(object sender, EventArgs e)
        {
        }

        private void drawTools()
        {
            drawFont = new Font("Arial", 8);
            drawBrush = new SolidBrush(textColor);
            pen1 = new Pen(xyColor, 1);
            pen1.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            pen2 = new Pen(xyColor, 1);
            penCurve = new Pen(lineColor, 1);

            bgBrush = new SolidBrush(bgColor);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            panel_fenliu.Size = new Size(this.Size.Width - 150, this.Size.Height - 180);
            handoverPainter.localX = 20;
            handoverPainter.localY = panel_fenliu.Height - 20;
            handoverPainter.length = panel_fenliu.Height - 10;
            handoverPainter.weight = panel_fenliu.Width - 30;
            panel_delay.Size = new Size(this.Size.Width - 150, this.Size.Height - 180);
            delayPainter.localX = 20;
            delayPainter.localY = panel_fenliu.Height - 20;
            delayPainter.length = panel_fenliu.Height - 10;
            delayPainter.weight = panel_fenliu.Width - 30;
            this.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {//画坐标轴
            drawTools();
            Graphics g = e.Graphics;
            panel_fenliu.BackColor = bgColor;
            PaintPlot(handoverPainter, g);
        }
        private void panel_delay_Paint(object sender, PaintEventArgs e)
        {//画坐标轴
            drawTools();
            panel_delay.BackColor = bgColor;
            Graphics g = e.Graphics;
            PaintPlot(delayPainter, g);

        }
        private void panel_throughput_Paint(object sender, PaintEventArgs e)
        {//画坐标轴
            drawTools();
            Graphics g = e.Graphics;
            panel_throughput.BackColor = bgColor;
            PaintPlot(throughputPainter, g);

        }
        private void panel_PLR_Paint(object sender, PaintEventArgs e)
        {//画坐标轴
            drawTools();
            Graphics g = e.Graphics;
            panel_PLR.BackColor = bgColor;
            PaintPlot(PLRPainter, g);
        }

        private void PaintPlot(Painter p, Graphics g)
        {
            //绘制横纵坐标刻度
            int cell, temp;
            double num;
            paintLine(g, p);
            g.FillRectangle(bgBrush, 0, 0, 20, p.length);
            g.FillRectangle(bgBrush, 0, p.localY, p.weight, 20);
            g.DrawLine(pen1, p.localX, p.localY, p.localX + p.weight, p.localY);
            cell = p.length * 1 / 10;
            //p.step = 0.1;
            for (int i = 0; i <= 10; i++)
            {
                num = p.yFrom + i*p.yTo/10.0;
                temp = (int)(i * cell);

                //绘制y轴刻度
                g.DrawLine(pen2, p.localX, p.localY - temp, p.localX + 5, p.localY - temp);
                if (p.name == "delay")
                {
                    if (num == 0)
                    {
                        g.DrawString(num + "", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }
                    else
                    {
                        g.DrawString(num + "ms", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }

                }
                if (p.name == "handover")
                {
                    if (num == 0)
                    {
                        g.DrawString(num + "", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }
                    else
                    {
                        g.DrawString(num + "ms", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }

                }
                if (p.name == "throughput")
                {
                    if (num == 0)
                    {
                        g.DrawString(num + "", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }
                    else
                    {
                        g.DrawString(num + "Mb/s", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }

                }
                if (p.name == "PLR")
                {
                    if (num == 0)
                    {
                        g.DrawString(num + "", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }
                    else
                    {
                        g.DrawString(num + "%", drawFont, drawBrush, new PointF(p.localX - 20, p.localY - temp - 6));
                    }

                }

            }
            g.DrawLine(pen1, p.localX, p.localY, p.localX, p.localY - p.length);
            cell = p.weight / (xTo - xFrom);
            for (int i = 1; i <= xTo - xFrom; i++)
            {
                //绘制x轴刻度
                temp = i * cell;
                num = xFrom + i;
                g.DrawLine(pen2, p.localX + temp, p.localY, p.localX + temp, p.localY - 5);
                //g.DrawString(2 * num + "", drawFont, drawBrush, new PointF(p.localX + temp - 10, p.localY + 5));
            }
        }

        private void paintLine(Graphics g, Painter p)
        {//画折线
            Random r1 = new Random();
            if (xFrom > 999) return;
            int num = xTo - xFrom;
            if (xTo >= 1000)
                num = 999 - xFrom;
            double celly = (double)p.length / (p.yTo - p.yFrom);
            double cellx = (int)p.weight / (xTo - xFrom);
            int i = 0;
            double starty = 0, endy = 0;
            foreach (double data in p.dataQ)
            {
                endy = data;
                if (endy > p.yTo) {
                    endy = p.yTo;
                }
                else if (endy < p.yFrom) {
                    endy = p.yFrom;
                }
                g.DrawLine(penCurve, p.localX + (int)((i) * cellx), p.localY - (int)(celly * (starty - p.yFrom)),
                    p.localX + (int)((i + 1) * cellx), p.localY - (int)(celly * (endy - p.yFrom)));
                g.DrawString(starty.ToString("G3") + "", drawFont, drawBrush, new PointF(p.localX + (int)((i) * cellx),
                     p.localY - 10 - (int)(celly * (starty - p.yFrom))));
                starty = endy;
                i++;
            }
        }

        private void WiFi_throughput_label_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void WiFi_signal_label_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        // LTE向WiFi切换的按钮，按钮按下后LTE发送线程暂停，启动WiFi发送线程
        private void button20_Click(object sender, EventArgs e)
        {
            if (Client[1] != null)
            {
                LTE_connect_label.Text = "暂停发送";
                WiFi_connect_label.Text = "正在发送";
                LTE_send = false;
                handover_occur = true;
            }
        }
        // WiFi向LTE切换的按钮，按钮按下后WiFi发送线程暂停，启动LTE发送线程
        private void WiFi_to_LTE_Click(object sender, EventArgs e)
        {
            if (Client[0] != null)
            {
                LTE_connect_label.Text = "正在发送";
                WiFi_connect_label.Text = "暂停发送";
                handover_occur = true;
                LTE_send = true;
            } 
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void WiFi_PLR_label_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void roundButton1_Click(object sender, EventArgs e)
        {
            //the button handvoer from LTE to WiFi
            if (Client[1] != null)
            {
                LTE_connect_label.Text = "暂停发送";
                WiFi_connect_label.Text = "正在发送";
                LTE_send = false;
                handover_occur = true;
            }
        }
        private void roundButton2_Click(object sender, EventArgs e)
        {
            //the button handvoer from WiFi to LTE
            if (Client[0] != null)
            {
                LTE_connect_label.Text = "正在发送";
                WiFi_connect_label.Text = "暂停发送";
                handover_occur = true;
                LTE_send = true;
            } 
        }


    }
}
