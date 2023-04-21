using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace KH_Inspection
{
    public class cls_Ini
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, decimal val, string filepath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string Read_Ini(string pstr_Section, string pstr_Key, string pstr_Filepath, string pstr_Default)
        {
            try
            {
                StringBuilder l_SB = new StringBuilder(1000);
                int Temp = GetPrivateProfileString(pstr_Section, pstr_Key, null, l_SB, 5000, pstr_Filepath);

                if (Temp == 0)
                    return pstr_Default;
                else
                    return l_SB.ToString();
            }
            catch
            {
                return pstr_Default;
            }
        }
        ///INI int 전용 메서드 생성
        ///2023 0318 1312
        ///Int Type 필요없으므로 주석처리
        ///2023 0322 1525
        public int Read_Ini(string pstr_Section, string pstr_Key, string pstr_Filepath, int pstr_Default)
        {
            try
            {
                StringBuilder l_SB = new StringBuilder(1000);
                int Temp = GetPrivateProfileString(pstr_Section, pstr_Key, null, l_SB, 5000, pstr_Filepath);

                //for (int i = 0; i < Temp; i++)
                //{
                //    string[] Value = new string[Temp];
                //    Value[i] = l_SB.ToString();
                //}
                //
                string Val = "";
                if (Temp == 0)
                    return pstr_Default;
                else
                {
                    for (int i = 0; i < l_SB.Length; i++)
                    {
                        Val += l_SB[i];
                    }
                    return Convert.ToInt32(Val);
                }


                //string val = "";
                //val = l_SB.ToString();

                //return Convert.ToInt32(l_SB);
            }
            catch (Exception e)
            {
                return pstr_Default;
            }
        }

        public void Write_Ini(string pstr_Section, string pstr_Key, string pstr_Value, string pstr_Filepath)
        {
            WritePrivateProfileString(pstr_Section, pstr_Key, pstr_Value, pstr_Filepath);
        }
        public void Write_Ini(string pstr_Section, string pstr_Key, decimal pstr_Value, string pstr_Filepath)
        {
            WritePrivateProfileString(pstr_Section, pstr_Key, pstr_Value, pstr_Filepath);
        }
        public void Write_Ini(string pstr_Section, string pstr_Key, int pstr_Value, string pstr_Filepath)
        {
            string Cvt_Val = Convert.ToString(pstr_Value);
            WritePrivateProfileString(pstr_Section, pstr_Key, Cvt_Val, pstr_Filepath);
        }

        ///int Type는 쓰기 불가능
        ///2023 0322 1446
        //public void Write_INT(string pstr_Section, string pstr_Key, int pstr_Value, string pstr_Filepath)
        //{
        //    WritePrivateProfileString(pstr_Section, pstr_Key, pstr_Value, pstr_Filepath);
        //}

    }
}
