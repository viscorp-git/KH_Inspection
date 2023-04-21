using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace KH_Inspection
{
    public class cls_Model
    {
        private cls_Ini m_CIni = new cls_Ini();

        public double m_dlExposure1 = 0;
        public double m_dlExposure2 = 0;

        public string m_strModelName = "";
        public int m_ntModelCount = 0;

        public string m_dlChannel1 = "255";
        public string m_dlChannel2 = "255";

        public string m_strWorkSpacePath = "";

        public int m_dlStream = 0;

        public cls_Model()
        {
            m_strModelName = "";
            m_ntModelCount = 0;
        }

        public cls_Model(string pstr_ModelName, int pnt_ModelCount)
        {
            m_strModelName = pstr_ModelName;
            m_ntModelCount = pnt_ModelCount;
        }

        public bool Model_Save()
        {
            try
            {
                m_CIni.Write_Ini("Camera", "m_dlExposure1", Convert.ToString(m_dlExposure1), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                m_CIni.Write_Ini("Camera", "m_dlExposure2", Convert.ToString(m_dlExposure2), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                m_CIni.Write_Ini("Light", "m_dlChannel1", Convert.ToString(m_dlChannel1), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                m_CIni.Write_Ini("Light", "m_dlChannel2", Convert.ToString(m_dlChannel2), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                m_CIni.Write_Ini("VPDL", "m_strWorkSpacePath", Convert.ToString(m_strWorkSpacePath), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                m_CIni.Write_Ini("VPDL", "m_dlStream", Convert.ToString(m_dlStream), Application.StartupPath + "\\Model\\" + m_ntModelCount.ToString("00") + "_" + m_strModelName + ".ini");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Model_Load(string pstr_ModelName, int pnt_ModelCount)
        {
            try
            {
                FileInfo l_FI = new FileInfo(Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini");

                if (l_FI.Exists == true)
                {
                    m_dlExposure1 = Convert.ToDouble(m_CIni.Read_Ini("Camera", "m_dlExposure1", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", ""));
                    m_dlExposure2 = Convert.ToDouble(m_CIni.Read_Ini("Camera", "m_dlExposure2", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", ""));
                    m_dlChannel1 = m_CIni.Read_Ini("Light", "m_dlChannel1", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", "");
                    m_dlChannel2 = m_CIni.Read_Ini("Light", "m_dlChannel2", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", "");
                    m_strWorkSpacePath = m_CIni.Read_Ini("VPDL", "m_strWorkSpacePath", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", "");
                    m_dlStream = Convert.ToInt32(m_CIni.Read_Ini("VPDL", "m_dlStream", Application.StartupPath + "\\Model\\" + pnt_ModelCount.ToString("00") + "_" + pstr_ModelName + ".ini", ""));
                    m_ntModelCount = pnt_ModelCount;
                    m_strModelName = pstr_ModelName;

                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
