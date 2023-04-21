using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace MegaInformationTechnology
{
    public class LightController
    {
        private SerialPort _mainSerialPort;
        private string _portName;
        private int _baudRate;
        private string _responseData;
        private string _readBuffer;

        //private char STX = (char)0x02;
        //private char ETX = (char)0x03;
        //MEGA조명에서 사용
        private char ACK = (char)0x06;
        private char NAK = (char)0x15;
        //LVS조명에서 사용 
        private char CR = (char)0x0D;
        private char LF = (char)0x0A;
        public LightController(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;

            if (portName != "")
                _mainSerialPort = new SerialPort(_portName, _baudRate);
        }

        public bool Open()
        {
            if (_mainSerialPort == null)
                return false;

            _mainSerialPort.Open();

            if (_mainSerialPort.IsOpen == true)
            {
                _mainSerialPort.DataReceived += MainDataReceived;
                return true;
            }
            else
                return false;
        }

        public void Close()
        {
            if (_mainSerialPort == null)
                return;

            _mainSerialPort.DataReceived -= MainDataReceived;
            _mainSerialPort.Close();
        }

        public bool SetOn(string channel, string light)
        {
            if (_mainSerialPort == null || _mainSerialPort.IsOpen == false)
                return false;

            //string sendData = STX + channel.ToString() + channel.ToString() + "ON" + ETX;
            string sendData = "L" + channel + light + CR + LF;

            _responseData = "";

            _mainSerialPort.Write(sendData);

            Thread.Sleep(50);

            if (_responseData == "" || _responseData[0] == NAK)
                return false;
            else
                return true;
        }
        /// <summary>
        /// LVS 조명
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool SetOff(string channel)
        {
            if (_mainSerialPort == null || _mainSerialPort.IsOpen == false)
                return false;

            //string sendData = STX + channel.ToString() + "OFF" + ETX;
            string sendData = "E" + channel + CR + LF;

            _responseData = "";

            _mainSerialPort.Write(sendData);

            Thread.Sleep(50);

            if (_responseData == "" || _responseData[0] == NAK)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 메가 조명
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public bool SetBrightness(string channel, int brightness)
        //{
        //    if (_mainSerialPort == null || _mainSerialPort.IsOpen == false)
        //        return false;

        //    string sendData = STX + channel.ToString() + brightness.ToString("000") + ETX;

        //    _responseData = "";

        //    _mainSerialPort.Write(sendData);

        //    Thread.Sleep(50);

        //    if (_responseData == "" || _responseData[0] == NAK)
        //        return false;
        //    else
        //        return true;
        //}

        private void MainDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string readBuffer = _readBuffer + _mainSerialPort.ReadExisting();
            _readBuffer = string.Empty;
            char[] charBuffer = new char[readBuffer.Length];
            int bufferLength = 0;

            for (int i = 0; i < readBuffer.Length; i++)
            {
                if (readBuffer[i] == LF)
                {
                    string dataForConvert = new string(charBuffer).Trim('\0');
                    _responseData = dataForConvert.Trim(CR);
                    _readBuffer = "";
                    charBuffer = new char[readBuffer.Length];
                }
                else
                {
                    charBuffer[bufferLength] = readBuffer[i];
                    bufferLength++;
                }
            }

            _readBuffer = new string(charBuffer).Trim('\0');
        }

    }
}
