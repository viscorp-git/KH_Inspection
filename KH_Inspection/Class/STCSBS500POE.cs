using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KH_Inspection

{
    public class STCSBS500POE
    {
        private CStApiAutoInit _cStApiAutoInit;
        private CStSystem _cStSystem;
        private CStDeviceArray _cStDeviceArray;
        private CStDevice _cSTDevice;
        private CStDataStreamArray _cStDataStreamArray;
        private CStDataStream _cStDataStream;
        private IntPtr[] _cStStreamArrayCallbackHandler;

        // Feature names
        const string ACQUISITION_MODE = "AcquisitionMode";              //Standard
        const string TRIGGER_SELECTOR = "TriggerSelector";              //Standard
        const string TRIGGER_SELECTOR_FRAME_START = "FrameStart";       //Standard
        const string TRIGGER_SELECTOR_EXPOSURE_START = "ExposureStart"; //Standard
        const string TRIGGER_MODE = "TriggerMode";                      //Standard
        const string TRIGGER_MODE_ON = "On";                            //Standard
        const string TRIGGER_MODE_OFF = "Off";                            //Standard
        const string TRIGGER_SOURCE = "TriggerSource";                  //Standard
        const string TRIGGER_SOURCE_SOFTWARE = "Software";              //Standard
        const string TRIGGER_SOFTWARE = "TriggerSoftware";              //Standard

        const string EXPOSURE_AUTO = "ExposureAuto";            //Standard
        const string GAIN_AUTO = "GainAuto";                    //Standard
        const string BALANCE_WHITE_AUTO = "BalanceWhiteAuto";   //Standard

        const string AUTO_LIGHT_TARGET = "AutoLightTarget";     //Custom
        const string GAIN = "Gain";                             //Standard
        const string GAIN_RAW = "GainRaw";                      //Custom

        const string EXPOSURE_MODE = "ExposureMode";            //Standard
        const string EXPOSURE_TIME = "ExposureTime";            //Standard
        const string EXPOSURE_TIME_RAW = "ExposureTimeRaw";     //Custom

        const string BALANCE_RATIO_SELECTOR = "BalanceRatioSelector";   //Standard
        const string BALANCE_RATIO = "BalanceRatio";			//Standard

        public enum TriggerSource
        {
            Software = 0,
            Line0 = 1,
            Hardware = 2
        }

        public enum AcquisitionMode
        {
            Continuous = 0,
            SingleFrame = 1,
            MultiFrame = 2
        }

        /// <summary>
        /// 클래스 메모리 설정
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            try
            {
                _cStApiAutoInit = new CStApiAutoInit();
                _cStSystem = new CStSystem();
                _cStDeviceArray = new CStDeviceArray();
                _cStDataStreamArray = new CStDataStreamArray();

                while (true)
                {
                    CStDevice device = null;

                    try
                    {
                        device = _cStSystem.CreateFirstStDevice();
                    }
                    catch
                    {
                        if (_cStDeviceArray.GetSize() == 0)
                        {
                            throw;
                        }

                        break;
                    }

                    _cStDeviceArray.Register(device);
                    _cStDataStreamArray.Register(device.CreateStDataStream(0));
                }

                _cStStreamArrayCallbackHandler = new IntPtr[_cStDeviceArray.GetSize()];

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        /// <summary>
        /// 클래스 메모리 해제
        /// </summary>
        public void Release()
        {
            if (_cStDataStreamArray != null)
                _cStDataStreamArray.Dispose();

            if (_cStDeviceArray != null)
                _cStDeviceArray.Dispose();

            if (_cStSystem != null)
                _cStSystem.Dispose();

            if (_cStApiAutoInit != null)
                _cStApiAutoInit.Dispose();
        }

        /// <summary>
        /// 촬영 1회
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public Bitmap Grab(uint cameraNumber, uint timeOut)
        {
            IStImage stImage = null;

            _cStDataStreamArray[cameraNumber].StartAcquisition();
            

            _cStDeviceArray[cameraNumber].AcquisitionStart();

            CStStreamBuffer streamBuffer = null;

            while (_cStDataStreamArray[cameraNumber].IsGrabbing)
            {
                streamBuffer = _cStDataStreamArray[cameraNumber].RetrieveBuffer(timeOut);

                if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent == true)
                {
                    stImage = streamBuffer.GetIStImage();
                    break;
                }
            }

            _cStDeviceArray[cameraNumber].AcquisitionStop();

            _cStDataStreamArray[cameraNumber].StopAcquisition();

            if (stImage != null)
            {
                ///230330 1626 CreateBitmap 메서드의 존재로 필요없는 로직

                Bitmap resultBitmap = CreateBitmap(stImage);

                stImage = null;

                if (streamBuffer != null)
                    streamBuffer.Dispose();

                return resultBitmap;
            }
            else
            {

                if (streamBuffer != null)
                    streamBuffer.Dispose();

                return null;
            }
        }

        //public Bitmap Grap2(uint timeOut)
        //{

        //}

        /// <summary>
        /// 트리거 모드 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="state"></param>
        public void SetTriggerMode(uint cameraNumber, bool state)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            if (state == true)
                SetEnumeration(nodeMapRemote, TRIGGER_MODE, TRIGGER_MODE_ON);
            else
                SetEnumeration(nodeMapRemote, TRIGGER_MODE, TRIGGER_MODE_OFF);
        }

        /// <summary>
        /// 취득 모드 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="mode"></param>
        public void SetAcquisitionMode(uint cameraNumber, AcquisitionMode mode)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            if (mode == AcquisitionMode.Continuous)
                SetEnumeration(nodeMapRemote, ACQUISITION_MODE, "Continuous");
            else if (mode == AcquisitionMode.SingleFrame)
                SetEnumeration(nodeMapRemote, ACQUISITION_MODE, "SingleFrame");
            else if (mode == AcquisitionMode.MultiFrame)
                SetEnumeration(nodeMapRemote, ACQUISITION_MODE, "MultiFrame");
        }

        /// <summary>
        /// 자동 노출 시간 기능 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="state"></param>
        public void SetAutoExposure(uint cameraNumber, bool state)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            if (state == true)
                SetEnumeration(nodeMapRemote, EXPOSURE_AUTO, "Continuous");
            else
                SetEnumeration(nodeMapRemote, EXPOSURE_AUTO, "Off");
        }

        /// <summary>
        /// 자동 대비 기능 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="state"></param>
        public void SetAutoGain(uint cameraNumber, bool state)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            if (state == true)
                SetEnumeration(nodeMapRemote, GAIN_AUTO, "Continuous");
            else
                SetEnumeration(nodeMapRemote, GAIN_AUTO, "Off");
        }

        /// <summary>
        /// 트리거 방식 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="source"></param>
        public void SetTriggerSource(uint cameraNumber, TriggerSource source)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            if (source == TriggerSource.Software)
                SetEnumeration(nodeMapRemote, TRIGGER_SOURCE, "Software");
            else if (source == TriggerSource.Line0)
                SetEnumeration(nodeMapRemote, TRIGGER_SOURCE, "Line0");
            else if (source == TriggerSource.Hardware)
                SetEnumeration(nodeMapRemote, TRIGGER_SOURCE, "Hardware");
        }

        /// <summary>
        /// 노출 시간 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="exposureTime"></param>
        public void SetExposureTime(uint cameraNumber, float exposureTime)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            SetFloatEnumeration(nodeMapRemote, EXPOSURE_TIME, exposureTime);
        }



        public void SetManualExposureTime(uint cameraNumber)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();
            float exposureTime = 100.0f;
            SetFloatEnumeration(nodeMapRemote, EXPOSURE_TIME, exposureTime);
        }


        /// <summary>
        /// 대비 설정
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="gain"></param>
        public void SetGain(uint cameraNumber, float gain)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            SetFloatEnumeration(nodeMapRemote, GAIN, gain);
        }

        /// <summary>
        /// 이벤트 등록
        /// </summary>
        /// <param name="cameraNumber"></param>
        /// <param name="handler"></param>
        /// <param name="parameter"></param>
        public bool RegisterCallback(uint cameraNumber, CallbackEventHandler handler)
        {
            try
            {
                _cStStreamArrayCallbackHandler[cameraNumber] = _cStDataStreamArray[cameraNumber].RegisterCallbackMethod(handler, null);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 이벤트 해제
        /// </summary>
        /// <param name="cameraNumber"></param>
        public bool DeregisterCallback(uint cameraNumber)
        {
            try
            {
                if (_cStStreamArrayCallbackHandler != null && _cStStreamArrayCallbackHandler[cameraNumber] != null)
                {
                    _cStDataStreamArray[cameraNumber].DeregisterCallbackMethod(_cStStreamArrayCallbackHandler[cameraNumber]);
                    _cStStreamArrayCallbackHandler[cameraNumber] = IntPtr.Zero;
                }
            }
            catch
            {
                return true;
            }
            return false;
        }

        public void SetTriggerSelector(uint cameraNumber)
        {
            INodeMap nodeMapRemote = _cStDeviceArray[cameraNumber].GetRemoteIStPort().GetINodeMap();

            SetEnumeration(nodeMapRemote, TRIGGER_SELECTOR, TRIGGER_SELECTOR_FRAME_START);
        }

        public bool StreamAcquisitionStart(uint cameraNumber)
        {
            try
            {
                _cStDataStreamArray[cameraNumber].StartAcquisition();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void StreamAcquisitionStop(uint cameraNumber)
        {
            _cStDataStreamArray[cameraNumber].StopAcquisition();
        }

        public bool DeviceAcquisitionStart(uint cameraNumber)
        {
            try
            {
                _cStDeviceArray[cameraNumber].AcquisitionStart();
            }
            catch
            {
                return false;
            }
            return true;
        }



        public void DeviceAcquisitionStop(uint cameraNumber)
        {
            _cStDeviceArray[cameraNumber].AcquisitionStop();
        }


        public Bitmap ConvertStImageToBitmap(IStImage stImage)
        {
            byte[] stImageByteArray = stImage.GetByteArray();

            Bitmap resultBitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format8bppIndexed);
            BitmapData resultBitmapData = resultBitmap.LockBits(new Rectangle(0, 0, (int)stImage.ImageWidth, (int)stImage.ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr resultBitmapIntPtr = resultBitmapData.Scan0;
            int resultBitmapLength = Math.Abs(resultBitmapData.Stride) * resultBitmap.Height;
            byte[] resultBitmapBytesValue = new byte[resultBitmapLength];

            Marshal.Copy(stImageByteArray, 0, resultBitmapIntPtr, stImageByteArray.Length);

            resultBitmap.UnlockBits(resultBitmapData);

            return (Bitmap)resultBitmap.Clone();
        }

            Bitmap m_Bitmap = null;
            CStPixelFormatConverter m_Converter = null;

        public Bitmap CreateBitmap(IStImage stImage)
        {
            if (m_Converter == null)
            {
                m_Converter = new CStPixelFormatConverter();
            }

            bool isColor = CStApiDotNet.GetIStPixelFormatInfo(stImage.ImagePixelFormat).IsColor;

            if (isColor)
            {
                // Convert the image data to BGR8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.BGR8;
            }
            else
            {
                // Convert the image data to Mono8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.Mono8;
            }

            if (m_Bitmap != null)
            {
                if ((m_Bitmap.Width != (int)stImage.ImageWidth) || (m_Bitmap.Height != (int)stImage.ImageHeight))
                {
                    m_Bitmap.Dispose();
                    m_Bitmap = null;
                }
            }

            if (m_Bitmap == null)
            {
                if (isColor)
                {
                    //m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format32bppRgb);
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format24bppRgb);
                }
                else
                {
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = m_Bitmap.Palette;
                    for (int i = 0; i < 256; ++i) palette.Entries[i] = Color.FromArgb(i, i, i);
                    m_Bitmap.Palette = palette;
                }
            }

            using (CStImageBuffer imageBuffer = CStApiDotNet.CreateStImageBuffer())
            {
                m_Converter.Convert(stImage, imageBuffer);

                // Lock the bits of the bitmap.
                BitmapData bmpData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.WriteOnly, m_Bitmap.PixelFormat);

                // Place the pointer to the buffer of the bitmap.
                IntPtr ptrBmp = bmpData.Scan0;
                byte[] imageData = imageBuffer.GetIStImage().GetByteArray();
                Marshal.Copy(imageData, 0, ptrBmp, imageData.Length);
                m_Bitmap.UnlockBits(bmpData);
            }

            return m_Bitmap;
        }
        



        private static void SetEnumeration(INodeMap nodeMap, string enumerationName, string valueName)
        {
            // Get the IEnum interface.
            IEnum enumNode = nodeMap.GetNode<IEnum>(enumerationName);

            // Update the settings using the IEnum interface.
            enumNode.StringValue = valueName;
        }

        private static void SetFloatEnumeration(INodeMap nodeMap, string enumerationName, float value)
        {
            // Get the IEnum interface.
            FloatNode enumNode = nodeMap.GetNode<FloatNode>(enumerationName);

            // Update the settings using the IEnum interface.
            enumNode.Value = value;
        }

        public bool CheckDevice(string serialNumber, out uint cameraNum)
        {
            cameraNum = 0;
            bool checkResut = false;

            for (uint i = 0; i < _cStDeviceArray.GetSize(); i++)
            {
                string deviceSerialNumber = _cStDeviceArray[i].GetIStDeviceInfo().SerialNumber;

                if (deviceSerialNumber == serialNumber)
                {
                    cameraNum = (uint)i;
                    checkResut = true;
                    break;
                }
            }

            return checkResut;
        }

        public bool Camera_Setup_Set(uint camera, float exposure)
        {
            try
            {
                SetExposureTime(camera, exposure);

                return true;
            }
            catch
            {
                return false;
            }

        }


        public bool Camera_Setup_ManualSet(uint camera)
        {
            try
            {
                SetManualExposureTime(camera);

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
