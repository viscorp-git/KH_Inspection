using Cognex.VisionPro;
using IDMAX_FrameWork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using MegaInformationTechnology;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using ViDi2;
using ViDi2.Runtime;
using WeifenLuo.WinFormsUI.ThemeVS2015;


namespace KH_Inspection
{
    public partial class Base_frm : Form
    {
        Main_frm f1 = new Main_frm();
        Setting_frm_2 f2 = new Setting_frm_2();
        Log_frm f3 = new Log_frm();

        //IO
        public short m_ID;

        //Log
        private static readonly ILog m_CLog = LogManager.GetLogger(typeof(Main_frm));

        public STCSBS500POE m_CCamera;

        uint mCathodeCameraNum;

        //Thread
        Thread m_Thread_Main;
        Thread m_Thread_Log;
        Thread m_Thread_ImageSave;
        Thread m_Thread_System;

        //IO
        Thread m_Thread_IO;
        bool m_blThread_IO;

        bool m_blTrigger = false;

        bool m_blAuto = false;
        bool m_blThread_Main = false;
        bool m_blThread_ImageSave = false;
        bool m_blThread_System = false;
        bool m_blThread_Log = false;

        //Connect
        bool m_blIOFlag = false;
        bool m_blCam = false;
        bool m_blWorkSpaceFlag = false;

        //이미지 취득모드
        bool m_AQmode = false;

        ConcurrentQueue<cls_ImageSave> m_QueueImageSave;
        ConcurrentQueue<cls_Display> m_QueueDisplay;
        ConcurrentQueue<string> m_QueueLog;
        ConcurrentQueue<cls_CSV> m_QueueCSV;

        // Move window variables
        private System.Drawing.Point m_PtDistance;
        private System.Drawing.Point m_PtStart;
        private System.Drawing.Point m_PtEnd;
        private bool m_blMouseDown;


        public class cls_ImageSave
        {
            public Bitmap[] m_BmpArrayImage = new Bitmap[1];

            public string m_strDateTime;

            public bool m_blInspectionResult = false;

            public void Dispose()
            {
                m_strDateTime = "";
            }
        }

        public class cls_CSV
        {
            public double[] m_dlArrayResult;
            public string m_strDateTime;
        }

        public class cls_Display
        {
            public CogImage8Grey m_CogImage8Grey_Top;
            public CogImage8Grey m_CogImage8Grey_Bottom;

            public double[] m_dlArrayResult; // 검사 결과 낱개 수치
            public bool[] m_blArrayResult; // 검사 결과 낱개
            public bool m_blTotalResult; // 검사 결과
            public int m_ntCT; // CT 시간
            public bool m_blInspectionState; // 검사 여부
        }

        public Base_frm()
        {
            InitializeComponent();


            f1.Show(dockPanel1, DockState.Document);
            f2.Show(dockPanel1, DockState.DockRightAutoHide);
            f3.Show(dockPanel1, DockState.DockBottomAutoHide);

        }

        private void Base_frm_Load(object sender, EventArgs e)
        {
            // Form1이 로드될 때 InitializeComponent() 메서드 호출
            InitializeComponent();

            m_QueueImageSave = new ConcurrentQueue<cls_ImageSave>();
            m_QueueDisplay = new ConcurrentQueue<cls_Display>();
            m_QueueLog = new ConcurrentQueue<string>();
            m_QueueCSV = new ConcurrentQueue<cls_CSV>();

            Loading_Start();

            m_blThread_Main = true;
            m_blThread_ImageSave = true;
            m_blThread_System = true;
            m_blThread_IO = true;
            m_blThread_Log = true;

            m_Thread_System = new Thread(Thread_System);
            m_Thread_System.IsBackground = true;
            m_Thread_System.Start();

            m_Thread_Main = new Thread(Thread_Main);
            m_Thread_Main.IsBackground = true;
            m_Thread_Main.Start();

            m_Thread_ImageSave = new Thread(Thread_ImageSave);
            m_Thread_ImageSave.IsBackground = true;
            m_Thread_ImageSave.Start();

            m_Thread_IO = new Thread(Thread_IO);
            m_Thread_IO.IsBackground = true;
            m_Thread_IO.Start();

            m_Thread_Log = new Thread(Thread_Log);
            m_Thread_Log.IsBackground = true;
            m_Thread_Log.Start();
        }

        private void Thread_Log()
        {
            while (m_blThread_Log == true)
            {
                if (m_QueueLog.Count > 0)
                {
                    try
                    {
                        int l_ntCount = m_QueueLog.Count;

                        string l_strText;

                        string l_strFileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                        DirectoryInfo l_DitInfo = new DirectoryInfo(Application.StartupPath + "\\SystemLog");
                        StringBuilder l_SBData = new StringBuilder();

                        if (l_DitInfo.Exists == false)
                            l_DitInfo.Create();

                        for (int i = 0; i < l_ntCount; i++)
                        {
                            if (m_QueueLog.TryDequeue(out l_strText) == true)
                                l_SBData.AppendLine(l_strText);
                        }

                        File.AppendAllText(Application.StartupPath + "\\SystemLog\\" + l_strFileName, l_SBData.ToString(), Encoding.UTF8);
                    }
                    catch (System.Exception exc)
                    {

                    }
                }

                Thread.Sleep(100);
            }
        }

        private void base_Main_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (m_blAuto == true)
            {
                e.Cancel = true;
            }

            if (m_QueueImageSave.Count > 0)
            {
                CustomMessageBox.Show("확인", "Image save count : " + m_QueueImageSave.Count, true);
                //MessageBox.Show("Image save count : " + m_QueueImageSave.Count);
                e.Cancel = true;
            }

            m_blThread_Main = false;
            m_blThread_ImageSave = false;
            m_blThread_IO = false;
            m_blThread_System = false;
            m_blThread_Log = false;

            if (m_CCamera != null)
            {
                if (m_blCam != false)
                {
                    m_CCamera.StreamAcquisitionStop(0);
                }
                //금호는 카메라 1개
                //if (m_blCam2 != false)
                //{
                //    m_CCamera.StreamAcquisitionStop(1);
                //}
                m_CCamera.Release();
            }
            if (m_Thread_Main != null)
                m_Thread_Main.Abort();

            if (m_Thread_ImageSave != null)
                m_Thread_ImageSave.Abort();

            if (m_Thread_IO != null)
                m_Thread_IO.Abort();

            if (m_Thread_System != null)
                m_Thread_System.Abort();

            if (m_Thread_Log != null)
                m_Thread_Log.Abort();
        }

        private void Loading_Start()
        {
            cls_LoadingProvider l_oCLoadingProvider = new cls_LoadingProvider();
            l_oCLoadingProvider.Open(7);
            int l_ntCount = 0;

            while (l_ntCount < 7)
            {
                switch(l_ntCount)
                {
                    case 0:
                        try
                        {
                            cls_Ini l_CIni = new cls_Ini();

                            l_oCLoadingProvider.UpdateProgress("Config-File Loading....", l_ntCount);

                            //cls_Param.m_untCamSerial = "23A9069";
                            //cls_Param.m_untCamSerial = "23AC927";
                            cls_Param.m_untCamSerial = l_CIni.Read_Ini("Cam", "m_untCam1Serial", Application.StartupPath + "\\Config.ini", "xxxxxxxx");
                            //cls_Param.m_untCamSerial = "18C7801";

                            //cls_Param.IO_Name = "DIO000";
                            cls_Param.IO_Name = l_CIni.Read_Ini("System", "IO_Name", Application.StartupPath + "\\Config.ini", "AAA000");

                            //cls_Param.ImageSavePath = @"C:\Users\USER\Pictures\230330_Beta";
                            cls_Param.ImageSavePath = l_CIni.Read_Ini("System", "ImageSavePath", Application.StartupPath + "\\Config.ini", @"C:\test0\image");
                            //cls_Param.imagePreoidDay = 1;

                            //cls_Param.ImageSaveType = "JPG";
                            cls_Param.ImageSaveType = l_CIni.Read_Ini("System", "imageSaveType", Application.StartupPath + "\\Config.ini", "xxx");

                            cls_Param.imagePreoidDay = Convert.ToInt32(l_CIni.Read_Ini("System", "imagePreoidDay", Application.StartupPath + "\\Config.ini", "1"));

                            cls_Param.ImageSaveModePath = @"C:\Users\USER\Pictures\230330_Beta";
                        }
                        catch (System.Exception exc)
                        {
                            l_oCLoadingProvider.UpdateProgress("Config-File Load Fail", l_ntCount);
                        }
                        break;

                    case 1:
                        break;

                    case 2:
                        try
                        {
                            l_oCLoadingProvider.UpdateProgress("Cathode Camera Connecting...", l_ntCount);

                            m_CCamera = new STCSBS500POE();

                            ///금호는 카메라 하나이므로 두개필요없음
                            ///
                            if (m_CCamera.Initialize() == true)
                            {
                                //if (m_CCamera.CheckDevice(cls_Info.m_untCam1Serial, out mCathodeCameraNum) == true)
                                if (m_CCamera.CheckDevice(cls_Param.m_untCamSerial, out mCathodeCameraNum) == true)
                                {
                                    m_blCam = true;
                                    f2.lb_Camera.Icon = Properties.Resources.OnImage1;
                                    f2.lb_Camera.Text = cls_Param.m_untCamSerial;
                                    l_oCLoadingProvider.UpdateProgress("Camera 1 Connect", l_ntCount);
                                    m_CLog.Debug("Camera #1 Conneted OK");
                                }
                                else
                                {
                                    m_blCam = false;
                                    f2.lb_Camera.Icon = Properties.Resources.OffImage1;
                                    l_oCLoadingProvider.UpdateProgress("Camera 1 Fail", l_ntCount);
                                    m_CLog.Error("Camera #1 Conneted Fail");
                                }

                            }
                        }
                        catch (System.Exception ex)
                        {
                            f2.lb_Camera.Image = Properties.Resources.OffImage1;

                            l_oCLoadingProvider.UpdateProgress("Camera 1 Connect Fail", l_ntCount);
                            //금호는 필요없음 230316 1755
                            //l_oCLoadingProvider.UpdateProgress("Camera 2 Connect Fail", l_ntCount);
                            m_CLog.Error("Camera #1, #2 Connected Fail");
                        }
                        break;

                    case 3:
                        break;

                    //case 4:
                    //    try
                    //    {
                    //        l_oCLoadingProvider.UpdateProgress("I/O Connecting....", l_ntCount);

                    //        if (AdvanDevice.Init(cls_Info.IO_Num) == true)
                    //        {
                    //            m_blIOFlag = true;
                    //            tSL_IO.Image = Properties.Resources.OnImage;
                    //            l_oCLoadingProvider.UpdateProgress("I/O Connect OK....", l_ntCount);
                    //        }
                    //        else
                    //        {
                    //            m_blIOFlag = false;
                    //            tSL_IO.Image = Properties.Resources.OffImage;
                    //            l_oCLoadingProvider.UpdateProgress("I/O Connect Fail....", l_ntCount);
                    //        }
                    //    }
                    //    break;
                default:
                    break;
                }
                l_ntCount++;
            }
            l_oCLoadingProvider.Close();
            l_oCLoadingProvider.Join();
        }

        private void Thread_ImageSave()
        {
            cls_ImageSave l_CImageSave;
            m_QueueImageSave.TryDequeue(out l_CImageSave);
            while (m_blThread_ImageSave == true)
            {
                try
                {
                    if (m_QueueImageSave.Count > 0)
                    {
                        int l_ntImageSave = m_QueueImageSave.Count;
                        //cls_ImageSave l_CImageSave;

                        for (int i = 0; i < l_ntImageSave; i++)
                        {

                            if (m_QueueImageSave.TryDequeue(out l_CImageSave) == true)
                            {
                                DirectoryInfo l_DI = new DirectoryInfo(cls_Param.ImageSavePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\");DirectoryInfo l_DI_NG = new DirectoryInfo(cls_Param.ImageSavePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" /*+ l_CImageSave.m_strModelName*/ + "\\NG" + "\\Right");
                                DirectoryInfo l_DI_OK = new DirectoryInfo(cls_Param.ImageSavePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" /*+ l_CImageSave.m_strModelName*/ + "\\NG" + "\\Right");
                                DirectoryInfo l_DI_AQ = new DirectoryInfo(cls_Param.ImageSavePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" /*+ l_CImageSave.m_strModelName*/ + "Aquisition");

                                if (l_DI.Exists == false)
                                    l_DI.Create();

                                if (l_DI_NG.Exists == false)
                                    l_DI_NG.Create();

                                if (l_DI_OK.Exists == false)
                                    l_DI_OK.Create();

                                if (l_DI_AQ.Exists == false)
                                    l_DI_AQ.Create();

                                //2022_04_01 이미지 저장 숫자 변경
                                if (cls_Info.imageSaveType == "BMP")
                                {

                                    if (m_AQmode)
                                        l_CImageSave.m_BmpArrayImage[0].Save(l_DI_AQ.FullName + "\\" + "_" + l_CImageSave.m_strDateTime + "_" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                                    else
                                    {
                                        if (l_CImageSave.m_blInspectionResult == true)
                                            l_CImageSave.m_BmpArrayImage[0].Save(l_DI_OK.FullName + "\\" + /*m_CModel.m_strModelName +*/ "_" + l_CImageSave.m_strDateTime + "_" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                                        else
                                            l_CImageSave.m_BmpArrayImage[0].Save(l_DI_NG.FullName + "\\" + "_" + l_CImageSave.m_strDateTime + "_" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                                    }
                                }
                                else
                                {

                                    if (m_AQmode)
                                        for (int j = 0; j < 2; j++)
                                        {
                                            l_CImageSave.m_BmpArrayImage[0].Save(l_DI_AQ.FullName + "\\" + "_" + l_CImageSave.m_strDateTime + "_" + j.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        }
                                    else
                                    {
                                        if (l_CImageSave.m_blInspectionResult == true)
                                            for (int j = 0; j < 2; j++)
                                            {
                                                //l_CImageSave.m_BmpArrayImage[j].Save(l_DI_OK.FullName + "\\" + l_CImageSave.m_strDateTime + "_" + j.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Jpeg);
                                                if (j == 0)
                                                {
                                                    l_CImageSave.m_BmpArrayImage[0].Save(l_DI_OK.FullName + "\\" + "_" + l_CImageSave.m_strDateTime + "_" + j.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                                                }
                                            }
                                        else
                                            for (int j = 0; j < 2; j++)
                                            {
                                                //l_CImageSave.m_BmpArrayImage[j].Save(l_DI_NG.FullName + "\\" + l_CImageSave.m_strDateTime + "_" + j.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Jpeg);
                                                if (j == 0)
                                                {
                                                    l_CImageSave.m_BmpArrayImage[0].Save(l_DI_NG.FullName + "\\" + "_" + l_CImageSave.m_strDateTime + "_" + j.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                                }
                                            }
                                    }


                                }

                            }
                        }
                    }
                }
                catch (System.Exception exc)
                {

                }

                Thread.Sleep(50);
            }
        }

        private void Thread_System()
        {
            while (m_blThread_System == true)
            {
                try
                {
                    DriveInfo[] l_DIArray = DriveInfo.GetDrives();

                    foreach (DriveInfo l_DI in l_DIArray)
                    {
                        if (l_DI.DriveType == DriveType.Fixed)
                        {
                            if (l_DI.Name.Contains("C") == true)
                            {
                                int l_ntTotalSize = Convert.ToInt32(l_DI.TotalSize / 1024 / 1024 / 1024);
                                int l_ntAvailableFreeSpace = Convert.ToInt32(l_DI.AvailableFreeSpace / 1024 / 1024 / 1024);
                                int l_ntUsage = l_ntTotalSize - l_ntAvailableFreeSpace;

                                this.Invoke(new EventHandler(delegate
                                {
                                    //복잡한 시스템 제거 0316 1828
                                    //tPB_CDrive.Maximum = l_ntTotalSize;
                                    //tSL_CDrive.Text = "C Drive ( " + l_ntUsage.ToString() + "GB / " + l_ntTotalSize.ToString() + "GB )";
                                    //tPB_CDrive.Value = l_ntUsage;
                                }));
                            }
                            else if (l_DI.Name.Contains("D") == true)
                            {
                                int l_ntTotalSize = Convert.ToInt32(l_DI.TotalSize / 1024 / 1024 / 1024);
                                int l_ntAvailableFreeSpace = Convert.ToInt32(l_DI.AvailableFreeSpace / 1024 / 1024 / 1024);
                                int l_ntUsage = l_ntTotalSize - l_ntAvailableFreeSpace;

                                this.Invoke(new EventHandler(delegate
                                {
                                    //복잡한 시스템 제거 0316 1828
                                    //tPB_DDrive.Maximum = l_ntTotalSize;
                                    //tSL_DDrive.Text = "D Drive ( " + l_ntUsage.ToString() + "GB / " + l_ntTotalSize.ToString() + "GB )";
                                    //tPB_DDrive.Value = l_ntUsage;
                                }));
                            }
                        }
                    }
                    ///복잡한 시스템 제거 0316 1828
                    ///if (m_QueueImageSave.Count > tPB_Image.Maximum)
                    ///    tPB_Image.Maximum = m_QueueImageSave.Count + 100;
                    ///
                    ///if (m_QueueDisplay.Count > tPB_Display.Maximum)
                    ///    tPB_Display.Maximum = m_QueueDisplay.Count + 100;
                    ///
                    ///if (m_QueueLog.Count > tPB_Log.Maximum)
                    ///    tPB_Log.Maximum = m_QueueLog.Count + 100;

                    this.Invoke(new EventHandler(delegate
                    {
                        ///복잡한 시스템 제거 0316 1828
                        ///tSL_Image.Text = "Image ( " + m_QueueImageSave.Count.ToString() + " / " + tPB_Image.Maximum.ToString() + " )";
                        ///tPB_Image.Value = m_QueueImageSave.Count;
                        ///
                        ///tSL_Display.Text = "Display ( " + m_QueueDisplay.Count.ToString() + " / " + tPB_Display.Maximum.ToString() + " )";
                        ///tPB_Display.Value = m_QueueDisplay.Count;
                        ///
                        ///tSL_Log.Text = "Log ( " + m_QueueLog.Count.ToString() + " / " + tPB_Log.Maximum.ToString() + " )";
                        ///tPB_Log.Value = m_QueueLog.Count;
                        ///
                        ///tSL_DateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    }));
                }
                catch
                {

                }

                Thread.Sleep(100);
            }
        }

        private void Thread_IO()
        {
            while (m_blThread_IO == true)
            {
                bool[] btemps_Input_0 = new bool[8];
                bool[] btemps_Input_1 = new bool[8];
                bool[] btemps_Output_0 = new bool[8];
                bool[] btemps_Output_1 = new bool[8];

                AdvanDevice.GET32_IO(out btemps_Input_0, out btemps_Input_1, out btemps_Output_0, out btemps_Output_1);

                // example
                if (btemps_Input_0[0] == true)
                {
                    // Trigger
                    //230330 1129 트리거 동작은 소프트웨어 트리거이므로, 주석처리함
                    //m_blTrigger = true;
                }

                Thread.Sleep(1);
            }
        }

        private void Thread_Main()
        {
            int l_ntGCCount = 0;
            while (m_blThread_Main == true)
            {
                if (m_blThread_Main == true)
                {
                    if (m_blTrigger == true)
                    {
                        l_ntGCCount++;
                    }
                }
            }
        }

        private void btn_Auto_Click(object sender, EventArgs e)
        {
            if (m_blIOFlag == false && m_blCam == false && m_blWorkSpaceFlag)
                return;
            if (btn_Auto.Text == "Manual")
            {
                btn_Auto.Text = "Auto";
                m_blAuto = false;
                //검사모드에서는 취득모드가 비활성화됨
                m_AQmode = false;
                btn_Auto.Image = Properties.Resources.play;

                btn_Setting.Enabled = true;
                btn_Log.Enabled = true;
                btn_Camera.Enabled = true;

                AdvanDevice.OutputStart(1, true, 0, 200, 0);
                m_CLog.Debug("Auto Play");
            }
            else
            {
                btn_Auto.Text = "Manual";
                m_blAuto = true;
                m_AQmode = false;

                m_blTrigger = true;
                btn_Auto.Image = Properties.Resources.Manual;

                btn_Setting.Enabled = false;
                btn_Log.Enabled = false;
                btn_Camera.Enabled = false;

                AdvanDevice.OutputStart(0, true, 0, 200, 0);
                m_CLog.Debug("Manual Play");
            }
        }

        private void btn_AQ_ON_Click(object sender, EventArgs e)
        {
            Main_frm main_Frm = new Main_frm();
            if (m_blIOFlag == false && m_blCam == false && m_blWorkSpaceFlag)
                return;

            if(btn_AQ_ON.Text == "A/Q OFF")
            {
                btn_AQ_ON.Text = "A/Q ON";
                m_blAuto = false;
                m_AQmode = false;
                btn_AQ_ON.Image = Properties.Resources.AQ_ON_2;

                btn_Setting.Enabled = true;
                btn_Auto.Enabled = true;
                btn_Log.Enabled = true;
                btn_Camera.Enabled = true;

                m_CLog.Debug("Aquisition OFF");
                main_Frm.AQ_Mode(m_AQmode = false);
            }
            else
            {
                btn_AQ_ON.Text = "A/Q OFF";
                m_blAuto = false;
                m_blTrigger = false;
                m_AQmode = false;
                btn_AQ_ON.Image = Properties.Resources.Manual;
                btn_Auto.Enabled = false;
                btn_Setting.Enabled = false;
                btn_Log.Enabled = false;
                btn_Camera.Enabled = false;
                m_CLog.Debug("Aquisition ON");
                main_Frm.AQ_Mode(m_AQmode = true);
            }
        }

        #region 메뉴 패널 버튼
        //private void mainToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    // Main_frm이 이미 dockPanel1 안에 있는지 확인
        //    bool isPanelExists = false;
        //    foreach (DockContent dockContent in dockPanel1.Contents)
        //    {
        //        if (dockContent is Main_frm)
        //        {
        //            isPanelExists = true;
        //            break;
        //        }
        //    }

        //    // Log_frm이 이미 dockPanel1 안에 존재하지 않으면 다시 보이게 함
        //    if (!isPanelExists)
        //    {
        //        Main_frm f1 = new Main_frm();
        //        f1.Show(dockPanel1, DockState.Document);
        //    }
        //}

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Setting_frm이 이미 dockPanel1 안에 있는지 확인
            bool isPanelExists = false;
            foreach (DockContent dockContent in dockPanel1.Contents)
            {
                if (dockContent is Setting_frm_2)
                {
                    isPanelExists = true;
                    break;
                }
            }

            // Setting_frm이 이미 dockPanel1 안에 존재하지 않으면 다시 보이게 함
            if (!isPanelExists)
            {
                Setting_frm_2 f2 = new Setting_frm_2();
                f2.Show(dockPanel1, DockState.DockRightAutoHide);
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// Log_frm이 이미 dockPanel1 안에 있는지 확인
            //bool isPanelExists = false;
            //foreach (DockContent dockContent in dockPanel1.Contents)
            //{
            //    if (dockContent is Log_frm)
            //    {
            //        isPanelExists = true;
            //        break;
            //    }
            //}

            //// Log_frm이 이미 dockPanel1 안에 존재하지 않으면 다시 보이게 함
            //if (!isPanelExists)
            //{
            //    Log_frm f3 = new Log_frm();
            //    f3.Show(dockPanel1, DockState.DockBottomAutoHide);
            //}
            f3.Show(dockPanel1, DockState.DockBottomAutoHide);
        }
        #endregion

        private void btn_Camera_Click(object sender, EventArgs e)
        {
            if(m_CCamera ==  null)
            {
                CustomMessageBox.Show("확인", "카메라 연결을 확인하세요", true);
                return;
            }

            Camera_frm camera = new Camera_frm(m_CCamera);
            camera.ShowDialog();
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            //// Setting_frm이 이미 dockPanel1 안에 있는지 확인
            //bool isPanelExists = false;
            //foreach (DockContent dockContent in dockPanel1.Contents)
            //{
            //    if (dockContent is Setting_frm_2)
            //    {
            //        isPanelExists = true;
            //        break;
            //    }
            //}

            //// Setting_frm이 이미 dockPanel1 안에 존재하지 않으면 다시 보이게 함
            //if (!isPanelExists)
            //{
            //    Setting_frm_2 f2 = new Setting_frm_2();
            //    f2.Show(dockPanel1, DockState.DockRightAutoHide);
            ////}
            //f2.Show(dockPanel1, DockState.DockRightAutoHide);
        }

        private void btn_Log_Click(object sender, EventArgs e)
        {
            //// Log_frm이 이미 dockPanel1 안에 있는지 확인
            //bool isPanelExists = false;
            //foreach (DockContent dockContent in dockPanel1.Contents)
            //{
            //    if (dockContent is Log_frm)
            //    {
            //        isPanelExists = true;
            //        break;
            //    }
            //}

            //// Log_frm이 이미 dockPanel1 안에 존재하지 않으면 다시 보이게 함
            //if (!isPanelExists)
            //{
            //    Log_frm f3 = new Log_frm();
            //    f3.Show(dockPanel1, DockState.DockBottomAutoHide);
            ////}
            //f3.Show(dockPanel1, DockState.DockBottomAutoHide);
        }
    }
}
