using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace KH_Inspection
{
    [Serializable]
    class cls_File
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);

        public void Save_ResultCSV(int pnt_CameraNum ,int pnt_ErrorNumber)
        {
            string[] l_strErrorCode = new string[13];
            string l_strDateTime = DateTime.Now.ToString("yyyyMMdd") + ".Csv";
            string l_strpath = "";
            string l_strList = ", OK, Fixture, Chipoff, Dented, Fiber, ForeignMaterial, Particle, Scratch, Burr, Void, Bubble, Peeloff, Delamination";

            if (pnt_CameraNum == 0)
            {
                l_strpath = "D:\\Result\\TopResult\\" + l_strDateTime;
            }
            else
            {
                l_strpath = "D:\\Result\\BottomResult\\" + l_strDateTime;
            }

            for (int i = 0; i < 13; i++)
            {
                if (i == pnt_ErrorNumber)
                {
                    l_strErrorCode[i] = "1";
                }
                else
                {
                    l_strErrorCode[i] = "0";
                }
            }         

            FileInfo l_FileInfo = new FileInfo(l_strpath);

            if (l_FileInfo.Exists == false)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(l_strpath))
                {
                    file.WriteLine(l_strList);
                    file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", DateTime.Now.ToString("hh:mm:ss"),  l_strErrorCode[0], l_strErrorCode[1], l_strErrorCode[2], l_strErrorCode[3], l_strErrorCode[4], l_strErrorCode[5]
                        , l_strErrorCode[6], l_strErrorCode[7], l_strErrorCode[8], l_strErrorCode[9], l_strErrorCode[10], l_strErrorCode[11], l_strErrorCode[12]);
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(l_strpath, true))
                {
                    file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", DateTime.Now.ToString("hh:mm:ss"), l_strErrorCode[0], l_strErrorCode[1], l_strErrorCode[2], l_strErrorCode[3], l_strErrorCode[4], l_strErrorCode[5]
                       , l_strErrorCode[6], l_strErrorCode[7], l_strErrorCode[8], l_strErrorCode[9], l_strErrorCode[10], l_strErrorCode[11], l_strErrorCode[12]);
                }
            }
        }

        public void Save_Result(int pnt_ErrorNumber)
        {
            //OK Fixture Chipoff Dented Fiber ForeignMaterial Particle Scratch Burr
            string[] l_strErrorCode = new string[9];
            string l_strDateTime = DateTime.Now.ToString("yyyyMMdd") + ".xls";
            string l_strpath = Application.StartupPath + "\\Result\\" + l_strDateTime;
            string l_strList = "OK, Fixture, Chipoff, Dented, Fiber, ForeignMaterial, Particle, Scratch, Burr";

            DirectoryInfo l_DirectoryInfo = new DirectoryInfo(Application.StartupPath + "\\Result");

            if (l_DirectoryInfo.Exists == false)
            {
                l_DirectoryInfo.Create();
            }

            for (int i = 0; i < 9; i++)
            {
                l_strErrorCode[i] = "0,";

                if (i == pnt_ErrorNumber)
                {
                    l_strErrorCode[i] = "1,";
                }
            }

            string l_strResultList = string.Join(" ", l_strErrorCode);

            FileInfo l_FileInfo = new FileInfo(l_strpath);

            if (l_FileInfo.Exists == false)
            {
                //l_FileInfo.Create();
                File.AppendAllText(l_strpath, l_strList);
                File.AppendAllText(l_strpath, Environment.NewLine);
            }
        
            //string l_strpath = "D:\\SystemLog\\" + l_strDateTime;
            File.AppendAllText(l_strpath, l_strResultList);
            File.AppendAllText(l_strpath, Environment.NewLine);
        }

        public void Save_SystemLog(System.Windows.Forms.ListBox p_oListBox, string pstr_Log)
        {
            p_oListBox.Items.Add(pstr_Log);

            DirectoryInfo l_DirectoryInfo = new DirectoryInfo("D:\\SystemLog");

            if (l_DirectoryInfo.Exists == false)
            {
                l_DirectoryInfo.Create();
            }

            string l_strDateTime = DateTime.Now.ToString("yyyyMMdd") + ".Log";

            string l_strpath = "D:\\SystemLog\\" + l_strDateTime;

            File.AppendAllText(l_strpath, pstr_Log);
            File.AppendAllText(l_strpath, Environment.NewLine);

            if (p_oListBox.Items.Count > 100)
            {
                p_oListBox.Items.Clear();
            }
        }

        public void Save_ErrorLog(System.Windows.Forms.ListBox p_oListBox, string pstr_Log)
        {
            p_oListBox.Items.Add(pstr_Log);

            DirectoryInfo l_DirectoryInfo = new DirectoryInfo("D:\\ErrorLog");

            if (l_DirectoryInfo.Exists == false)
            {
                l_DirectoryInfo.Create();
            }

            string l_strDateTime = DateTime.Now.ToString("yyyyMMdd") + ".Log";

            string l_strpath = "D:\\ErrorLog\\" + l_strDateTime;

            File.AppendAllText(l_strpath, pstr_Log);
            File.AppendAllText(l_strpath, Environment.NewLine);

            if (p_oListBox.Items.Count > 100)
            {
                p_oListBox.Items.Clear();
            }
        }

        public void Create_Folder(string pstr_FolderName)
        {
            DirectoryInfo l_DirectoryInfo = new DirectoryInfo(pstr_FolderName);

            if (l_DirectoryInfo.Exists == false)
            {
            }
                l_DirectoryInfo.Create();
        }

        public bool FormScreen_Save(Form p_FormName, string pstr_ImageName, string pstr_Filepath)
        {
            try
            {
                DirectoryInfo l_DirectoryInfo = new DirectoryInfo(pstr_Filepath);

                Bitmap l_BmpTemp = new Bitmap(p_FormName.Width, p_FormName.Height);
                Graphics l_Graphics = Graphics.FromImage(l_BmpTemp);
                l_Graphics.CopyFromScreen(new System.Drawing.Point(p_FormName.Bounds.X, p_FormName.Bounds.Y), new System.Drawing.Point(0, 0), p_FormName.Size);

                if (l_DirectoryInfo.Exists == false)
                {
                    l_DirectoryInfo.Create();
                }

                l_BmpTemp.Save(pstr_Filepath + "\\" + pstr_ImageName + ".bmp");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Write_Ini(string pstr_Section, string pstr_Key, string pstr_Value, string pstr_FilePath)
        {
            WritePrivateProfileString(pstr_Section, pstr_Key, pstr_Value, pstr_FilePath);
        }

        public string Read_Ini(string pstr_Section, string pstr_Key, string pstr_FilePath,string pstr_Defult)
        {
            StringBuilder l_strBuilderValue = new StringBuilder();

            GetPrivateProfileString(pstr_Section, pstr_Key, pstr_Defult, l_strBuilderValue, 64, pstr_FilePath);

            return Convert.ToString(l_strBuilderValue);
        }

        public bool System_Logger(string str)
        {
            StreamWriter sw = null;
            try
            {
                DateTime datetime = DateTime.Now;
                FileInfo exefileinfo = new FileInfo(Application.ExecutablePath);

                string path = "d:\\System_Log";
                path = path + @"\" + datetime.ToString("yyyy_MM_dd");
                DirectoryInfo drInfo = new DirectoryInfo(path);
                if (!drInfo.Exists)
                {
                    drInfo = Directory.CreateDirectory(path);
                }
                string fullname = path + @"\" + datetime.ToString("yyyy_MM_dd") + ".txt";
                sw = new StreamWriter(fullname, true, Encoding.GetEncoding("ks_c_5601-1987"));
                sw.WriteLine(str);

            }
            catch
            {
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
            }
            return true;
        }

        public bool IO_HistoryLogger(string str)
        {
            StreamWriter sw = null;
            try
            {
                DateTime datetime = DateTime.Now;
                FileInfo exefileinfo = new FileInfo(Application.ExecutablePath);

                string path = "D:\\VISION_IO_LOG";
                path = path + @"\" + datetime.ToString("yyyy_MM_dd");
                DirectoryInfo drInfo = new DirectoryInfo(path);
                if (!drInfo.Exists)
                {
                    drInfo = Directory.CreateDirectory(path);
                }
                string fullname = path + @"\" + datetime.ToString("yyyy_MM_dd") + ".txt";                
                sw = new StreamWriter(fullname, true, Encoding.GetEncoding("ks_c_5601-1987"));
                sw.WriteLine(str);
                sw.Close();
                drInfo = null;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
            }
            return true;
        }

        public bool System_HistoryLogger(string str)
        {
            StreamWriter sw = null;
            try
            {
                DateTime datetime = DateTime.Now;
                FileInfo exefileinfo = new FileInfo(Application.ExecutablePath);

                string path = "D:\\VISION_System_LOG";
                path = path + @"\" + datetime.ToString("yyyy_MM_dd");
                DirectoryInfo drInfo = new DirectoryInfo(path);
                if (!drInfo.Exists)
                {
                    drInfo = Directory.CreateDirectory(path);
                }
                string fullname = path + @"\" + datetime.ToString("yyyy_MM_dd") + ".txt";
                sw = new StreamWriter(fullname, true, Encoding.GetEncoding("ks_c_5601-1987"));
                sw.WriteLine(str);
                drInfo = null;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
            }
            return true;
        }

        public String[] GetFromToDays(DateTime FromDate, DateTime EndDate)
        {
            String[] arrDays;
            String strDays = "";

            String strFrom = FromDate.ToString("yyyy_MM_dd 00:00:00");
            String strEnd = EndDate.ToString("yyyy_MM_dd 00:00:00");
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dtFrom = DateTime.ParseExact(strFrom, "yyyy_MM_dd HH:mm:ss", provider);
            DateTime dtTo = DateTime.ParseExact(strEnd, "yyyy_MM_dd HH:mm:ss", provider);


            while (dtFrom <= dtTo)
            {
                strDays += dtFrom.ToString("yyyy_MM_dd") + ",";
                dtFrom = dtFrom.AddDays(1);
            }

            arrDays = strDays.Substring(0, strDays.Length - 1).Split(',');

            return arrDays;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Data"></param>
        /// <param name="lv"></param>
        public void txtHistoryLoader(DateTime FromDate, DateTime EndDate, string Data, ListView lv)
        {
            try
            {
                string[] DateList = GetFromToDays(FromDate, EndDate);

                if (DateList.Length > 0)
                {
                    for (int i = 0; i < DateList.Length; i++)
                    {
                        string logPath = "D:\\VISION_LOG" + @"\" + DateList[i] + @"\" + DateList[i] + ".txt";  //프로그램 실행되고 있는데 path 가져오기
                        FileInfo fi = new FileInfo(logPath);
                        if (fi.Exists)
                        {
                            string[] txtValue = System.IO.File.ReadAllLines(logPath, Encoding.GetEncoding("ks_c_5601-1987"));
                            if (txtValue.Length > 0)
                            {
                                for (int j = 0; j < txtValue.Length; j++)
                                {
                                    if (txtValue[j] != "")
                                    {
                                        string[] temp = txtValue[j].Split(',');
                                        if (temp[1].Contains(Data) || Data == "")
                                        {
                                            ListViewItem item = new ListViewItem(temp[0]);
                                            for (int z = 1; z < temp.Length; z++)
                                            {

                                                item.SubItems.Add(temp[z]);
                                            }
                                            lv.Items.Add(item);
                                        }


                                    }

                                }
                            }
                        }
                    }

                }


            }
            catch
            {
                MessageBox.Show("History Load Fail");
            }

        }

    }
}
