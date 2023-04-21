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
using Sentech.StApiDotNET;
using Sentech.GenApiDotNET;
using System.Drawing.Design;

namespace KH_Inspection
{
    public partial class Setting_frm_2 : DockContent
    {
        Setting KH = new Setting();
        public Setting_frm_2()
        {
            InitializeComponent();
        }

        private void Setting_frm_2_Load(object sender, EventArgs e)
        {

            KH.Cam_No = cls_Param.m_untCamSerial;
            KH.IO_Name = cls_Param.IO_Name;
            KH.Save_Path = cls_Param.ImageSavePath;
            KH.Save_Type = cls_Param.ImageSaveType;
            KH.Save_Period = cls_Param.imagePreoidDay;
            propertyGrid1.SelectedObject = KH;
        }

        public class Setting
        {
            private string cam_no;
            private string IO_name;
            private string Save_path;
            private string Save_type;
            private int Save_period;

            public Setting() { }

            [CategoryAttribute("Camera Setting"), DescriptionAttribute("카메라 시리얼 넘버")]
            public string Cam_No
            {
                get { return cam_no; }
                set { cam_no = value; }
            }

            [CategoryAttribute("I/O Setting"), DescriptionAttribute("I/O 보드 이름")]
            public string IO_Name
            {
                get { return IO_name; }
                set { IO_name = value; }
            }

            [CategoryAttribute("Save Image Setting"), DescriptionAttribute("이미지 저장 경로 설정")]
            [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
            public string Save_Path
            {
                get { return Save_path; }
                set { Save_path = value; }
            }

            [CategoryAttribute("Save Image Setting"), DescriptionAttribute("저장될 이미지 포맷을 설정")]
            public string Save_Type
            {
                get { return Save_type; }
                set { Save_type = value; }
            }

            [CategoryAttribute("Save Image Setting"), DescriptionAttribute("이미지를 저장할 주기 (일 단위)")]
            public int Save_Period
            {
                get { return Save_period; }
                set { Save_period = value; }
            }
        }

        private void btn_set_save_Click(object sender, EventArgs e)
        {
            cls_Param.m_untCamSerial = KH.Cam_No;
            cls_Param.IO_Name = KH.IO_Name;
            cls_Param.ImageSavePath = KH.Save_Path;
            cls_Param.ImageSaveType = KH.Save_Type;
            cls_Param.imagePreoidDay = KH.Save_Period;

            cls_Ini l_CIni = new cls_Ini();

            l_CIni.Write_Ini("Cam", "m_untCam1Serial", cls_Param.m_untCamSerial, Application.StartupPath + "\\Config.ini");
            l_CIni.Write_Ini("System", "IO_Name", cls_Param.IO_Name, Application.StartupPath + "\\Config.ini");
            l_CIni.Write_Ini("System", "ImageSavePath", cls_Param.ImageSavePath, Application.StartupPath + "\\Config.ini");
            l_CIni.Write_Ini("System", "imageSaveType", cls_Param.ImageSaveType, Application.StartupPath + "\\Config.ini");
            l_CIni.Write_Ini("System", "imagePreoidDay", cls_Param.imagePreoidDay, Application.StartupPath + "\\Config.ini");
        }

        public class FolderNameEditor : UITypeEditor
        {
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                using (var folderBrowser = new FolderBrowserDialog())
                {
                    folderBrowser.Description = "Select folder";
                    folderBrowser.SelectedPath = (string)value;

                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        return folderBrowser.SelectedPath;
                    }
                }

                return value;
            }

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }
    }
}