using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Collections;

namespace GMapTest1
{
    public partial class Form1 : Form
    {

        public GMapOverlay overlay = new GMapOverlay("WPMarker"); //用于绘制航点的Marker

        public  double [,] arr = new double [2, 8];
        public  double[,] ver = new double[2, 8];
        public double[,] arrdis = new double[2, 100];
        public double[,] arrmid = new double[2, 100];
        public double[,] tem = new double[2, 8];
        public  int y ;
        public  int i ;
        public  int i1;
        public int i11;
        public int c;

        public string strlng;
        public string strlat;
        public string dte; 
        public double lat1;
        public double lat2;
        public double lng1;
        public double lng2;
        public double area;

        
        
        public double C;
        public double D;//测距
        public double D0;//测距
        public double DX;//三角形X方向单位向量长度
        public double DY;//三角形y方向单位向量长度
        public double D1;//左伸长
        public double D2;//右伸长
        public double D11;//左伸长边距
        public double D22;//右伸长边距
        public double DXB;//斜边150米对应距离

        public double M0;//用于寻找最大值对应的点
        public double M1;//用于寻找最大值对应的点
        public double M2;//用于寻找最大值对应的点
        public double M3;//用于寻找最大值对应的点
        public double M4;//用于寻找最大值对应的点

        public double XL;//向量x 方向差值
        public double YL;//向量y 方向差值
        public double XLL;//与另外一个边的向量差
        public double YLL;//与另外一个边的向量差
        public double FXX;//单位向量x方向
        public double FYY;//单位向量y方向
        public double FX;//单位向量x方向
        public double FY;//单位向量y方向
        public double FXXX;//增量
        public double FYYY;
        public double Da;//测距
        public double Db;//测距
        public double Ang;//定义夹角   
        public double Ang0;
        public double Ang1;
        public double Ang2;
        public double Ang3;
        public double Ang4;
        public double Ang5;
        public double Ang6;
        public double Ang7;
        public double Ang8;

        public double Angdis;//倍数
        public double h;//定义三角高度用于判断拐的次数
        public double h1;//定义小三角短边
        public double h2;//定义四边形两高
        public int hh;//定义上三角形dis[hh+1]
        public int hs;//定义上三角开始
        public double h11;//定义四边形两高
        public double h22;//定义四边形两高

        public int m;    //用于寻找最大值对应的哪两个点
        public double Dismid;//中间值防值警告
        public double Dismax;//边长距离最大值




        public double[] Dis = new double[8];//定义数组存放距离       
        public double[] DisH = new double[100];//定义数组存放横向每个距离  
        public double[] DisHH = new double[100];//上三角数据存放
        public double R = 6371.0;
        private DateTime timemousedown;
        private int timemillSecond = 200;
       
        private int mousepress_x = 0, mousepress_y = 0;
        bool isMouseDrag = false;//false:鼠标单击 true:鼠标拖拽
    
        bool isMouseDown = false;
        bool isMarkerEnter = false;
        bool isMarkerDrag = false;
        GMapMarker currentMarker;
        public Form1()
        {
            InitializeComponent();
            this.gMapControl1.CacheLocation = System.Windows.Forms.Application.StartupPath + "\\GMapCache\\"; //缓存位置
            this.gMapControl1.CacheLocation = System.Windows.Forms.Application.StartupPath;
            this.gMapControl1.MapProvider = GMapProviders.GoogleChinaHybridMap;
            this.gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            this.gMapControl1.MinZoom = 1;                                                     //最小比例
            this.gMapControl1.MaxZoom = 35;                                                    //最大比例
            this.gMapControl1.Zoom = 14;                                                       //当前比例
            this.gMapControl1.ShowCenter = false;                                              //不显示中心十字点
            this.gMapControl1.DragButton = System.Windows.Forms.MouseButtons.Left;             //左键拖拽地图
            this.gMapControl1.Position = new PointLatLng(34.7861399,113.786437);

            this.gMapControl1.Overlays.Add(overlay);

            this.gMapControl1.MouseClick += gMapControl1_MouseClick;
            this.gMapControl1.MouseDown += gMapControl1_MouseDown;

            this.gMapControl1.OnMarkerEnter += gMapControl1_OnMarkerEnter;
            this.gMapControl1.OnMarkerLeave += gMapControl1_OnMarkerLeave;
            this.gMapControl1.MouseMove += gMapControl1_MouseMove;
            this.gMapControl1.MouseUp += gMapControl1_MouseUp;
        }
        List<PointLatLng> list = new List<PointLatLng>();
       
        void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng p = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            if (isMarkerEnter && isMouseDown) {
                isMarkerDrag = true;
            }
            if (!isMouseDown) {
                isMarkerDrag = false;
            }
            if (isMarkerDrag) { 
                currentMarker.Position = p;
            }

        }
        void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            isMarkerEnter = false;
        }
        void gMapControl1_OnMarkerEnter(GMapMarker item){
            isMarkerEnter = true;
            currentMarker = item;
        }
        void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            timemousedown = DateTime.Now;
           

            isMouseDown = true;
        }
        void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {

     
            #region 拖拽与点击分清操作

            if (timemousedown.AddMilliseconds(timemillSecond) > DateTime.Now)
            {
                isMouseDrag = false;
            }
            else
            {
                isMouseDrag = true;
            }

           
            if (!isMouseDrag)
            {
               
               PointLatLng p = gMapControl1.FromLocalToLatLng(e.X, e.Y);
               GMapMarker marker = new GMarkerGoogle(p, GMarkerGoogleType.blue_dot);

                tem[0, i] = p.Lat;// 定义临时数组储存鼠标点击后的纬度
                tem[1, i] = p.Lng;//定义临时数组储存鼠标点击后的经度

                overlay.Markers.Add(marker);
                list.Add(p);



                #region Routes操作

                overlay.Routes.Clear();

                GMapRoute route = new GMapRoute(list, "line");
                route.Stroke.Color = Color.Red;
                route.Stroke.Width = 2;  //设置画
                overlay.Routes.Add(route);

                #endregion

                #region Polygons操作

                overlay.Polygons.Clear();

                GMapPolygon polygon = new GMapPolygon(list, "多边形");
                polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                polygon.Stroke = new Pen(Color.Blue, 2);
                polygon.IsHitTestVisible = true;
                overlay.Polygons.Add(polygon);

                #endregion
                
                
                 arr[0, y] = tem[0, i];
                 arr[1, y] = tem[1, i];
                 strlng = " " + arr[1, y];//实时显示当前经经度
                 strlat = " " + arr[0, y] ;//实时显示当前经纬度
                 textBox1.Text = strlng;
                 textBox2.Text = strlat;

                 if (y == 0) //首次点击
                 {              
                     lat2 = 0;     
                 }
                 if (y == 1)//第二次点击与第一次的距离
                 {
                     lat1 = arr[0, 0] * Math.PI / 180.0;
                     lat2 = arr[0, 1] * Math.PI / 180.0;
                     lng1 = arr[1, 0] * Math.PI / 180.0;
                     lng2 = arr[1, 1] * Math.PI / 180.0;

                     double a = lat1 - lat2;
                     double b = lng1 - lng2;
                     D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                            + Math.Cos(lat1) * Math.Cos(lat2)
                            * Math.Pow(Math.Sin(b / 2), 2)));
                     D = D * R;
                     D = Math.Round(D * 10000) / 10000;
                     D = D * 1000;
                     Dis[y - 1] = D;
                     dte = " " + D + "米";
                 }
                 if (y >= 2)//实时距离
                 { 
                    lat1 = arr[0, y-1] * Math.PI / 180.0;
                    lat2 = arr[0, y  ] * Math.PI / 180.0;
                    lng1 = arr[1, y-1] * Math.PI / 180.0;
                    lng2 = arr[1, y  ] * Math.PI / 180.0;
                    double a = lat1 - lat2;
                    double b = lng1 - lng2;
                    D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                           + Math.Cos(lat1) * Math.Cos(lat2)
                           * Math.Pow(Math.Sin(b / 2), 2)));
                    D = D * R;
                    D = Math.Round(D * 10000) / 10000;
                    D = D * 1000;
                    Dis[y - 1] = D;
                     dte = " " + D+"米";
                 }



                   if (lat2 == 0)// 判断首次点击事件
                    {
                       dte = " ";
                    }
                   i = i + 1;
                   y = y + 1;
                   textBox3.Text = dte;

                 
                
                   
            }
            #endregion
            
            
        }
       

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (y == 2)
            {
                MessageBox.Show("请重新选取");
            }
            if (y == 3)
            {
               #region  统计全部边长操作
                lat1 = arr[0, 0] * Math.PI / 180.0;
                lat2 = arr[0, y - 1] * Math.PI / 180.0;
                lng1 = arr[1, 0] * Math.PI / 180.0;
                lng2 = arr[1, y - 1] * Math.PI / 180.0;
                double a = lat1 - lat2;
                double b = lng1 - lng2;
                D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                       + Math.Cos(lat1) * Math.Cos(lat2)
                       * Math.Pow(Math.Sin(b / 2), 2)));
                D = D * R;
                D = Math.Round(D * 10000) / 10000;
                D = D * 1000;
                Dis[y - 1] = D;
                #endregion
               #region   findmax操作

                if ((Dis[0] - Dis[1]) > 0)
                {
                    Dismax = Dis[0];
                   

                }
                else
                {
                    Dismax = Dis[1];
                 
                }

                if ((Dismax - Dis[2]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;

                }
                else
                {
                    Dismax = Dis[2];
                }
                        
                M0 = Dismax - Dis[0];
                M1 = Dismax - Dis[1];
                M2 = Dismax - Dis[2];
                Ang0 = (Dis[0] * Dis[0] + Dis[2] * Dis[2] - Dis[1] * Dis[1]) / (2 * Dis[0] * Dis[2]);// 计算三角形的每个角度
                Ang1 = (Dis[0] * Dis[0] + Dis[1] * Dis[1] - Dis[2] * Dis[2]) / (2 * Dis[0] * Dis[1]);
                Ang2 = (Dis[1] * Dis[1] + Dis[2] * Dis[2] - Dis[0] * Dis[0]) / (2 * Dis[1] * Dis[2]);
                Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                Ang0 = Ang0 * Math.PI / 180;
                Ang1 = Ang1 * Math.PI / 180;
                Ang2 = Ang2 * Math.PI / 180;

                if (M0 == 0)
                {
                    m = 0;
                }
                if (M1 == 0)
                {
                    m = 1;
                }
                if (M2 == 0)
                {
                    m = 2;
                }

                y = m;
                if (y == 1)//跳出
                {
                    y = 10;
                
                }
                if (y == 0)//第一点和第二点为最大值
                {
                    y = 1;
                    m = 1;
                    h = Dis[2] * Math.Sin(Ang0);
                    h1 = Dis[2] * Math.Cos(Ang0);
                    D1 = 150 / Math.Tan(Ang0)+200;
                    D11 = 150 / Math.Sin(Ang1);
                    D2 = 150 / Math.Tan(Ang1)+200;
                    D22 = Dis [1];
                    
                }
                
                if (y == 2)//第三点与第一点为最大值

                {
                    m = 2;
                    y = 1;
                    h = Dis[0] * Math.Sin(Ang0);
                    h1 = Dis[0] * Math.Cos(Ang0);
                    D1 = 150 / Math.Tan(Ang0)+200;
                    D11 = 150 / Math.Sin(Ang2);
                    D2 = 150 / Math.Tan(Ang2)+200;
                    D22 = Dis[1];
                }
                if (y == 10)  //y=m=1第二点与第三点为最大值
                {
                    y = 2;
                    m = 2;
                    h = Dis[0] * Math.Sin(Ang1);
                    h1 = Dis[0] * Math.Cos(Ang1);
                    D1 = 150 / Math.Tan(Ang1) + 200;
                    D11 = 150 / Math.Sin(Ang2);
                    D2 = 150 / Math.Tan(Ang2) + 200;
                    D22 = Dis [2];
                }
                
                #endregion
               #region  高于150米解决方案
                c = 3;
                List<PointLatLng> points = new List<PointLatLng>();

                if (h >= 150)//判断拐的次数

                {
                    XL = (arr[0, m] - arr[0, y - 1]) * 10000;
                    YL = (arr[1, m] - arr[1, y - 1]) * 10000;
                    FX = XL / (Math.Sqrt(XL * XL + YL * YL));
                    FY = YL / (Math.Sqrt(XL * XL + YL * YL));
                    FX = FX / 10000;
                    FY = FY / 10000;
                    lat1 = (arr[0, m] + FX) * Math.PI / 180.0;
                    lat2 = (arr[0, m]) * Math.PI / 180.0;
                    lng1 = (arr[1, m] + FY) * Math.PI / 180.0;
                    lng2 = (arr[1, m]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DX= 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DX = DX * R;
                    DX = Math.Round(DX * 10000) / 10000;
                    DX = DX * 1000;  // x方向单位向量长度
                    FXXX = FX * h1 / DX;
                    FYYY = FY * h1 / DX;

                    if (M0 == 0) //第一点和第二点为最大值
                    {
                        m = 0;
                    }
                    if (M2 == 0) //第三点与第一点为最大值
                    {
                        m = 0;
                    }
                    if (M1 == 0) //第二点与第三点为最大值
                    {
                        m = 1;
                    }

                  
                    arrmid[0, 0] = arr[0, m] + FXXX; //求出三角形垂足的经纬度
                    arrmid[1, 0] = arr[1, m] + FYYY;

                    if (M0 == 0) //第一点和第二点为最大值
                    {
                        m = 2;
                    }
                    if (M2 == 0) //第三点与第一点为最大值
                    {
                        m = 1;
                    }
                    if (M1 == 0) //第二点与第三点为最大值
                    {
                        m = 0;
                    }

                    XLL = (arr[0, m] - arrmid[0, 0]) * 10000; //求垂直的单位向量
                    YLL = (arr[1, m] - arrmid[1, 0]) * 10000;
                    FXX = XLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                    FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                    FXX = FXX / 10000;
                    FYY = FYY / 10000;

                    lat1 = (arrmid[0, 0] ) * Math.PI / 180.0;//求垂直单位向量长度
                    lat2 = (arrmid[0, 0]+FXX) * Math.PI / 180.0;
                    lng1 = (arrmid[1, 0] ) * Math.PI / 180.0;
                    lng2 = (arrmid[1, 0]+FYY) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DY = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DY = DY * R;
                    DY = Math.Round(DY * 10000) / 10000;
                    DY = DY * 1000; // Y方向单位向量长度
                    DY = 150 / DY;

                    h = Math.Floor(h / 150) ;//迭代次数

                    DisH[0]=(Dismax + 200) / DX;

                    for (i = 1; i <= h; i=i+2)//横向距离和求其与单位向量的倍数
                    {
                        DisH[i] = (D22-i*D11)*Dismax /D22 +D2 +200;
  
                    }
                    for (i = 2; i <= h; i = i + 2)
                    {
                        DisH[i] = (D22 - i * D11) * Dismax / D22 + D1+200;

                    }

                    for (i = 1; i <= h; i++)
                    {
                        DisH[i] = DisH[i] / DX;
                    }

                    

                    if (M0==0) //第一点和第二点为最大值
                    {
                        m = 0;
                        arrdis[0, 0] = arr[0, m];
                        arrdis[1, 0] = arr[1, m];     
                        y = 0;
                        for (i = 0; i <= (h+1)*2; i=i+1)

                        {
                            if (2==i)
                            {
                                y = i - 1;

                            }
                            if (i>3)
                            {
                                y = i - y - 1;

                            }

                            if (y%2 - 1 == 0)
                            {
                                DisH[m] = -DisH[y];
                            }
                            else 
                            {
                                DisH[m] = DisH[y];
                            }

                                arrdis[0, i + 1] = arrdis[0, i] + DisH[m] * FX;
                                arrdis[1, i + 1] = arrdis[1, i] + DisH[m] * FY;

                                arrdis[0, i + 2] = arrdis[0, i + 1] + DY * FXX;
                                arrdis[1, i + 2] = arrdis[1, i + 1] + DY * FYY;

                                i++;

                        }

                    }
                    if (M2==0) //第三点与第一点为最大值
                    {
                        m = 0;
                        arrdis[0, 0] = arr[0, m];
                        arrdis[1, 0] = arr[1, m];
                        y = 0;
                        for (i = 0; i <= (h + 1) * 2; i = i + 1)
                        {
                            if (2 == i)
                            {
                                y = i - 1;

                            }
                            if (i > 3)
                            {
                                y = i - y - 1;

                            }

                            if (y % 2 - 1 == 0)
                            {
                                DisH[m] = -DisH[y];
                            }
                            else
                            {
                                DisH[m] = DisH[y];
                            }

                            arrdis[0, i + 1] = arrdis[0, i] + DisH[m] * FX;
                            arrdis[1, i + 1] = arrdis[1, i] + DisH[m] * FY;

                            arrdis[0, i + 2] = arrdis[0, i + 1] + DY * FXX;
                            arrdis[1, i + 2] = arrdis[1, i + 1] + DY * FYY;

                            i++;

                        }
   
                    }
                    if (M1==0) //第二点与第三点为最大值
                    {
                        m = 1;
                        arrdis[0, 0] = arr[0, m];
                        arrdis[1, 0] = arr[1, m];

                        y = 0;
                        for (i = 0; i <= (h + 1) * 2; i = i + 1)
                        {
                            m = 0;
                            if (2 == i)
                            {
                                y = i - 1;

                            }
                            if (i > 3)
                            {
                                y = i - y - 1;

                            }

                            if (y % 2 - 1 == 0)
                            {
                                DisH[m] = -DisH[y];
                            }
                            else
                            {
                                DisH[m] = DisH[y];
                            }

                            arrdis[0, i + 1] = arrdis[0, i] + DisH[m] * FX;
                            arrdis[1, i + 1] = arrdis[1, i] + DisH[m] * FY;

                            arrdis[0, i + 2] = arrdis[0, i + 1] + DY * FXX;
                            arrdis[1, i + 2] = arrdis[1, i + 1] + DY * FYY;

                            i++;

                        }

                        
                    }

                    for (m = 0; m <= 2*h+1; m++)
                    {
                        PointLatLng p = new PointLatLng(arrdis[0, m], arrdis[1, m]);
                        points.Add(p);
                        GMapRoute r = new GMapRoute(points, "");
                        r.Stroke = new Pen(Color.Orange, 2);
                        overlay.Routes.Add(r);

                    }
                    hs = (int)h;

                    PointLatLng p1 = new PointLatLng(arrdis[0, 0], arrdis[1, 0]);
                    GMapMarker marker = new GMarkerGoogle(p1, GMarkerGoogleType.green_big_go);

                    PointLatLng p2 = new PointLatLng(arrdis[0, m - 1], arrdis[1, m - 1]);
                    GMapMarker marker1 = new GMarkerGoogle(p2, GMarkerGoogleType.red_big_stop);

                    overlay.Markers.Add(marker);
                    overlay.Markers.Add(marker1);
                    

                }
               #endregion
                else
             {
               #region 低于150米解决方法
                 c = 3;
                    XL = (arr[0, m] - arr[0, y - 1]) * 10000;
                    YL = (arr[1, m] - arr[1, y - 1]) * 10000;
                    FX = XL / (Math.Sqrt(XL * XL + YL * YL));
                    FY = YL / (Math.Sqrt(XL * XL + YL * YL));
                    FX = FX / 10000;
                    FY = FY / 10000;
                    lat1 = (arr[0, m] + FX) * Math.PI / 180.0;
                    lat2 = (arr[0, m]) * Math.PI / 180.0;
                    lng1 = (arr[1, m] + FY) * Math.PI / 180.0;
                    lng2 = (arr[1, m]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    D0 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    D0 = D0 * R;
                    D0 = Math.Round(D0 * 10000) / 10000;
                    D0 = D0 * 1000;
                    FX = FX * 100 / D0;
                    FY = FY * 100 / D0;
                    if (M2 == 0)
                    {
                       
                        arrdis[0, 0] = arr[0, 0];
                        arrdis[1, 0] = arr[1, 0];
                    }
                    else 
                    {
                        m = m+0;
                        arrdis[0, 0] = arr[0, m - 1];
                        arrdis[1, 0] = arr[1, m - 1];
                    }

                    if (M2 == 0)
                    {

                 
                    arrdis[0, 1] = arr[0, 2] + FX;
                    arrdis[1, 1] = arr[1, 2] + FY;
                    }
                    else
                    {
                        m = m+0;
                        arrdis[0, 1] = arr[0, m] + FX;
                        arrdis[1, 1] = arr[1, m] + FY;
                    }
                  
                    
     

                if (m == 1 & y ==1)//第一点和第二点为最大值，找到第三点，求其与第二点的差
                 {
                     y = 3;
                     m = 1;
                     Dis[3] = Dis[1];
                 }
                 if (y == 2 & m == 2)//第二点与第三点为最大值，找到第一点，求其与第三点的差
                 {
                     y = 10;
                     m = 2;
                  
                     Dis[3] = Dis[2];
                 }
                 if (m == 2 & y == 1)//第三点与第一点为最大值,找到第二点，求其与第三点的差
                 {
                     m = 2;
                     y = 2;
                  
                     Dis[3] = Dis[1];
                 }
                 if (y == 10)
                 {
                     y = 1;

                 }
                     
                XLL = (arr[0, y-1] - arr[0, m]) * 10000;
                YLL = (arr[1, y-1] - arr[1, m]) * 10000;
                FXX = XLL / (Math.Sqrt(XLL * XLL+ YLL * YLL));
                FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));                   
              //Ang = (-XL * XLL + -YL * YLL) / (Math.Sqrt(XL * XL + YL * YL) * Math.Sqrt(XLL * XLL + YLL * YLL));
              //Ang = Math.Acos(Ang) * 180 / Math.PI;       
                Angdis = Dis [3] / 2;
                FXX = FXX / 10000;
                FYY = FYY / 10000;             
                lat1 = (arr[0, m] + FX + FXX) * Math.PI / 180.0;
                lat2 = (arr[0, m] + FX) * Math.PI / 180.0;
                lng1 = (arr[1, m] + FY + FYY) * Math.PI / 180.0;
                lng2 = (arr[1, m] + FY) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));               
                D = D * R;
                D = Math.Round(D * 10000) / 10000;
                D = D * 1000;
                Angdis = Angdis / D;
                FXX = Angdis * FXX ;
                FYY = Angdis * FYY ;
                arrdis[0, 2] = arr[0, m] + FX+FXX ;
                arrdis[1, 2] = arr[1, m] + FY+FYY ;

                Dismax = (Dismax / 2 + 200) / D0;
                FXXX = -FX * Dismax * D0 / 100;
                FYYY = -FY * Dismax * D0 / 100;
                arrdis[0, 3] = arr[0, m] + FX + FXX + FXXX;
                arrdis[1, 3] = arr[1, m] + FY + FYY + FYYY;
 
                for (m = 0; m <= 3; m++)
                {
                    PointLatLng p = new PointLatLng(arrdis[0, m], arrdis[1, m]);
                    points.Add(p);
                    GMapRoute r = new GMapRoute(points, "");
                    r.Stroke = new Pen(Color.Orange, 2);
                    overlay.Routes.Add(r);

                }

                PointLatLng p1 = new PointLatLng(arrdis[0, 0], arrdis[1, 0]);
                GMapMarker marker = new GMarkerGoogle(p1, GMarkerGoogleType.green_big_go);

                PointLatLng p2 = new PointLatLng(arrdis[0, m - 1], arrdis[1, m - 1]);
                GMapMarker marker1 = new GMarkerGoogle(p2, GMarkerGoogleType.red_big_stop);

                overlay.Markers.Add(marker);
                overlay.Markers.Add(marker1);


                    #endregion
             }

            }
            if (y == 4)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                c = 4;
                #region  统计全部边长操作
                lat1 = arr[0, 0] * Math.PI / 180.0;
                lat2 = arr[0, y - 1] * Math.PI / 180.0;
                lng1 = arr[1, 0] * Math.PI / 180.0;
                lng2 = arr[1, y - 1] * Math.PI / 180.0;
                double a = lat1 - lat2;
                double b = lng1 - lng2;
                D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                       + Math.Cos(lat1) * Math.Cos(lat2)
                       * Math.Pow(Math.Sin(b / 2), 2)));
                D = D * R;
                D = Math.Round(D * 10000) / 10000;
                D = D * 1000;
                Dis[y - 1] = D;
                #endregion

                #region   findmax操作

                if ((Dis[0] - Dis[1]) > 0)
                {
                    Dismax = Dis[0];
                }
                else
                {
                    Dismax = Dis[1];
                }

                if ((Dismax - Dis[2]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;
                }
                else
                {
                    Dismax = Dis[2];
                }
                if ((Dismax - Dis[3]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;
                }
                else
                {
                    Dismax = Dis[3];
                }

                #endregion

                #region   
                M0 = Dismax - Dis[0];
                M1 = Dismax - Dis[1];
                M2 = Dismax - Dis[2];
                M3 = Dismax - Dis[3];

                if (M0 == 0)
                {
                    m = 0;
                    y = 2;
                }

                if (M1 == 0)
                {
                    m = 1;
                    y = 3;
                }
                if (M2 == 0)
                {
                    m = 2;
                    y = 0;
                }
                if (M3 == 0)
                {
                    m = 3;
                    y = 1;
                }
                lat1 = arr[0, m] * Math.PI / 180.0; //求出斜边长
                lat2 = arr[0, y] * Math.PI / 180.0;
                lng1 = arr[1, m] * Math.PI / 180.0;
                lng2 = arr[1, y] * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2)
                       + Math.Cos(lat1) * Math.Cos(lat2)
                       * Math.Pow(Math.Sin(Db / 2), 2)));
                D = D * R;
                D = Math.Round(D * 10000) / 10000;
                D = D * 1000;

                Dis[4] = D;//斜边长
                if (M0 == 0)
                {
                    m = 0;
                    y = 2;
                    Ang0 = (Dis[0] * Dis[0] + Dis[4] * Dis[4] - Dis[1] * Dis[1]) / (2 * Dis[0] * Dis[4]);// 把四边形分隔成两个三角形并求其6个角度
                    Ang1 = (Dis[0] * Dis[0] + Dis[1] * Dis[1] - Dis[4] * Dis[4]) / (2 * Dis[0] * Dis[1]);
                    Ang2 = (Dis[1] * Dis[1] + Dis[4] * Dis[4] - Dis[0] * Dis[0]) / (2 * Dis[1] * Dis[4]);
                    Ang3 = (Dis[4] * Dis[4] + Dis[2] * Dis[2] - Dis[3] * Dis[3]) / (2 * Dis[4] * Dis[2]);
                    Ang4 = (Dis[3] * Dis[3] + Dis[2] * Dis[2] - Dis[4] * Dis[4]) / (2 * Dis[3] * Dis[2]);
                    Ang5 = (Dis[3] * Dis[3] + Dis[4] * Dis[4] - Dis[2] * Dis[2]) / (2 * Dis[3] * Dis[4]);

                    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;

                    Ang0 = Ang0 * Math.PI / 180;
                    Ang1 = Ang1 * Math.PI / 180;
                    Ang2 = Ang2 * Math.PI / 180;
                    Ang3 = Ang3 * Math.PI / 180;
                    Ang4 = Ang4 * Math.PI / 180;
                    Ang5 = Ang5 * Math.PI / 180;
                }

                //编角度
                if (M1 == 0)
                {
                    m = 1;
                    y = 3;
                    Ang0 = (Dis[1] * Dis[1] + Dis[4] * Dis[4] - Dis[2] * Dis[2]) / (2 * Dis[1] * Dis[4]);// 把四边形分隔成两个三角形并求其6个角度1
                    Ang1 = (Dis[2] * Dis[2] + Dis[1] * Dis[1] - Dis[4] * Dis[4]) / (2 * Dis[2] * Dis[1]);
                    Ang2 = (Dis[2] * Dis[2] + Dis[4] * Dis[4] - Dis[1] * Dis[1]) / (2 * Dis[2] * Dis[4]);
                    Ang3 = (Dis[4] * Dis[4] + Dis[3] * Dis[3] - Dis[0] * Dis[0]) / (2 * Dis[3] * Dis[4]);
                    Ang4 = (Dis[3] * Dis[3] + Dis[0] * Dis[0] - Dis[4] * Dis[4]) / (2 * Dis[3] * Dis[0]);
                    Ang5 = (Dis[0] * Dis[0] + Dis[4] * Dis[4] - Dis[3] * Dis[3]) / (2 * Dis[0] * Dis[4]);

                    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;

                    Ang0 = Ang0 * Math.PI / 180;
                    Ang1 = Ang1 * Math.PI / 180;
                    Ang2 = Ang2 * Math.PI / 180;
                    Ang3 = Ang3 * Math.PI / 180;
                    Ang4 = Ang4 * Math.PI / 180;
                    Ang5 = Ang5 * Math.PI / 180;

                }
                if (M2 == 0)
                {
                    m = 2;
                    y = 0;
                    Ang0 = (Dis[2] * Dis[2] + Dis[4] * Dis[4] - Dis[3] * Dis[3]) / (2 * Dis[2] * Dis[4]);// 把四边形分隔成两个三角形并求其6个角度
                    Ang1 = (Dis[2] * Dis[2] + Dis[3] * Dis[3] - Dis[4] * Dis[4]) / (2 * Dis[2] * Dis[3]);
                    Ang2 = (Dis[3] * Dis[3] + Dis[4] * Dis[4] - Dis[2] * Dis[2]) / (2 * Dis[3] * Dis[4]);
                    Ang3 = (Dis[4] * Dis[4] + Dis[0] * Dis[0] - Dis[1] * Dis[1]) / (2 * Dis[4] * Dis[0]);
                    Ang4 = (Dis[0] * Dis[0] + Dis[1] * Dis[1] - Dis[4] * Dis[4]) / (2 * Dis[0] * Dis[1]);
                    Ang5 = (Dis[1] * Dis[1] + Dis[4] * Dis[4] - Dis[0] * Dis[0]) / (2 * Dis[1] * Dis[4]);

                    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;

                    Ang0 = Ang0 * Math.PI / 180;
                    Ang1 = Ang1 * Math.PI / 180;
                    Ang2 = Ang2 * Math.PI / 180;
                    Ang3 = Ang3 * Math.PI / 180;
                    Ang4 = Ang4 * Math.PI / 180;
                    Ang5 = Ang5 * Math.PI / 180;

                }
                if (M3 == 0)
                {
                    m = 3;
                    y = 1;
                    Ang0 = (Dis[3] * Dis[3] + Dis[4] * Dis[4] - Dis[0] * Dis[0]) / (2 * Dis[3] * Dis[4]);// 把四边形分隔成两个三角形并求其6个角度
                    Ang1 = (Dis[0] * Dis[0] + Dis[3] * Dis[3] - Dis[4] * Dis[4]) / (2 * Dis[0] * Dis[3]);
                    Ang2 = (Dis[0] * Dis[0] + Dis[4] * Dis[4] - Dis[3] * Dis[3]) / (2 * Dis[0] * Dis[4]);
                    Ang3 = (Dis[4] * Dis[4] + Dis[1] * Dis[1] - Dis[2] * Dis[2]) / (2 * Dis[4] * Dis[1]);
                    Ang4 = (Dis[1] * Dis[1] + Dis[2] * Dis[2] - Dis[4] * Dis[4]) / (2 * Dis[1] * Dis[2]);
                    Ang5 = (Dis[2] * Dis[2] + Dis[4] * Dis[4] - Dis[1] * Dis[1]) / (2 * Dis[2] * Dis[4]);

                    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;

                    Ang0 = Ang0 * Math.PI / 180;
                    Ang1 = Ang1 * Math.PI / 180;
                    Ang2 = Ang2 * Math.PI / 180;
                    Ang3 = Ang3 * Math.PI / 180;
                    Ang4 = Ang4 * Math.PI / 180;
                    Ang5 = Ang5 * Math.PI / 180;

                }
                //转坐标
                Ang = Ang0 + Ang5 ;
   
                ver[0, 0] = 0;
                ver[1, 0] = 0;

                if (M0 == 0)
                {

                    ver[0, 1] = Dis[0];
                    ver[1, 1] = 0;

                    ver[0, 2] = Dis[4] * Math.Cos(Ang0);
                    ver[1, 2] = Dis[4] * Math.Sin(Ang0);

                    if (Ang > 1.57)
                    {
                        Ang = 3.14 - Ang;
                        ver[0, 3] = -Dis[3] * Math.Cos(Ang);
                        ver[1, 3] = -Dis[3] * Math.Sin(Ang);
                        h1 = -ver[1, 3];
                    }
                    else
                    {
                        Ang = Ang + 0;
                        ver[0, 3] = Dis[3] * Math.Cos(Ang);
                        ver[1, 3] = Dis[3] * Math.Sin(Ang);
                        h1 = ver[1, 3];
                    }
                }

                if (M1 == 0)
                {
                  
                    ver[0, 1] = Dis[1];
                    ver[1, 1] = 0;

                    ver[0, 2] = Dis[4] * Math.Cos(Ang0);
                    ver[1, 2] = Dis[4] * Math.Sin(Ang0);

                    if (Ang > 1.57)
                    {
                        Ang = 3.14 - Ang;
                        ver[0, 3] = -Dis[0] * Math.Cos(Ang);
                        ver[1, 3] = -Dis[0] * Math.Sin(Ang);
                        h1 = -ver[1, 3];

                    }
                    else
                    {
                        Ang = Ang + 0;
                        ver[0, 3] = Dis[0] * Math.Cos(Ang);
                        ver[1, 3] = Dis[0] * Math.Sin(Ang);
                        h1 = ver[1, 3];
                    }
                   
                    
                   
                }
                if (M2 == 0)
                {
        
                    ver[0, 1] = Dis[2];
                    ver[1, 1] = 0;

                    ver[0, 2] = Dis[4] * Math.Cos(Ang0);
                    ver[1, 2] = Dis[4] * Math.Sin(Ang0);

                    if (Ang > 1.57)
                    {
                        Ang = 3.14 - Ang;
                        ver[0, 3] = -Dis[1] * Math.Cos(Ang);
                        ver[1, 3] = -Dis[1] * Math.Sin(Ang);
                        h1 = -ver[1, 3];
                    }
                    else
                    {
                        Ang = Ang + 0;
                        ver[0, 3] = Dis[1] * Math.Cos(Ang);
                        ver[1, 3] = Dis[1] * Math.Sin(Ang);
                        h1 = ver[1, 3];
                    }

                    
                }
                if (M3 == 0)
                {
                    ver[0, 1] = Dis[3];
                    ver[1, 1] = 0;

                    ver[0, 2] = Dis[4] * Math.Cos(Ang0);
                    ver[1, 2] = Dis[4] * Math.Sin(Ang0);

                    if (Ang > 1.57)
                    {
                        Ang = 3.14 - Ang;
                        ver[0, 3] = -Dis[2] * Math.Cos(Ang);
                        ver[1, 3] = -Dis[2] * Math.Sin(Ang);
                        h1 = -ver[1, 3];
                    }
                    else
                    {
                        Ang = Ang + 0;
                        ver[0, 3] = Dis[2] * Math.Cos(Ang);
                        ver[1, 3] = Dis[2] * Math.Sin(Ang);
                        h1 = ver[1, 3];
                    }
                }


                //定义左右两个数组分别为
                double[,] X1 = new double[2, 100];
                double[,] X2 = new double[2, 100];
                //判断交点个数

                

                h2 = ver[1, 2];
       
                h1 = Math.Floor(h1 / 150);//与第一条线交点次数
                h2 = Math.Floor(h2 / 150);//与第二条线交点次数
                if (h2 > h1)   //判断总次数
                {
                    hs = (int)h2;
                }
                else 
                {
                    hs = (int)h1;
                }
                
                //求x方向向量
                if (M0 == 0)
                {
                    m = 0;// m和y用来确定x方向起始
                    y = 1;
                    hh = 0;//求垂足
                    h11 = 2;//求y方向
                    i = 3;//求y1
                    h22 = 2;//求y2
                    i1 = 3;
                    i11 = 1;

                }

                if (M1 == 0)
                {
                    m = 1;
                    y = 2;
                    hh =1;
                    h11 = 3;
                    i = 0;
                    h22 = 3;
                    i1= 0;
                    i11 = 2;
                }
                if (M2 == 0)
                {
                    m = 2;
                    y = 3;
                    hh = 2;
                    h11 = 0;
                    i = 1;
                    h22 = 0;
                    i1 = 1;
                    i11 = 3;
                   
                }
                if (M3 == 0)
                {
                    m = 3;
                    y = 0;
                    hh = 3;
                    h11 = 1;
                    i = 2;
                    h22 = 1;
                    i1 = 2;
                    i11 = 0;
                }



                // x方向单位向量长度
                XL = (arr[0, m] - arr[0, y]) * 10000;
                YL = (arr[1, m] - arr[1, y]) * 10000;
                FX = XL / (Math.Sqrt(XL * XL + YL * YL));
                FY = YL / (Math.Sqrt(XL * XL + YL * YL));
                FX = -FX / 10000;   // x方向单位向量
                FY = -FY / 10000;   // x方向单位向量
                lat1 = (arr[0, 0] + FX) * Math.PI / 180.0;
                lat2 = (arr[0, 0]) * Math.PI / 180.0;
                lng1 = (arr[1, 0] + FY) * Math.PI / 180.0;
                lng2 = (arr[1, 0]) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                DX = DX * R;
                DX = Math.Round(DX * 10000) / 10000;
                DX = DX * 1000;  // x方向单位向量长度



                h = Dis[4] * Math.Cos(Ang0);//求垂足
                h = h / DX;

                arrmid[0, 0] = arr[0, hh] + h * FX;
                arrmid[1, 0] = arr[1, hh] + h * FY;
                arrdis[0, 1] = arrmid[0, 0];
                arrdis[1, 1] = arrmid[1, 0];


                //y方向单位向量
                XLL = (arrmid[0, 0] - arr[0, (int)h11]) * 10000;
                YLL = (arrmid[1, 0] - arr[1, (int)h11]) * 10000;
                FXX = XLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                FXX = FXX / 10000;   // y方向单位向量
                FYY = FYY / 10000;   // y方向单位向量

                lat1 = (arrmid[0, 0] + FXX) * Math.PI / 180.0;
                lat2 = (arrmid[0, 0]) * Math.PI / 180.0;
                lng1 = (arrmid[1, 0] + FYY) * Math.PI / 180.0;
                lng2 = (arrmid[1, 0]) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                DY = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                DY = DY * R;
                DY = Math.Round(DY * 10000) / 10000;
                DY = DY * 1000;  // y方向单位向量长度
                DY = 150 / DY;//y方向纵向的倍数


                double y1;
                double y11;
                double y1x;
                double y1y;
                double y1xx;
                double y1yy;
                double y2;
                double y22;
                double y2x;
                double y2y;
                double y2xx;
                double y2yy;
                double y3;
                double y33;
                double y3x;
                double y3y;
                double y3xx;
                double y3yy;




                //y1方向单位向量
                y1x = (arr[0, i] - arr[0, m]) * 10000;
                y1y = (arr[1, i] - arr[1, m]) * 10000;
                y1xx = y1x / (Math.Sqrt(y1x * y1x + y1y * y1y));
                y1yy = y1y / (Math.Sqrt(y1x * y1x + y1y * y1y));
                y1xx = y1xx / 10000;
                y1yy = y1yy / 10000;
                lat1 = (arr[0, m] + y1xx) * Math.PI / 180.0;
                lat2 = (arr[0, m]) * Math.PI / 180.0;
                lng1 = (arr[1, m] + y1yy) * Math.PI / 180.0;
                lng2 = (arr[1, m]) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                y1 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                y1 = y1 * R;
                y1 = Math.Round(y1 * 10000) / 10000;
                y1 = y1 * 1000;

                //y2方向单位向量
                y2x = (arr[0, i] - arr[0, (int)h22]) * 10000;
                y2y = (arr[1, i] - arr[1, (int)h22]) * 10000;
                y2xx = y2x / (Math.Sqrt(y2x * y2x + y2y * y2y));
                y2yy = y2y / (Math.Sqrt(y2x * y2x + y2y * y2y));
                y2xx = y2xx / 10000;
                y2yy = y2yy / 10000;
                lat1 = (arr[0, i] + y2xx) * Math.PI / 180.0;
                lat2 = (arr[0, i]) * Math.PI / 180.0;
                lng1 = (arr[1, i] + y2yy) * Math.PI / 180.0;
                lng2 = (arr[1, i]) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                y2 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                y2 = y2 * R;
                y2 = Math.Round(y2 * 10000) / 10000;
                y2 = y2 * 1000;


                //y3方向单位向量
                y3x = (arr[0, (int)h22] - arr[0, y]) * 10000;
                y3y = (arr[1, (int)h22] - arr[1, y]) * 10000;
                y3xx = y3x / (Math.Sqrt(y3x * y3x + y3y * y3y));
                y3yy = y3y / (Math.Sqrt(y3x * y3x + y3y * y3y));
                y3xx = y3xx / 10000;
                y3yy = y3yy / 10000;
                lat1 = (arr[0, y] + y3xx) * Math.PI / 180.0;
                lat2 = (arr[0, y]) * Math.PI / 180.0;
                lng1 = (arr[1, y] + y3yy) * Math.PI / 180.0;
                lng2 = (arr[1, y]) * Math.PI / 180.0;
                Da = lat1 - lat2;
                Db = lng1 - lng2;
                y3 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                y3 = y3 * R;
                y3 = Math.Round(y3 * 10000) / 10000;
                y3 = y3 * 1000;


                // 转换坐标
                //求和y1的交点经纬度

                if (h2 > h1)   //判断总次数
                {
                    hs = (int)h2;

                    for (i = 1; i <= h1; i++)
                    {
                        if (Ang > 1.57)
                        {
                            Ang = 3.14 - Ang;
                        }
                        else
                        {
                            Ang = Ang + 0;
                        }
                        y11 = i * 150 / Math.Sin(Ang) / y1;//单位倍数
                        X1[0, i] = arr[0, m] + y11 * y1xx;
                        X1[1, i] = arr[1, m] + y11 * y1yy;
                    }
                    y = 1;
                    for (i = (int)h1 + 1; i <= hs; i++)
                    {


                        h = (h1 + 1) * 150 - ver[1, 3];

                        Ang = Ang4 - (3.14 - Ang5 - Ang0);

                        h = h / Math.Sin(Ang);

                        h = h / y2;//得倍数

                        X1[0, (int)h1 + 1] = arr[0, i1] + h * (-y2xx);
                        X1[1, (int)h1 + 1] = arr[1, i1] + h * (-y2yy);

                        y22 = y * 150 / Math.Sin(Ang) / y2 + h;//得倍数
                        X1[0, i + 1] = arr[0, i1] + y22 * (-y2xx);
                        X1[1, i + 1] = arr[1, i1] + y22 * (-y2yy);
                        y++;
                    }

                    for (i = 1; i <= hs; i++)
                    {
                        if (Ang1 > 1.57)
                        {
                            Ang1 = 3.14 - Ang1;
                        }
                        else
                        {
                            Ang1 = Ang1 + 0;
                        }
                        y33 = i * 150 / Math.Sin(Ang1) / y3;//单位倍数
                        X2[0, i] = arr[0, i11] + y33 * y3xx;
                        X2[1, i] = arr[1, i11] + y33 * y3yy;
                    }

                }
         else
                {
                    hs = (int)h1;

                    for (i = 1; i <= hs; i++)
                    {
                        if (Ang > 1.57)
                        {
                            Ang = 3.14 - Ang;
                        }
                        else
                        {
                            Ang = Ang + 0;
                        }
                        y11 = i * 150 / Math.Sin(Ang) / y1;//单位倍数
                        X1[0, i] = arr[0, m] + y11 * y1xx;
                        X1[1, i] = arr[1, m] + y11 * y1yy;
                    }

                    for (i = 1; i <= h2; i++)
                    {

                        if (Ang1 > 1.57)
                        {
                            Ang1 = 3.14 - Ang1;
                        }
                        else
                        {
                            Ang1 = Ang1 + 0;
                        }
                        y33 = i * 150 / Math.Sin(Ang1) / y3;//单位倍数
                        X2[0, i] = arr[0, i11] + y33 * y3xx;
                        X2[1, i] = arr[1, i11] + y33 * y3yy;
                    }
                    y = 1;
                    for (i = (int)h2 + 1; i <= hs; i++)
                    {

                        h = (h2 + 1) * 150 - ver[1, 2];

                        Ang = Ang2 + Ang3 - (3.14 - Ang1);

                        h = h / Math.Sin(Ang);

                        h = h / y2;//得倍数

                        X2[0, (int)h2 + 1] = arr[0, (int)h22] + h * y2xx;
                        X2[1, (int)h2 + 1] = arr[1, (int)h22] + h * y2yy;

                        y22 = y * 150 / Math.Sin(Ang) / y2 + h;//得倍数
                        X2[0, i + 1] = arr[0, (int)h22] + y22 * y2xx;
                        X2[1, i + 1] = arr[1, (int)h22] + y22 * y2yy;
                        y++;
                    }


                }

                arrdis[0, 0] = arr[0, m];
                arrdis[1, 0] = arr[1, m];
                X2[0, 0] = arr[0, i11];
                X2[1, 0] = arr[1, i11];

                y = 0;
                for (i = 0; i <= 2 * hs + 1; i++)
                {


                    arrdis[0, i + 1] = X2[0, y] + FX * 200 / DX;
                    arrdis[1, i + 1] = X2[1, y] + FY * 200 / DX;

                    arrdis[0, i + 2] = arrdis[0, i + 1] + (-FXX) * 150 / DY;
                    arrdis[1, i + 2] = arrdis[1, i + 1] + (-FYY) * 150 / DY;

                    arrdis[0, i + 3] = X1[0, y + 1] + (-FX) * 200 / DX;
                    arrdis[1, i + 3] = X1[1, y + 1] + (-FY) * 200 / DX;

                    arrdis[0, i + 4] = arrdis[0, i + 3] + (-FXX) * 150 / DY;
                    arrdis[1, i + 4] = arrdis[1, i + 3] + (-FYY) * 150 / DY;

                    i = i + 3;
                    y = y + 2;

                }

                    for (m = 0; m <2*hs+2; m++)
                    {
                        PointLatLng p = new PointLatLng(arrdis[0, m], arrdis[1, m]);
                        points.Add(p);
                        GMapRoute r = new GMapRoute(points, "");
                        r.Stroke = new Pen(Color.Orange, 2);
                        overlay.Routes.Add(r);
                    }

                    PointLatLng p1 = new PointLatLng(arrdis[0, 0], arrdis[1, 0]);
                    GMapMarker marker = new GMarkerGoogle(p1, GMarkerGoogleType.green_big_go);

                    PointLatLng p2 = new PointLatLng(arrdis[0, m - 1], arrdis[1, m - 1]);
                    GMapMarker marker1 = new GMarkerGoogle(p2, GMarkerGoogleType.red_big_stop);

                    overlay.Markers.Add(marker);
                    overlay.Markers.Add(marker1);

#endregion
  
                #region 上一种方法

                //if (M0 == 0)//定义起点
                //{
   
                //    Ang0 = (Dis[0] * Dis[0] + Dis[4] * Dis[4] - Dis[1] * Dis[1]) / (2 * Dis[0] * Dis[4]);// 把四边形分隔成两个三角形并求其6个角度
                //    Ang1 = (Dis[0] * Dis[0] + Dis[1] * Dis[1] - Dis[4] * Dis[4]) / (2 * Dis[0] * Dis[1]);
                //    Ang2 = (Dis[1] * Dis[1] + Dis[4] * Dis[4] - Dis[0] * Dis[0]) / (2 * Dis[1] * Dis[4]);
                //    Ang3 = (Dis[4] * Dis[4] + Dis[2] * Dis[2] - Dis[3] * Dis[3]) / (2 * Dis[4] * Dis[2]);
                //    Ang4 = (Dis[3] * Dis[3] + Dis[2] * Dis[2] - Dis[4] * Dis[4]) / (2 * Dis[3] * Dis[2]);
                //    Ang5 = (Dis[3] * Dis[3] + Dis[4] * Dis[4] - Dis[2] * Dis[2]) / (2 * Dis[3] * Dis[4]);

                //    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                //    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                //    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                //    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                //    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                //    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;

                //    Ang0 = Ang0 * Math.PI / 180;
                //    Ang1 = Ang1 * Math.PI / 180;
                //    Ang2 = Ang2 * Math.PI / 180;
                //    Ang3 = Ang3 * Math.PI / 180;
                //    Ang4 = Ang4 * Math.PI / 180;
                //    Ang5 = Ang5 * Math.PI / 180;

                //    D22 = Dis[0] * Math.Sin(Ang0 + Ang5) / Math.Sin(3.14 - (Ang0 + Ang1 + Ang5));//补的三角形右边长
                //    D11 = Dis[0] * Math.Sin(Ang1) / Math.Sin(3.14 - (Ang0 + Ang1 + Ang5));
                  
                    

                //    Ang=Ang0+Ang5;//求两高
                //    if (Ang > 1.57)
                //    {
                //        Ang = 3.14 - Ang;
                //    }
                //    else 
                //    {
                //        Ang = Ang + 0;
                //    }
                //    if (Ang1 > 1.57)
                //    {
                //        Ang1 = 3.14 - Ang1;
                //    }
                //    else
                //    {
                //        Ang1= Ang1 + 0;
                //    }

                //    h1 = Dis[3] * Math.Sin(Ang);
                //    h = Dis[3] * Math.Cos(Ang);
                //    h2 = Dis[1] * Math.Sin(Ang1);
                //    DXB = 150 / Math.Sin(Ang);
                //    D1 = 150 / Math.Tan(Ang)+200;
                //    D2 = 150 / Math.Tan(Ang1)+200;

                //    XL = (arr[0, 0] - arr[0, 1]) * 10000;
                //    YL = (arr[1, 0] - arr[1, 1]) * 10000;
                //    FX = XL / (Math.Sqrt(XL * XL + YL * YL));
                //    FY = YL / (Math.Sqrt(XL * XL + YL * YL));
                //    FX = -FX / 10000;   // x方向单位向量
                //    FY = -FY / 10000;   // x方向单位向量
                //    lat1 = (arr[0, 0] + FX) * Math.PI / 180.0;
                //    lat2 = (arr[0, 0]) * Math.PI / 180.0;
                //    lng1 = (arr[1, 0] + FY) * Math.PI / 180.0;
                //    lng2 = (arr[1, 0]) * Math.PI / 180.0;
                //    Da = lat1 - lat2;
                //    Db = lng1 - lng2;
                //    DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                //    DX = DX * R;
                //    DX = Math.Round(DX * 10000) / 10000;
                //    DX = DX * 1000;  // x方向单位向量长度

                //    FXXX =FX * h / DX;
                //    FYYY =FY * h / DX;

                //    arrmid[0, 0] = arr[0, 0] + FXXX;
                //    arrmid[1, 0] = arr[1, 0] + FYYY;

              
                //    XLL = (arrmid[0, 0] - arr[0, 3]) * 10000;
                //    YLL = (arrmid[1, 0] - arr[1, 3]) * 10000;
                //    FXX = XLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                //    FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                //    FXX = FXX / 10000;   // y方向单位向量
                //    FYY = FYY / 10000;   // y方向单位向量

                //    lat1 = (arrmid[0, 0]+FXX) * Math.PI / 180.0;
                //    lat2 = (arrmid[0, 0]) * Math.PI / 180.0;
                //    lng1 = (arrmid[1, 0]+FYY) * Math.PI / 180.0;
                //    lng2 = (arrmid[1, 0]) * Math.PI / 180.0;
                //    Da = lat1 - lat2;
                //    Db = lng1 - lng2;
                //    DY = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                //    DY = DY * R;
                //    DY = Math.Round(DY * 10000) / 10000;
                //    DY = DY * 1000;  // y方向单位向量长度

                //    DY = 150 / DY;//y方向150 所放大的倍数

                //    h11 = h1;
                //    h22 = h2;

                //    h1 = Math.Floor(h1 / 150);
                //    h2 = Math.Floor(h2 / 150);
                //    if (h2 > h1)
                //    {
                //        h = h1 + 1;
                //        h11 = h11 + 0;

                //    }
                //    else 
                //    {
                //        h = h2 + 1;
                //        h11 = h22;
                //    }

                   

                //    for (i = 1; i <= h; i = i + 2)//横向距离和求其与单位向量的倍数
                //    {
                //        DisH[i] = (D11 - i * DXB) * Dismax / D11 + D2 + 200;

                //    }
                //    for (i = 2; i <= h; i = i + 2)
                //    {
                //        DisH[i] = (D11 - i * DXB) * Dismax / D11 + D1 + 200;

                //    }

                //    for (i = 1; i <= h; i++)
                //    {
                //        DisH[i] = DisH[i] / DX;
                //    }

                //    DisH[0] = (Dismax + 200) / DX;

                //    arrdis[0, 0] = arr[0, 0];
                //    arrdis[1, 0] = arr[1, 0];


                //    hh = (int)h;
                //    Dismax = DisH[hh] * DX; //求解上三角最大边
                //    h11 = hh * 150 - h11;

                //    h11 = h11 / Math.Tan(3.14 - Ang0 - Ang5 - Ang4) + 200;
                //    if (h % 2 - 1 == 0)
                //    {

                //        Dismax = Dismax  - h11-D1;
                //    }

                //    else
                //    {

                //        Dismax = Dismax  - D1-h11;  //求出上三角最长边
                //    }


                //    D11 = Dismax * Math.Sin(3.14 - Ang0 - Ang5 - Ang4) / Math.Sin(Ang4);

                //    D2 = 150 / Math.Tan(3.14 - Ang0 - Ang5 - Ang4) + 200;


                //    if (h2 > h1)
                //    {
                //        h = h2;
                //        hs = (int)h1 + 1;

                //    }
                //    else
                //    {
                //        h = h1;
                //        hs = (int)h2 + 1;
                //    }

                //    if (h % 2 - 1 == 0)
                //    {
                       
                //        for (i = 1; i <= h - hs; i = i + 2)//横向距离和求其与单位向量的倍数
                //        {
                //            DisHH[i] = (D11 - i * DXB) * Dismax / D11 + D1+ 200;

                //        }
                //        for (i = 2; i <= h - hs; i = i + 2)
                //        {
                //            DisHH[i] = (D11 - i * DXB) * Dismax / D11 + D2 + 200;

                //        }
                //    }

                //    else
                //    {

                //        DisHH[1] = (D11 - 1 * DXB) * Dismax / D11 + D1 + D2;
                //        for (i = 2; i <= h - hs; i = i + 2)//横向距离和求其与单位向量的倍数
                //        {
                //            DisHH[i] = (D11 - i * DXB) * Dismax / D11 + D1 + 200;

                //        }
                //        for (i = 3; i <= h - hs; i = i + 2)
                //        {
                //            DisHH[i] = (D11 - i * DXB) * Dismax / D11 + D2 + 200;

                //        }
                //    }

                //    for (i = 1; i <= h - hs; i++)
                //    {
                //        DisHH[i] = DisHH[i] / DX;
                //    }
                //    hh = hs;

                //    for (i = 1; i <= h - hs; i++)
                //    {

                //        DisH[hh + 1] = DisHH[i];
                //        hh++;
                //    }

                //    y = 0;

                //    for (i = 0; i <= (h + 1) * 2; i = i + 1)
                //    {
                //        if (2 == i)
                //        {
                //            y = i - 1;

                //        }
                //        if (i > 3)
                //        {
                //            y = i - y - 1;

                //        }

                //        if (y % 2 - 1 == 0)
                //        {
                //            DisH[m] = -DisH[y];
                //        }
                //        else
                //        {
                //            DisH[m] = DisH[y];
                //        }

                //        arrdis[0, i + 1] = arrdis[0, i] + DisH[m] * (FX);
                //        arrdis[1, i + 1] = arrdis[1, i] + DisH[m] * (FY);

                //        arrdis[0, i + 2] = arrdis[0, i + 1] + DY * (-FXX);
                //        arrdis[1, i + 2] = arrdis[1, i + 1] + DY * (-FYY);

                //        i++;

                //    }
                //}
      

                //for (m = 0; m <= 2*(h+1)-1; m++)
                //{
                //    PointLatLng p = new PointLatLng(arrdis[0, m], arrdis[1, m]);
                //    points.Add(p);
                //    GMapRoute r = new GMapRoute(points, "");
                //    r.Stroke = new Pen(Color.Orange, 4);
                //    overlay.Routes.Add(r);

                //}
              #endregion

    
            }
            if (y == 5)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                c = 5;
                #region  统计全部边长操作
                lat1 = arr[0, 0] * Math.PI / 180.0;
                lat2 = arr[0, y - 1] * Math.PI / 180.0;
                lng1 = arr[1, 0] * Math.PI / 180.0;
                lng2 = arr[1, y - 1] * Math.PI / 180.0;
                double a = lat1 - lat2;
                double b = lng1 - lng2;
                D = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                       + Math.Cos(lat1) * Math.Cos(lat2)
                       * Math.Pow(Math.Sin(b / 2), 2)));
                D = D * R;
                D = Math.Round(D * 10000) / 10000;
                D = D * 1000;
                Dis[y - 1] = D;
                #endregion

                #region  

                if ((Dis[0] - Dis[1]) > 0)
                {
                    Dismax = Dis[0];
                }
                else
                {
                    Dismax = Dis[1];
                }

                if ((Dismax - Dis[2]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;
                }
                else
                {
                    Dismax = Dis[2];
                }
                if ((Dismax - Dis[3]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;
                }
                else
                {
                    Dismax = Dis[3];
                }
                if ((Dismax - Dis[4]) > 0)
                {
                    Dismid = Dismax;
                    Dismax = Dismid;
                }
                else
                {
                    Dismax = Dis[4];
                }

               
                M0 = Dismax - Dis[0];
                M1 = Dismax - Dis[1];
                M2 = Dismax - Dis[2];
                M3 = Dismax - Dis[3];
                M4 = Dismax - Dis[4];



                
                  //求解两斜边
                    lat1 = (arr[0, 0]) * Math.PI / 180.0;
                    lat2 = (arr[0, 2]) * Math.PI / 180.0;
                    lng1 = (arr[1, 0]) * Math.PI / 180.0;
                    lng2 = (arr[1, 2]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DX = DX * R;
                    DX = Math.Round(DX * 10000) / 10000;
                    DX = DX * 1000;  // x方向单位向量长度
                    Dis[5] = DX;
                    lat1 = (arr[0, 0]) * Math.PI / 180.0;
                    lat2 = (arr[0, 3]) * Math.PI / 180.0;
                    lng1 = (arr[1, 0]) * Math.PI / 180.0;
                    lng2 = (arr[1, 3]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DX = DX * R;
                    DX = Math.Round(DX * 10000) / 10000;
                    DX = DX * 1000;  // x方向单位向量长度
                    Dis[6] = DX;

                    //编角度
                    Ang0 = (Dis[0] * Dis[0] + Dis[5] * Dis[5] - Dis[1] * Dis[1]) / (2 * Dis[0] * Dis[5]);// 把五边形分隔成三个三角形并求9个角度
                    Ang1 = (Dis[0] * Dis[0] + Dis[1] * Dis[1] - Dis[5] * Dis[5]) / (2 * Dis[0] * Dis[1]);
                    Ang2 = (Dis[1] * Dis[1] + Dis[5] * Dis[5] - Dis[0] * Dis[0]) / (2 * Dis[1] * Dis[5]);
                    Ang3 = (Dis[5] * Dis[5] + Dis[2] * Dis[2] - Dis[6] * Dis[6]) / (2 * Dis[5] * Dis[2]);
                    Ang4 = (Dis[6] * Dis[6] + Dis[2] * Dis[2] - Dis[5] * Dis[5]) / (2 * Dis[6] * Dis[2]);
                    Ang5 = (Dis[5] * Dis[5] + Dis[6] * Dis[6] - Dis[2] * Dis[2]) / (2 * Dis[5] * Dis[6]);
                    Ang6 = (Dis[3] * Dis[3] + Dis[6] * Dis[6] - Dis[4] * Dis[4]) / (2 * Dis[3] * Dis[6]);
                    Ang7 = (Dis[3] * Dis[3] + Dis[4] * Dis[4] - Dis[6] * Dis[6]) / (2 * Dis[3] * Dis[4]);
                    Ang8 = (Dis[4] * Dis[4] + Dis[6] * Dis[6] - Dis[3] * Dis[3]) / (2 * Dis[4] * Dis[6]);


                    Ang0 = Math.Acos(Ang0) * 180 / Math.PI;
                    Ang1 = Math.Acos(Ang1) * 180 / Math.PI;
                    Ang2 = Math.Acos(Ang2) * 180 / Math.PI;
                    Ang3 = Math.Acos(Ang3) * 180 / Math.PI;
                    Ang4 = Math.Acos(Ang4) * 180 / Math.PI;
                    Ang5 = Math.Acos(Ang5) * 180 / Math.PI;
                    Ang6 = Math.Acos(Ang6) * 180 / Math.PI;
                    Ang7 = Math.Acos(Ang7) * 180 / Math.PI;
                    Ang8 = Math.Acos(Ang8) * 180 / Math.PI;

                    Ang0 = Ang0 * Math.PI / 180;
                    Ang1 = Ang1 * Math.PI / 180;
                    Ang2 = Ang2 * Math.PI / 180;
                    Ang3 = Ang3 * Math.PI / 180;
                    Ang4 = Ang4 * Math.PI / 180;
                    Ang5 = Ang5 * Math.PI / 180;
                    Ang6 = Ang6 * Math.PI / 180;
                    Ang7 = Ang7 * Math.PI / 180;
                    Ang8 = Ang8 * Math.PI / 180;


                    //转换坐标系
                    Ang = Ang0 + Ang5 + Ang8;
                    if(Ang >1.57)
                    {
                        Ang =3.14-Ang ;
                        ver[0, 4] = 0-(Dis[4] * Math.Cos(Ang));
                        ver[1, 4] = Dis[4] * Math.Sin(Ang);
                    }
                    else 
                    {
                        Ang = Ang + 0;
                        ver[0, 4] = Dis[4] * Math.Cos(Ang);
                        ver[1, 4] = Dis[4] * Math.Sin(Ang);
                    }

                    ver[0, 0] = 0;
                    ver[1, 0] = 0;

                    ver[0, 1] = Dis [0];
                    ver[1, 1] = 0;

                    ver[0, 2] = Dis[5]*Math .Cos(Ang0);
                    ver[1, 2] = Dis[5] * Math.Sin(Ang0);

                    ver[0, 3] = Dis[6] * Math.Cos(Ang0+Ang5 );
                    ver[1, 3] = Dis[6] * Math.Sin(Ang0+Ang5 );

                    

                    //定义左右两个数组分别为
                    double[,] X1 = new double[2,100];
                    double[,] X2 = new double[2,100];
                    //判断交点个数

                    h1 = ver[1, 4];
                    h2 = ver[1, 2];
                    h  = ver[1, 3];

                    h1 = Math.Floor(h1 / 150);//与第一条线交点次数
                    h2 = Math.Floor(h2 / 150);//与第二条线交点次数
                    h = Math.Floor(h / 150);// 判断总次数
                    hs =(int)h;
                    


                    // x方向单位向量长度
                    XL = (arr[0, 0] - arr[0, 1]) * 10000;
                    YL = (arr[1, 0] - arr[1, 1]) * 10000;
                    FX = XL / (Math.Sqrt(XL * XL + YL * YL));
                    FY = YL / (Math.Sqrt(XL * XL + YL * YL));
                    FX = -FX / 10000;   // x方向单位向量
                    FY = -FY / 10000;   // x方向单位向量
                    lat1 = (arr[0, 0] + FX) * Math.PI / 180.0;
                    lat2 = (arr[0, 0]) * Math.PI / 180.0;
                    lng1 = (arr[1, 0] + FY) * Math.PI / 180.0;
                    lng2 = (arr[1, 0]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DX = DX * R;
                    DX = Math.Round(DX * 10000) / 10000;
                    DX = DX * 1000;  // x方向单位向量长度

                    

                    h = Dis[5]* Math.Cos(Ang0);
                    h = h / DX;

                    arrmid[0, 0] = arr[0, 0] + h * FX;
                    arrmid[1, 0] = arr[1, 0] + h * FY;
 
                   //y方向单位向量
                    XLL = (arrmid[0, 0] - arr[0, 2]) * 10000;
                    YLL = (arrmid[1, 0] - arr[1, 2]) * 10000;
                    FXX = XLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                    FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
                    FXX = -FXX / 10000;   // y方向单位向量
                    FYY = -FYY / 10000;   // y方向单位向量

                    lat1 = (arrmid[0, 0] + FXX) * Math.PI / 180.0;
                    lat2 = (arrmid[0, 0]) * Math.PI / 180.0;
                    lng1 = (arrmid[1, 0] + FYY) * Math.PI / 180.0;
                    lng2 = (arrmid[1, 0]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    DY = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    DY = DY * R;
                    DY = Math.Round(DY * 10000) / 10000;
                    DY = DY * 1000;  // y方向单位向量长度
                    DY = 150 / DY;//y方向纵向的倍数

                    arrdis[0, 0] = arr[0, 0];
                    arrdis[1, 0] = arr[1, 0];
                    

                    
                    double y1;
                    double y11;
                    double y1x;
                    double y1y;
                    double y1xx;
                    double y1yy;
                    double y2;
                    double y22;
                    double y2x;
                    double y2y;
                    double y2xx;
                    double y2yy;
                    double y3;
                    double y33;
                    double y3x;
                    double y3y;
                    double y3xx;
                    double y3yy;
                    double y4;
                    double y44;
                    double y4x;
                    double y4y;
                    double y4xx;
                    double y4yy;

                    //y1方向单位向量
                    y1x = (arr[0, 0] - arr[0, 4]) * 10000;
                    y1y= (arr[1, 0] - arr[1, 4]) * 10000;
                    y1xx = y1x / (Math.Sqrt(y1x * y1x + y1y * y1y));
                    y1yy = y1y / (Math.Sqrt(y1x * y1x + y1y * y1y));
                    y1xx = -y1xx / 10000;  
                    y1yy = -y1yy / 10000;   
                    lat1 = (arr[0, 0] + y1xx) * Math.PI / 180.0;
                    lat2 = (arr[0, 0]) * Math.PI / 180.0;
                    lng1 = (arr[1, 0] + y1yy) * Math.PI / 180.0;
                    lng2 = (arr[1, 0]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    y1 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    y1 = y1 * R;
                    y1 = Math.Round(y1 * 10000) / 10000;
                    y1 = y1 * 1000; 

                    //y2方向单位向量
                    y2x = (arr[0, 4] - arr[0, 3]) * 10000;
                    y2y = (arr[1, 4] - arr[1, 3]) * 10000;
                    y2xx = y2x / (Math.Sqrt(y2x * y2x + y2y * y2y));
                    y2yy = y2y / (Math.Sqrt(y2x * y2x + y2y * y2y));
                    y2xx = -y2xx / 10000;   
                    y2yy = -y2yy / 10000;  
                    lat1 = (arr[0, 4] + y2xx) * Math.PI / 180.0;
                    lat2 = (arr[0, 4]) * Math.PI / 180.0;
                    lng1 = (arr[1, 4] + y2yy) * Math.PI / 180.0;
                    lng2 = (arr[1, 4]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    y2 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    y2 = y2 * R;
                    y2 = Math.Round(y2 * 10000) / 10000;
                    y2 = y2 * 1000;  


                    //y3方向单位向量
                    y3x = (arr[0, 2] - arr[0, 3]) * 10000;
                    y3y = (arr[1, 2] - arr[1, 3]) * 10000;
                    y3xx = y3x / (Math.Sqrt(y3x * y3x + y3y * y3y));
                    y3yy = y3y / (Math.Sqrt(y3x * y3x + y3y * y3y));
                    y3xx = -y3xx / 10000;   
                    y3yy = -y3yy / 10000;   
                    lat1 = (arr[0, 2] + y3xx) * Math.PI / 180.0;
                    lat2 = (arr[0, 2]) * Math.PI / 180.0;
                    lng1 = (arr[1, 2] + y3yy) * Math.PI / 180.0;
                    lng2 = (arr[1, 2]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    y3 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    y3 = y3 * R;
                    y3 = Math.Round(y3 * 10000) / 10000;
                    y3 = y3 * 1000;

                    //y4方向单位向量
                    y4x = (arr[0, 2] - arr[0, 1]) * 10000;
                    y4y = (arr[1, 2] - arr[1, 1]) * 10000;
                    y4xx = y4x / (Math.Sqrt(y4x * y4x + y4y * y4y));
                    y4yy = y4y / (Math.Sqrt(y4x * y4x + y4y * y4y));
                    y4xx = y4xx / 10000;
                    y4yy = y4yy / 10000;
                    lat1 = (arr[0, 2] + y4xx) * Math.PI / 180.0;
                    lat2 = (arr[0, 2]) * Math.PI / 180.0;
                    lng1 = (arr[1, 2] + y4yy) * Math.PI / 180.0;
                    lng2 = (arr[1, 2]) * Math.PI / 180.0;
                    Da = lat1 - lat2;
                    Db = lng1 - lng2;
                    y4 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
                    y4 = y4 * R;
                    y4 = Math.Round(y4 * 10000) / 10000;
                    y4 = y4 * 1000;

                   // 转换坐标

                    X1[0, 0] = arr[0, 0];
                    X1[1, 0] = arr[1, 0];
                    for (i = 1; i <= h1; i++)
                    {
                       // X1[0, i] = ver[0, 0] + (ver[0, 4] - ver[0, 0]) * (150 * i - ver[1, 0]) / (ver[1, 4] - ver[1, 0]);
                       // X1[1, i] = 150 * i;
                        y11 = i*150 / Math.Sin(Ang)/y1;//单位倍数
                        X1[0, i] = arr[0, 0] + y11 * y1xx;
                        X1[1, i] = arr[1, 0] + y11 * y1yy;

                    }

                    y = 1;
                    for (i = (int)h1 + 1; i <= hs; i++)
                    {
                        
                       // X1[0, i] = ver[0, 4] + (ver[0, 3] - ver[0, 4]) * (150 * i - ver[1, 4]) / (ver[1, 3] - ver[1, 4]);
                       // X1[1, i] = 150 * i;

                        h=(h1 + 1)*150- ver[1,4];

                        Ang = Ang0 + Ang5 + Ang8;
                        Ang =Ang7 -(3.14- Ang );
         
                        h= h /Math .Sin (Ang);

                        h = h / y2;//得倍数

                        X1[0, (int)h1 + 1] = arr[0, 4] + h * y2xx;
                        X1[1, (int)h1 + 1] = arr[1, 4] + h * y2yy;

                        y22 = y * 150 / Math.Sin(Ang) / y2 + h;//得倍数
                        X1[0, i + 1] = arr[0, 4] + y22 * y2xx;
                        X1[1, i + 1] = arr[1, 4] + y22 * y2yy;
                        y++;
                    }

                     X2[0, 0] = arr[0, 1];
                     X2[1, 0] = arr[1, 1];
                    for (i = 1; i <= h2; i++)
                    {
                       // X2[0, i] = ver[0, 1] + (ver[0, 2] - ver[0, 1]) * (150 * i - ver[1, 1]) / (ver[1, 2] - ver[1, 1]);
                       // X2[1, i] = 150 * i;
                        if (Ang1 > 1.57)
                        {
                            Ang1 = 3.14 - Ang1;
                        }
                        else
                        {
                            Ang1 = Ang1 + 0;
                        }
                        y44 = i * 150 / Math.Sin(Ang1) / y4;//单位倍数
                        X2[0, i] = arr[0, 1] + y44 * y4xx;
                        X2[1, i] = arr[1, 1] + y44 * y4yy;
                    }

                    y = 1;
                    for (i = (int)h2 + 1; i <= hs; i++)
                    {

                      // X2[0, i] = ver[0, 2] + (ver[0, 3] - ver[0, 2]) * (150 * i - ver[1, 2]) / (ver[1, 3] - ver[1, 2]);
                      // X2[1, i] = 150 * i;

                        h = (h2 + 1) * 150 - ver[1, 2];
                       
                        Ang = Ang2 + Ang3 - (3.14 - Ang1);
                       
                        h = h / Math.Sin(Ang);

                        h = h / y3;//得倍数

                        X2[0, (int)h2 + 1] = arr[0, 2] + h * y3xx;
                        X2[1, (int)h2 + 1] = arr[1, 2] + h * y3yy;

                        y33 = y * 150 / Math.Sin(Ang) / y3 + h;//得倍数
                        X2[0, i + 1] = arr[0, 2] + y33 * y3xx;
                        X2[1, i + 1] = arr[1, 2] + y33 * y3yy;
                        y++;
                    }

                    arrdis[0, 0] = arr[0, 0];
                    arrdis[1, 0] = arr[1, 0];

                    y = 0;
                    for (i = 0; i <= 2 * hs + 1; i++)
                    {

                        arrdis[0, i + 1] = X2[0, y] + FX * 200 / DX;
                        arrdis[1, i + 1] = X2[1, y] + FY * 200 / DX;

                        arrdis[0, i + 2] = arrdis[0, i+1] + FXX * 150 / DY;
                        arrdis[1, i + 2] = arrdis[1, i+1] + FYY * 150 / DY;

                        arrdis[0, i + 3] = X1[0, y + 1] + (-FX) * 200 / DX;
                        arrdis[1, i + 3] = X1[1, y + 1] + (-FY) * 200 / DX;

                        arrdis[0, i + 4] = arrdis[0, i + 3] + FXX * 150 / DY;
                        arrdis[1, i + 4] = arrdis[1, i + 3] + FYY * 150 / DY;

                        i = i + 3;
                        y = y + 2;
             
                    }

                   for (m = 0; m <= 2 * hs + 1; m++)
                        {
                            PointLatLng p = new PointLatLng(arrdis[0, m], arrdis[1, m]);
                            points.Add(p);
                            GMapRoute r = new GMapRoute(points, "");
                            r.Stroke = new Pen(Color.Orange, 2);
                            overlay.Routes.Add(r);
                        }
                   PointLatLng p1 = new PointLatLng(arrdis[0, 0], arrdis[1, 0]);
                   GMapMarker marker = new GMarkerGoogle(p1, GMarkerGoogleType.green_big_go);

                   PointLatLng p2 = new PointLatLng(arrdis[0, m - 1], arrdis[1, m - 1]);
                   GMapMarker marker1 = new GMarkerGoogle(p2, GMarkerGoogleType.red_big_stop);

                   overlay.Markers.Add(marker);
                   overlay.Markers.Add(marker1);
            }

                #endregion

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            overlay.Routes.Clear();
            overlay.Polygons.Clear();
            overlay.Markers.Clear();
            list.Clear();
            for (i = 0; i <= 7; i++)
            {
                tem[0, i] = 0;
                tem[1, i] = 0;

            }
            i = 0;
            for (i = 0; i <= 7; i++)
            {
                arr[0, i] = 0;
                arr[1, i] = 0;

            }
            i = 0;
            y = 0;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            List<PointLatLng> points = new List<PointLatLng>();
            overlay.Routes.Clear();
            overlay.Markers.Clear();
            
            // 正北方向单位向量
            XLL = (34.0000- 34.1111) * 10000;
            YLL = (113.0000 - 113.0000) * 10000;
            FXX = XLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
            FYY = YLL / (Math.Sqrt(XLL * XLL + YLL * YLL));
            FXX = FXX / 10000;   //正北方向单位X向量
            FYY = FYY / 10000;   //正北方向单位Y向量

            lat1 = (34.0000+ FXX) * Math.PI / 180.0;
            lat2 = (34.0000) * Math.PI / 180.0;
            lng1 = (113.0000 + FYY) * Math.PI / 180.0;
            lng2 = (113.0000) * Math.PI / 180.0;
            Da = lat1 - lat2;
            Db = lng1 - lng2;
            DY = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
            DY = DY * R;
            DY = Math.Round(DY * 10000) / 10000;
            DY = DY * 1000;  
            DY = 150 / DY;
            
          



            // 正东方向单位向量
            XL = (34.0000 - 34.0000) * 10000;
            YL = (113.1111 - 113.0000) * 10000;
            FX = XL / (Math.Sqrt(XL * XL + YL * YL));
            FY = YL / (Math.Sqrt(XL * XL + YL * YL));
            FX = FX / 10000;   //正南方向单位X向量
            FY = FY / 10000;    //正南方向单位y向量
            lat1 = (34.0000 + FX) * Math.PI / 180.0;
            lat2 = (34.0000) * Math.PI / 180.0;
            lng1 = (113.1111 + FY) * Math.PI / 180.0;
            lng2 = (113.1111) * Math.PI / 180.0;
            Da = lat1 - lat2;
            Db = lng1 - lng2;
            DX = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Da / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(Db / 2), 2)));
            DX = DX * R;
            DX = Math.Round(DX * 10000) / 10000;
            DX = DX * 1000;


            if (textBox4.Text == "")
            {
                h1 = 0;
            }
            else 
            {
                h1 = double.Parse(textBox4.Text) / DY;//倍数南
               
            }

            if (textBox5.Text == "")
            {
                h11 = 0;
            }
            else 
            {
                h11 = double.Parse(textBox5.Text) / DY;//倍数北
            }
            if (textBox7.Text == "")
            {
                h2 = 0;

            }
            else 
            {
                h2 = double.Parse(textBox7.Text) / DX;//倍数东
            }
            if (textBox6.Text == "")
            {
                h22 = 0;
            }
            else  
            {
                h22 = double.Parse(textBox6.Text) / DX;//倍数西
            }
           


            for (i = 0; i <= 2 * hs + 1; i++)

            {
                arrdis[0, i] = arrdis[0, i] + h1 * (-FXX);
                arrdis[1, i] = arrdis[1, i] + h1 * (-FYY);
            
            }
            for (i = 0; i <= 2 * hs + 1; i++)
            {
                arrdis[0, i] = arrdis[0, i] + h11 * (FXX);
                arrdis[1, i] = arrdis[1, i] + h11 * (FYY);

            }

            for (i = 0; i <= 2 * hs + 1; i++)
            {
                arrdis[0, i] = arrdis[0, i] + h2 * FX;
                arrdis[1, i] = arrdis[1, i] + h2 * FY;

            }
            for (i = 0; i <= 2 * hs + 1; i++)
            {
                arrdis[0, i] = arrdis[0, i] + h22 * (-FX);
                arrdis[1, i] = arrdis[1, i] + h22 * (-FY);

            }


            for (i = 0; i <= 2 * hs + 1; i++)
            { 
                     arrmid[0, i]  =arrdis[0, i] ;
                     arrmid[1, i] = arrdis[1, i];
            
            }




                for (m = 0; m <= 2 * hs + 1; m++)
                {
                    
                    PointLatLng p = new PointLatLng(arrmid[0, m], arrmid[1, m]);
                    points.Add(p);
                    GMapRoute r = new GMapRoute(points, "");
                    r.Stroke = new Pen(Color.Orange, 2);
                    overlay.Routes.Add(r);
                }


            PointLatLng p1 = new PointLatLng(arrdis[0, 0], arrdis[1, 0]);
            GMapMarker marker = new GMarkerGoogle(p1, GMarkerGoogleType.green_big_go);

            PointLatLng p2 = new PointLatLng(arrdis[0, m - 1], arrdis[1, m - 1]);
            GMapMarker marker1 = new GMarkerGoogle(p2, GMarkerGoogleType.red_big_stop);

            for (i = 0; i <= c; i++)
            {
                arr[0, i] = arr[0, i] + 0;
                arr[1, i] = arr[1, i] + 0;
                PointLatLng p3 = new PointLatLng(arr[0, i], arr[1, i]);
                GMapMarker marker2 = new GMarkerGoogle(p3, GMarkerGoogleType.blue_dot);
                overlay.Markers.Add(marker2);
            }
            

            overlay.Markers.Add(marker);
            overlay.Markers.Add(marker1);
            



        }

  
    }
}

