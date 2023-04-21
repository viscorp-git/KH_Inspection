using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Cognex.VisionPro;

namespace KH_Inspection
{
    public partial class Main_frm : DockContent
    {
        //STCSBS500POE m_CCamera = new STCSBS500POE();

        Base_frm base_Form = new Base_frm();

        public Main_frm()
        {
            InitializeComponent();
        }

        public void AQ_Mode(bool m_AQmode)
        {
            if (m_AQmode == true)
            {
                base_Form.m_CCamera.SetAcquisitionMode(0, 0);
                Bitmap[] l_BmpImage = new Bitmap[0];

                //Bitmap l_BmpImage = new Bitmap(cls_Param.ImageSavePath);
                //Bitmap l_BmpImage = new Bitmap(cls_Param.ImageSaveModePath);

                l_BmpImage[0] = base_Form.m_CCamera.Grab(0, 5000);

                cogRecordDisplay1.Image = new CogImage24PlanarColor(l_BmpImage[0]);
                string l_strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                cls_ImageSave l_CImageSave = new cls_ImageSave();
                l_CImageSave.m_BmpArrayImage = l_BmpImage;
                //l_CImageSave.m_BmpArrayImage = l_BmpImage;
                l_CImageSave.m_strDateTime = l_strDateTime;
            }
        }
        public class cls_ImageSave
        {
            //public Bitmap[] m_BmpArrayImage = new Bitmap[2];
            /// <summary>
            /// 230330 1639 해당 경로에 이미지를 참조하여 할당하는 코드
            /// 현재는 공간을 만들어야함
            /// </summary>
            //public Bitmap m_BmpArrayImage = new Bitmap(cls_Param.ImageSavePath);
            public Bitmap[] m_BmpArrayImage = new Bitmap[1];

            public string m_strDateTime;
            /// <summary>
            ///금호석유화학 모델 필요없음
            ///230330 1136
            /// </summary>
            //public string m_strModelName;
            public bool m_blInspectionResult = false;

            public void Dispose()
            {
                m_strDateTime = "";
            }
        }
    }
}
