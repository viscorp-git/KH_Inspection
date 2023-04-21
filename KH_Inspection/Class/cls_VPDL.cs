using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using KProcessing;
using ViDi2;

namespace KH_Inspection
{
    public class cls_VPDL

    {
        private ViDi2.Runtime.IControl m_IControl;
        public ViDi2.Runtime.IWorkspace m_IWorkspace;
        public ViDi2.Runtime.IStream[] m_IStream;
        public ISample[] m_ISample;

        Thread Main;
        Bitmap[] testImage;


        KProcessing.ImageProcessing m_CImageProcessing;

        public cls_VPDL(GpuMode p_GpuMode)
        {
            m_CImageProcessing = new ImageProcessing();

            m_IControl = new ViDi2.Runtime.Local.Control(GpuMode.Deferred);
            m_IControl.InitializeComputeDevices(p_GpuMode, new List<int>() { });
        }

        public bool Load_Workspace(string pstr_Filepath)
        {

            try
            {
                for (int i = 0; i < m_IControl.Workspaces.Names.Count; i++)
                    m_IControl.Workspaces.Remove(m_IControl.Workspaces.Names[i]);

                m_IWorkspace = null;
                m_IWorkspace = m_IControl.Workspaces.Add("workspace", pstr_Filepath);

                m_IStream = new ViDi2.Runtime.IStream[m_IWorkspace.Streams.Count];
                m_ISample = new ISample[m_IWorkspace.Streams.Count];

                for (int i = 0; i < m_IWorkspace.Streams.Count; i++)
                {
                    m_IStream[i] = m_IWorkspace.Streams[m_IWorkspace.Streams.Names[i]];
                    m_ISample[i] = m_IStream[i].CreateSample();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool[] Excute_Run(Bitmap[] pBmp_Image, int pnt_StreamNumber, CogRecordDisplay[] p_CogRecordDisplay)
        {
            //testImage = null;
            //testImage = new Bitmap[2];

            bool[] l_blArrayResult = { true, true, false };

            IRedMarking[] l_IRedMarking_Inspection = null;

            for (int i = 0; i < p_CogRecordDisplay.Length; i++)
                p_CogRecordDisplay[i].StaticGraphics.Clear();

            //2022_05_18 use 1 Streams
            IImage l_IImage_1 = new FormsImage(pBmp_Image[0]);
            IImage l_IImage_2 = new FormsImage(pBmp_Image[1]);

            ISample l_ISample_1 = m_IStream[0].CreateSample(l_IImage_1);
            ISample l_ISample_2 = m_IStream[0].CreateSample(l_IImage_2);

            l_ISample_1.Process(null, new List<int>() { 0 });
            l_ISample_2.Process(null, new List<int>() { 0 });

            IRedMarking[] l_IRedMarking = { l_ISample_1.Markings["INS"] as IRedMarking, l_ISample_2.Markings["INS"] as IRedMarking };

            IRedMarking[] l_IRedMarking_Inspection_0 = new IRedMarking[] { l_IRedMarking[0] };
            IRedMarking[] l_IRedMarking_Inspection_1 = new IRedMarking[] { l_IRedMarking[1] };


            CogGraphicLabel l_CogGraphicLabel = new CogGraphicLabel();
            l_CogGraphicLabel.X = 120;
            l_CogGraphicLabel.Y = 120;
            Font l_Font = l_CogGraphicLabel.Font;
            l_CogGraphicLabel.Font = new Font(l_Font.FontFamily, 15, FontStyle.Bold);

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                else if (i == 1)
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;

                if (l_IRedMarking[i].Views.Count != 0)
                {

                    p_CogRecordDisplay[i].Image = new CogImage8Grey(l_IRedMarking[i].ViewImage(0).Bitmap);

                    //if (i == 0)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                    //else if (i == 1)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;

                    ArrayList polygonList = new ArrayList(0);

                    for (int j = 0; j < l_IRedMarking_Inspection[0].Views[0].Regions.Count; j++)
                    {
                        IRegion l_IRegion = l_IRedMarking_Inspection[0].Views[0].Regions[j];
                        System.Collections.ObjectModel.ReadOnlyCollection<ViDi2.Point> l_PtOuter = l_IRegion.Outer;

                        CogPolygon l_CogPolygon = new CogPolygon();
                        l_CogPolygon.Color = CogColorConstants.Red;
                        l_CogPolygon.FillMode = CogPolygonFillConstants.WindingNumber;
                        l_CogPolygon.LineWidthInScreenPixels = 2;

                        for (int k = 0; k < l_PtOuter.Count; k++)
                        {
                            l_CogPolygon.AddVertex(l_PtOuter[k].X, l_PtOuter[k].Y, k);
                        }

                        if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Green;
                            l_CogGraphicLabel.Text = "OK";
                            l_blArrayResult[i] = true;
                        }
                        else if (l_IRedMarking_Inspection[0].Views[0].Regions.Count != 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Red;
                            l_CogGraphicLabel.Text = "NG";
                            l_blArrayResult[i] = false;
                        }
                        p_CogRecordDisplay[i].StaticGraphics.Add(l_CogPolygon, "");
                    }

                    if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                    {
                        l_CogGraphicLabel.Color = CogColorConstants.Green;
                        l_CogGraphicLabel.Text = "OK";
                    }
                }
                else
                {
                    p_CogRecordDisplay[i].Image = new CogImage8Grey(pBmp_Image[i]);
                    l_CogGraphicLabel.Color = CogColorConstants.Red;
                    l_CogGraphicLabel.Text = "NG";
                    l_blArrayResult[i] = false;
                }
                p_CogRecordDisplay[i].StaticGraphics.Add(l_CogGraphicLabel, "");
            }


            if (l_blArrayResult[0] == true && l_blArrayResult[1] == true)
            {
                l_blArrayResult[2] = true;
            }

            return l_blArrayResult;
        }


        //2023/01/10 - ekjeong
        public bool[] Excute_Run_V1(Bitmap[] pBmp_Image, int pnt_StreamNumber, CogRecordDisplay[] p_CogRecordDisplay)
        {
            //testImage = null;
            //testImage = new Bitmap[2];

            bool[] l_blArrayResult = { true, true, false };

            IRedMarking[] l_IRedMarking_Inspection = null;
            IBlueMarking[] IIBluMarking_Inspection = null;

            for (int i = 0; i < p_CogRecordDisplay.Length; i++)
                p_CogRecordDisplay[i].StaticGraphics.Clear();

            //2022_05_18 use 1 Streams
            IImage l_IImage_1 = new FormsImage(pBmp_Image[0]);
            IImage l_IImage_2 = new FormsImage(pBmp_Image[1]);

            ISample l_ISample_1 = m_IStream[0].CreateSample(l_IImage_1);
            ISample l_ISample_2 = m_IStream[0].CreateSample(l_IImage_2);

            l_ISample_1.Process(null, new List<int>() { 0 });
            l_ISample_2.Process(null, new List<int>() { 0 });

            IRedMarking[] l_IRedMarking = { l_ISample_1.Markings["INS"] as IRedMarking, l_ISample_2.Markings["INS"] as IRedMarking };
            IBlueMarking[] I_IblueMarkings = { l_ISample_1.Markings["Locate"] as IBlueMarking, l_ISample_2.Markings["Locate"] as IBlueMarking };

            IRedMarking[] l_IRedMarking_Inspection_0 = new IRedMarking[] { l_IRedMarking[0] };
            IRedMarking[] l_IRedMarking_Inspection_1 = new IRedMarking[] { l_IRedMarking[1] };

            IBlueMarking[] l_IBlueMarking_Inspection_0 = new IBlueMarking[] { I_IblueMarkings[0] };
            IBlueMarking[] l_IBlueMarking_Inspection_1 = new IBlueMarking[] { I_IblueMarkings[1] };


            CogGraphicLabel l_CogGraphicLabel = new CogGraphicLabel();
            l_CogGraphicLabel.X = 120;
            l_CogGraphicLabel.Y = 120;
            Font l_Font = l_CogGraphicLabel.Font;
            l_CogGraphicLabel.Font = new Font(l_Font.FontFamily, 15, FontStyle.Bold);

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                    IIBluMarking_Inspection = l_IBlueMarking_Inspection_0;
                }
                else if (i == 1)
                {
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;
                    IIBluMarking_Inspection = l_IBlueMarking_Inspection_1;
                }


                if (l_IRedMarking[i].Views.Count != 0)
                {

                    p_CogRecordDisplay[i].Image = new CogImage8Grey(l_IRedMarking[i].ViewImage(0).Bitmap);

                    //if (i == 0)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                    //else if (i == 1)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;

                    ArrayList polygonList = new ArrayList(0);

                    for (int j = 0; j < l_IRedMarking_Inspection[0].Views[0].Regions.Count; j++)
                    {
                        IRegion l_IRegion = l_IRedMarking_Inspection[0].Views[0].Regions[j];
                        System.Collections.ObjectModel.ReadOnlyCollection<ViDi2.Point> l_PtOuter = l_IRegion.Outer;

                        CogPolygon l_CogPolygon = new CogPolygon();
                        l_CogPolygon.Color = CogColorConstants.Red;
                        l_CogPolygon.FillMode = CogPolygonFillConstants.WindingNumber;
                        l_CogPolygon.LineWidthInScreenPixels = 2;

                        for (int k = 0; k < l_PtOuter.Count; k++)
                        {
                            l_CogPolygon.AddVertex(l_PtOuter[k].X, l_PtOuter[k].Y, k);
                        }

                        if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Green;
                            l_CogGraphicLabel.Text = "OK";
                            l_blArrayResult[i] = true;
                        }
                        else if (l_IRedMarking_Inspection[0].Views[0].Regions.Count != 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Red;
                            l_CogGraphicLabel.Text = "NG";
                            l_blArrayResult[i] = false;
                        }
                        p_CogRecordDisplay[i].StaticGraphics.Add(l_CogPolygon, "");
                    }

                    if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                    {
                        l_CogGraphicLabel.Color = CogColorConstants.Green;
                        l_CogGraphicLabel.Text = "OK";
                    }
                }
                else
                {
                    p_CogRecordDisplay[i].Image = new CogImage8Grey(pBmp_Image[i]);
                    l_CogGraphicLabel.Color = CogColorConstants.Red;
                    l_CogGraphicLabel.Text = "NG";
                    l_blArrayResult[i] = false;
                }
                p_CogRecordDisplay[i].StaticGraphics.Add(l_CogGraphicLabel, "");
            }


            if (l_blArrayResult[0] == true && l_blArrayResult[1] == true)
            {
                l_blArrayResult[2] = true;
            }

            return l_blArrayResult;
        }





        public bool[] Execute_Sebang(Bitmap[] pBmp_Image, int pnt_StreamNumber, CogRecordDisplay[] p_CogRecordDisplay)
        {
            //testImage = null;
            //testImage = new Bitmap[2];

            bool[] l_blArrayResult = { true, true, false };

            IRedMarking[] l_IRedMarking_Inspection = null;

            for (int i = 0; i < p_CogRecordDisplay.Length; i++)
                p_CogRecordDisplay[i].StaticGraphics.Clear();

            //2022_05_18 use 1 Streams
            IImage l_IImage_1 = new FormsImage(pBmp_Image[0]);
            IImage l_IImage_2 = new FormsImage(pBmp_Image[1]);

            ISample l_ISample_1 = m_IStream[0].CreateSample(l_IImage_1);
            ISample l_ISample_2 = m_IStream[0].CreateSample(l_IImage_2);

            l_ISample_1.Process(null, new List<int>() { 0 });
            l_ISample_2.Process(null, new List<int>() { 0 });

            IRedMarking[] l_IRedMarking = { l_ISample_1.Markings["INS"] as IRedMarking, l_ISample_2.Markings["INS"] as IRedMarking };

            IRedMarking[] l_IRedMarking_Inspection_0 = new IRedMarking[] { l_IRedMarking[0] };
            IRedMarking[] l_IRedMarking_Inspection_1 = new IRedMarking[] { l_IRedMarking[1] };


            CogGraphicLabel l_CogGraphicLabel = new CogGraphicLabel();
            l_CogGraphicLabel.X = 120;
            l_CogGraphicLabel.Y = 120;
            Font l_Font = l_CogGraphicLabel.Font;
            l_CogGraphicLabel.Font = new Font(l_Font.FontFamily, 15, FontStyle.Bold);

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                else if (i == 1)
                    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;

                if (l_IRedMarking[i].Views.Count != 0)
                {

                    p_CogRecordDisplay[i].Image = new CogImage8Grey(l_IRedMarking[i].ViewImage(0).Bitmap);

                    //if (i == 0)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_0;
                    //else if (i == 1)
                    //    l_IRedMarking_Inspection = l_IRedMarking_Inspection_1;

                    ArrayList polygonList = new ArrayList(0);

                    for (int j = 0; j < l_IRedMarking_Inspection[0].Views[0].Regions.Count; j++)
                    {
                        IRegion l_IRegion = l_IRedMarking_Inspection[0].Views[0].Regions[j];
                        System.Collections.ObjectModel.ReadOnlyCollection<ViDi2.Point> l_PtOuter = l_IRegion.Outer;

                        CogPolygon l_CogPolygon = new CogPolygon();
                        l_CogPolygon.Color = CogColorConstants.Red;
                        l_CogPolygon.FillMode = CogPolygonFillConstants.WindingNumber;
                        l_CogPolygon.LineWidthInScreenPixels = 2;

                        for (int k = 0; k < l_PtOuter.Count; k++)
                        {
                            l_CogPolygon.AddVertex(l_PtOuter[k].X, l_PtOuter[k].Y, k);
                        }

                        if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Green;
                            l_CogGraphicLabel.Text = "OK";
                            l_blArrayResult[i] = true;
                        }
                        else if (l_IRedMarking_Inspection[0].Views[0].Regions.Count != 0)
                        {
                            l_CogGraphicLabel.Color = CogColorConstants.Red;
                            l_CogGraphicLabel.Text = "NG";
                            l_blArrayResult[i] = false;
                        }
                        p_CogRecordDisplay[i].StaticGraphics.Add(l_CogPolygon, "");
                    }

                    if (l_IRedMarking_Inspection[0].Views[0].Regions.Count == 0)
                    {
                        l_CogGraphicLabel.Color = CogColorConstants.Green;
                        l_CogGraphicLabel.Text = "OK";
                    }
                }
                else
                {
                    p_CogRecordDisplay[i].Image = new CogImage8Grey(pBmp_Image[i]);
                    l_CogGraphicLabel.Color = CogColorConstants.Red;
                    l_CogGraphicLabel.Text = "NG";
                    l_blArrayResult[i] = false;
                }
                p_CogRecordDisplay[i].StaticGraphics.Add(l_CogGraphicLabel, "");
            }


            if (l_blArrayResult[0] == true && l_blArrayResult[1] == true)
            {
                l_blArrayResult[2] = true;
            }

            return l_blArrayResult;
        }

        public Bitmap Execute_ExtractImage(Bitmap pBmp_Image, int pnt_StreamNumber, string pstr_ToolName, CogDisplay pC_Cogdisplay)
        {
            System.Diagnostics.Stopwatch l_SW = new System.Diagnostics.Stopwatch();
            l_SW.Start();

            IImage l_IImage = new FormsImage((Bitmap)pBmp_Image.Clone());

            m_ISample[pnt_StreamNumber].AddImage(l_IImage);

            ViDi2.ITool l_ITool_Analyze = m_ISample[pnt_StreamNumber].Tools["Locate"].Children["Analyze"];

            m_ISample[pnt_StreamNumber].Process(l_ITool_Analyze);

            IRedMarking l_IRedMarking_Analyze = m_ISample[pnt_StreamNumber].Markings["Analyze"] as IRedMarking;

            IRedView l_IRedView_Analyze = l_IRedMarking_Analyze.Views[0];

            System.Collections.ObjectModel.ReadOnlyCollection<ViDi2.Point> l_PtOuter = l_IRedView_Analyze.Regions[0].Outer;

            PointF[] l_PointF_Data = new PointF[l_PtOuter.Count];

            for (int i = 0; i < l_PtOuter.Count; i++)
                l_PointF_Data[i] = new System.Drawing.PointF((float)l_PtOuter[i].X, (float)l_PtOuter[i].Y);

            Bitmap l_BmpImage = new Bitmap(l_IRedMarking_Analyze.ViewImage(0).Bitmap.Width, l_IRedMarking_Analyze.ViewImage(0).Bitmap.Height, PixelFormat.Format24bppRgb);

            byte[] l_byteData = new byte[l_BmpImage.Width * l_BmpImage.Height * 3];

            Parallel.For(0, l_byteData.Length, (int i) =>
            {
                l_byteData[i] = 0;
            });

            BitmapData l_BD = l_BmpImage.LockBits(new Rectangle(0, 0, l_BmpImage.Width, l_BmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            IntPtr l_IntPtr = l_BD.Scan0;

            Marshal.Copy(l_byteData, 0, l_IntPtr, l_byteData.Length);

            l_BmpImage.UnlockBits(l_BD);

            Graphics l_Graphics = Graphics.FromImage(l_BmpImage);

            l_Graphics.FillPolygon(new SolidBrush(Color.White), l_PointF_Data);

            l_SW.Stop();
            MessageBox.Show(l_SW.ElapsedMilliseconds.ToString());

            return (Bitmap)l_BmpImage.Clone();
        }

        public Bitmap Execute_ExtractImage_V2(Bitmap pBmp_Image, int pnt_StreamNumber, string pstr_ToolName, CogDisplay pC_Cogdisplay)
        {
            System.Diagnostics.Stopwatch l_SW = new System.Diagnostics.Stopwatch();
            l_SW.Start();

            IImage l_IImage = new FormsImage((Bitmap)pBmp_Image.Clone());

            m_ISample[pnt_StreamNumber].AddImage(l_IImage);

            ViDi2.ITool l_ITool_Analyze = m_ISample[pnt_StreamNumber].Tools["Locate"].Children["Analyze"];

            m_ISample[pnt_StreamNumber].Process(l_ITool_Analyze);

            IRedMarking l_IRedMarking_Analyze = m_ISample[pnt_StreamNumber].Markings["Analyze"] as IRedMarking;

            IRedView l_IRedView_Analyze = l_IRedMarking_Analyze.Views[0];

            System.Collections.ObjectModel.ReadOnlyCollection<ViDi2.Point> l_PtOuter = l_IRedView_Analyze.Regions[0].Outer;

            CogPolygon l_CogPolygon = new CogPolygon();

            for (int i = 0; i < l_PtOuter.Count; i++)
                l_CogPolygon.AddVertex(l_PtOuter[i].X, l_PtOuter[i].Y, i);

            pC_Cogdisplay.StaticGraphics.Add(l_CogPolygon, "");

            return null;
        }

        public Bitmap Execute_CropTest_V2(Bitmap pBmp_Image, int pnt_StreamNumber, string pstr_ToolName, CogDisplay pC_Cogdisplay)
        {
            IImage l_IImage = new FormsImage((Bitmap)pBmp_Image.Clone());

            m_ISample[pnt_StreamNumber].AddImage(l_IImage);

            ViDi2.ITool l_ITool_Locate = m_ISample[pnt_StreamNumber].Tools["Locate"];

            m_ISample[pnt_StreamNumber].Process(l_ITool_Locate);

            IBlueMarking l_IBlueMarking_Locate = m_ISample[pnt_StreamNumber].Markings["Locate"] as IBlueMarking;

            IBlueView l_IBlueView_Locate = l_IBlueMarking_Locate.Views[0];

            ViDi2.Point l_PtRegion = l_IBlueView_Locate.Features[0].Position;
            ViDi2.Size l_Size = l_IBlueView_Locate.Features[0].Size;
            double l_dlAngle = l_IBlueView_Locate.Features[0].Angle;

            CogRectangleAffine l_CogRectangleAffine = new CogRectangleAffine();
            l_CogRectangleAffine.CenterX = l_PtRegion.X;
            l_CogRectangleAffine.CenterY = l_PtRegion.Y;
            l_CogRectangleAffine.SideXLength = l_Size.Width;
            l_CogRectangleAffine.SideYLength = l_Size.Height;
            l_CogRectangleAffine.Rotation = l_dlAngle;

            double l_dlStartX = l_CogRectangleAffine.CornerOriginX;
            double l_dlStartY = l_CogRectangleAffine.CornerOriginY;

            System.Diagnostics.Stopwatch l_SW = new System.Diagnostics.Stopwatch();
            l_SW.Start();

            Bitmap[] l_BmpImage_Result = new Bitmap[2];

            Image l_Image = (Bitmap)pBmp_Image.Clone();

            Image l_BmpImage = RotateImage(l_Image, -(float)CogMisc.RadToDeg(l_dlAngle));

            return (Bitmap)l_BmpImage.Clone();
        }

        public static Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);

            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new System.Drawing.Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }

        public Bitmap Execute_CropTest(Bitmap pBmp_Image, int pnt_StreamNumber, string pstr_ToolName, CogDisplay pC_Cogdisplay)
        {
            IImage l_IImage = new FormsImage((Bitmap)pBmp_Image.Clone());

            m_ISample[pnt_StreamNumber].AddImage(l_IImage);

            ViDi2.ITool l_ITool_Locate = m_ISample[pnt_StreamNumber].Tools["Locate"];

            m_ISample[pnt_StreamNumber].Process(l_ITool_Locate);

            IBlueMarking l_IBlueMarking_Locate = m_ISample[pnt_StreamNumber].Markings["Locate"] as IBlueMarking;

            IBlueView l_IBlueView_Locate = l_IBlueMarking_Locate.Views[0];

            ViDi2.Point l_PtRegion = l_IBlueView_Locate.Features[0].Position;
            ViDi2.Size l_Size = l_IBlueView_Locate.Features[0].Size;
            double l_dlAngle = l_IBlueView_Locate.Features[0].Angle;

            CogRectangleAffine l_CogRectangleAffine = new CogRectangleAffine();
            l_CogRectangleAffine.CenterX = l_PtRegion.X;
            l_CogRectangleAffine.CenterY = l_PtRegion.Y;
            l_CogRectangleAffine.SideXLength = l_Size.Width;
            l_CogRectangleAffine.SideYLength = l_Size.Height;
            l_CogRectangleAffine.Rotation = l_dlAngle;

            double l_dlStartX = l_CogRectangleAffine.CornerOriginX;
            double l_dlStartY = l_CogRectangleAffine.CornerOriginY;

            System.Diagnostics.Stopwatch l_SW = new System.Diagnostics.Stopwatch();
            l_SW.Start();

            Bitmap[] l_BmpImage_Result = new Bitmap[2];

            for (int i = 0; i < 2; i++)
                l_BmpImage_Result[i] = (Bitmap)pBmp_Image.Clone();

            Parallel.For(0, 2, (int i) =>
            {
                int l_ntNumber = i;
                Bitmap l_BmpImage_Original = (Bitmap)l_BmpImage_Result[i].Clone();

                Bitmap l_BmpImage = m_CImageProcessing.CropRotatedRect(l_BmpImage_Original,
                    new Rectangle(Convert.ToInt32(l_dlStartX), Convert.ToInt32(l_dlStartY), Convert.ToInt32(l_Size.Width), Convert.ToInt32(l_Size.Height)),
                    -(float)CogMisc.RadToDeg(l_dlAngle),
                    false);

                l_BmpImage_Result[l_ntNumber] = (Bitmap)l_BmpImage.Clone();
            });

            l_SW.Stop();
            MessageBox.Show(l_SW.ElapsedMilliseconds.ToString());

            //pC_Cogdisplay.StaticGraphics.Add(l_CogRectangleAffine, "");

            //CogImage8Grey l_CogImage8Grey = new CogImage8Grey(pBmp_Image);

            //CogCopyRegionTool l_CogCopyRegionTool = new CogCopyRegionTool();
            //l_CogCopyRegionTool.InputImage = l_CogImage8Grey;
            //l_CogCopyRegionTool.Region = l_CogRectangleAffine;
            //l_CogCopyRegionTool.Run();

            //return l_CogCopyRegionTool.OutputImage.ToBitmap();
            return (Bitmap)l_BmpImage_Result[0].Clone();
        }


    }
}
