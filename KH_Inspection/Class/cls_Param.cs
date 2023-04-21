using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KH_Inspection
{
    class cls_Param
    {
        public cls_Ini paramUp = new cls_Ini();
        public cls_Setup setup = new cls_Setup();
        public static string m_untCamSerial { set; get; }
        public static string IO_Name { set; get; }
        public static string IO_Num { set; get; }
        public static string ImageSavePath { set; get; }
        public static string ImageSaveModePath { set; get; }
        public static string ImageSaveType { set; get; }
        //public static string imagePreoidDay { set; get; }
        public static int imagePreoidDay { set; get; }
        public static string m_strPLC_IP { set; get; }
        public static string m_ntPLC_Port { set; get; }
        public static string m_strLastModelName { set; get; }
        public static string m_ntLastModelCount { set; get; }
        public static string s_lightControllerPort { set; get; }
        public static int s_lightControllerBaudRate { set; get; }

        public static string str_lightControllerBaudRate
        {
            get { return s_lightControllerBaudRate.ToString(); }
        }


        //public static string s_lightControllerBaudRate { set; get; }

        //public string testing0 = "";
        /// <summary>
        /// setget 프로퍼티에 변수 다수 선언
        /// 2303120 1038 set 함수 내에 다수 선언은 불가능
        /// </summary>
        ///public string testing
        ///{
        ///    set
        ///    {
        ///        testing = value;
        ///        testing0 = value;
        ///    }
        ///    get
        ///    {
        ///        return testing;
        ///        return testing0;
        ///    }
        ///}


        //public bool setup_Param()
        //{
        //    paramUp.Read_Ini()
        //}

        public string[] camParam()
        {
            ///string[] param =
            ///{
            ///    "23AC927",
            ///    "",
            ///    "",
            ///    "",
            ///    "",
            ///    "",
            ///    "",
            ///    "",
            ///    "",
            ///
            ///};
            ///
            
            string[] prm = new string[setup.m_config.Length];
            prm = setup.m_config;
            return prm;
        }

        ///2303201040불필요
        ///public string[] index =
        ///{
        ///};
        ///public string[] camind()
        ///{
        ///    index = setup.m_config;
        ///    return index;
        ///}


        /// <summary>
        /// setget 프로퍼티로 여러개 저장하기
        /// 2023 0317 1317
        /// </summary>
        public static string[] testing1
        {
            
            set
            {
                int num = 0;
                for (int i = 0; i < num; i++)
                {
                    string[] te1 = value;
                }

            }
            get
            {
                return testing1;
            }
        }
        public static string[] System_config { set; get; }


    }
}
