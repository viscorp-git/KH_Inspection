using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using IDMAX_FrameWork;
using Sentech.StApiDotNET;

namespace KH_Inspection
{
    public partial class Camera_frm : Form
    {
        STCSBS500POE m_CCamera = new STCSBS500POE();

        bool m_blThreadLive;
        bool m_blStream;
        bool m_blDevice;
        bool m_blCallback;

        string m_strWorkSpaceFilePath = "";

        int m_ntStream = -1;

        Bitmap[] I_RunBmpImage = new Bitmap[1];

        public Camera_frm(STCSBS500POE pC_Camera/*, cls_VPDL pC_VPDL*/)
        {
            InitializeComponent();
            m_CCamera = pC_Camera;
            //m_CVPDL = pC_VPDL;
        }

        private void Camera_frm_Load(object sender, EventArgs e)
        {
            cogDisplayToolbar_camera.Display = cogDisplay1;
        }

        private void btn_Live_Camera_Click(object sender, EventArgs e)
        {
            cogDisplay1.InteractiveGraphics.Clear();
            cogDisplay1.StaticGraphics.Clear();

            if (m_blThreadLive == false)
            {
                btn_Live_Camera.Text = "Live Stop";

                if (m_blCallback == false)
                {
                    m_blCallback = m_CCamera.RegisterCallback(0, OnCallback);
                }
                if (m_blStream == false)
                {
                    m_blStream = m_CCamera.StreamAcquisitionStart(0);
                }
                if (m_blDevice == false)
                {
                    m_blDevice = m_CCamera.DeviceAcquisitionStart(0);
                }

                m_blThreadLive = true;
            }
            else
            {
                btn_Live_Camera.Text = "LIVE";

                if(m_blDevice == true)
                {
                    m_CCamera.DeviceAcquisitionStop(0);
                    m_blDevice = false;
                }

                m_blThreadLive = false;
            }
        }

        private void btn_Grab_Camera_Click(object sender, EventArgs e)
        {
            if (m_blCallback == true)
            {
                m_CCamera.DeregisterCallback(1);
                m_blCallback = false;
            }
            if (m_blStream == true)
            {
                m_CCamera.StreamAcquisitionStop(1);
                m_blStream = false;
            }
            if (m_blThreadLive == true)
            {
                CustomMessageBox.Show("확인", "LIVE 상태를 확인하세요.", true);
                return;
            }

            Bitmap displayBitmap = m_CCamera.Grab(0, 5000);

            if (displayBitmap != null)
            {
                cogDisplay1.Image = new CogImage8Grey(displayBitmap);
            }

            GC.Collect();
        }

        public void OnCallback(IStCallbackParamBase paramBase, object[] param)
        {
            if(paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
            {
                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;

                if (callbackParam != null)
                {
                    try
                    {
                        IStDataStream dataStream = callbackParam.GetIStDataStream();

                        using(CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(50))
                        {
                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                            {
                                IStImage stImage = streamBuffer.GetIStImage();

                                this.Invoke(new EventHandler(delegate {cogDisplay1.Image = new CogImage8Grey(m_CCamera.ConvertStImageToBitmap(stImage)); }));

                                Byte[] imageData = stImage.GetByteArray();
                                Console.Write("BlockId = " + streamBuffer.GetIStStreamBufferInfo().FrameID);
                                Console.Write("Size : " + stImage.ImageWidth + "x" + stImage.ImageHeight);
                                Console.Write("First byte = " + imageData[0] + Environment.NewLine);
                            }
                            else
                            {
                                Console.WriteLine("Image data does not exist");
                            }
                        }
                    }
                    catch(System.Exception e)
                    {
                        Console.Error.WriteLine("An exection occurred. \r\n" + e.Message);
                    }
                }
            }
        }
    }
}
