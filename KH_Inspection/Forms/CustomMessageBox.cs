using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KH_Inspection
{
    public partial class CustomMessageBox : Form
    {
        private string _captionText = "";
        private string _contentsText = "";
        private ButtonsType _selectedButtonsType;
        private SelectButton _resultButton;
        private bool _isKorean = false;

        public enum ButtonsType
        {
            OK = 0,
            OKCancle = 1,
            YesNo = 2
        }

        public enum SelectButton
        {
            OK = 0,
            Cancle = 1,
            Yes = 2,
            No = 3
        }

        public CustomMessageBox(string captionText, string contentsText, ButtonsType buttonsType, bool isKorean)
        {
            InitializeComponent();
            _captionText = captionText;
            _contentsText = contentsText;
            _selectedButtonsType = buttonsType;
            _isKorean |= isKorean;
        }

        private void MessageBox_frm_Load(object sender, EventArgs e)
        {
            lbl_Caption.Text = _captionText;
            lbl_Contents.Text = _contentsText;

            if (_selectedButtonsType == ButtonsType.OK)
            {
                btn_Position1.Visible = false;
                btn_Position2.Visible = true;

                if (_isKorean == true)
                    btn_Position2.Text = "확인";
                else
                    btn_Position2.Text = "OK";
            }
            else if (_selectedButtonsType == ButtonsType.OKCancle)
            {
                btn_Position1.Visible = true;
                btn_Position2.Visible = true;

                if (_isKorean == true)
                {
                    btn_Position1.Text = "확인";
                    btn_Position2.Text = "취소";
                }
                else
                {
                    btn_Position1.Text = "OK";
                    btn_Position2.Text = "Cancle";
                }


            }
            else if (_selectedButtonsType == ButtonsType.YesNo)
            {
                btn_Position1.Visible = true;
                btn_Position2.Visible = true;

                if (_isKorean == true)
                {
                    btn_Position1.Text = "예";
                    btn_Position2.Text = "아니요";
                }
                else
                {
                    btn_Position1.Text = "Yes";
                    btn_Position2.Text = "No";
                }
            }
        }

        public static SelectButton Show(string captionText, string contentsText, ButtonsType buttonsType, bool isKorean = false)
        {
            CustomMessageBox customMessageBox = new CustomMessageBox(captionText, contentsText, buttonsType, isKorean);
            customMessageBox.ShowDialog();

            return customMessageBox._resultButton;
        }

        public static SelectButton Show(string captionText, string contentsText, bool isKorean = false)
        {
            CustomMessageBox customMessageBox = new CustomMessageBox(captionText, contentsText, ButtonsType.OK, isKorean);
            customMessageBox.ShowDialog();

            return customMessageBox._resultButton;
        }

        private void btn_Position1_MouseMove(object sender, MouseEventArgs e)
        {
            if (btn_Position1.BackColor != Color.Silver)
                btn_Position1.BackColor = Color.Silver;
        }

        private void btn_Position2_MouseMove(object sender, MouseEventArgs e)
        {
            if (btn_Position2.BackColor != Color.Silver)
                btn_Position2.BackColor = Color.Silver;
        }

        private void frm_MessageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (btn_Position1.BackColor != Color.Black)
                btn_Position1.BackColor = Color.Black;

            if (btn_Position2.BackColor != Color.Black)
                btn_Position2.BackColor = Color.Black;
        }

        private void btn_Position1_Click(object sender, EventArgs e)
        {
            if (_selectedButtonsType == ButtonsType.OKCancle)
            {
                _resultButton = SelectButton.OK;
                this.Close();
            }
            else if (_selectedButtonsType == ButtonsType.YesNo)
            {
                _resultButton = SelectButton.Yes;
                this.Close();
            }
        }

        private void btn_Position2_Click(object sender, EventArgs e)
        {
            if (_selectedButtonsType == ButtonsType.OK)
            {
                _resultButton = SelectButton.OK;
                this.Close();
            }
            else if (_selectedButtonsType == ButtonsType.OKCancle)
            {
                _resultButton = SelectButton.Cancle;
                this.Close();
            }
            else if (_selectedButtonsType == ButtonsType.YesNo)
            {
                _resultButton = SelectButton.No;
                this.Close();
            }
        }
    }
}
