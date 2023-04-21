using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KH_Inspection
{
    public class cls_Info
    {
        public static string m_untCam1Serial = "";
        //금호는 필요없음230316 2007
        //public static string m_untCam2Serial = "";

        public static int m_ntSurfFinder_Idx = 0;
        public static string m_strSurfFinder_IP = "";

        public static string IO_Name = "";
        public static int IO_Num = 0;

        public static string ImageSavePath = "";
        public static string ImageSaveModePath = "";
        public static string imageSaveType = "";
        public static int imagePreoidDay = 0;

        public static string m_strPLC_IP = "";
        public static int m_ntPLC_Port = 0;

        public static string m_strLastModelName = "";
        public static int m_ntLastModelCount = 0;

        public static string s_lightControllerPort = "";
        public static int s_lightControllerBaudRate = 0;

    }
}