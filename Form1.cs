using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace DeepTrainingTest
{
    public partial class VisionControl : UserControl
    {

        #region 变量定义

        int ErrCount;                                               //处理错误子程序
        object LvLock = new object();                               //处理错误子程序
        object ErrLock = new object();                              //处理错误子程序

        HTuple hv_ExpDefaultWinHandle_01;                           //窗口指针
        HTuple hv_ExpDefaultWinHandle_02;                           //窗口指针
        const int WINDOW_WIDTH = 512;      // 固定窗口宽度
        const int WINDOW_HEIGHT = 512;     // 固定窗口高度
                                           // 原图原始尺寸（用于分割结果缩放对齐）
        HTuple hv_ImgOrgWidth = new HTuple(), hv_ImgOrgHeight = new HTuple();
        HalconClass halconClass = new HalconClass();
        // Local iconic variables 

        HObject ho_Image, ho_SegmentationResult, ho_SegmentationResultScaled, ho_SegmentationRegion;
        // Local control variables 

        HTuple hv_DLModelHandle = new HTuple(), hv_ImageWidth = new HTuple();
        HTuple hv_ImageHeight = new HTuple(), hv_NumChannels = new HTuple();
        HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
        HTuple hv_ClassNames = new HTuple(), hv_ClassIDs = new HTuple();
        HTuple hv_DLPreprocessParam = new HTuple(), hv_DLDeviceHandles = new HTuple();
        HTuple hv_DLDevice = new HTuple(), hv_DLSample = new HTuple();

        // Local iconic variables 

        HObject ho_CCDImage = null;

        // Local control variables 

        HTuple hv_AcqHandle = new HTuple();


        HTuple hv_DLResult = new HTuple(), hv_Keys = new HTuple();
        HTuple hv_WindowHandle = new HTuple(), hv_DLDatasetInfo = new HTuple();
        HTuple hv_KeysDisplay = new HTuple(), hv_WindowDict = new HTuple();

        #endregion

        #region 初始化区
        public VisionControl()
        {
            InitializeComponent();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_SegmentationResult);
            HOperatorSet.GenEmptyObj(out ho_SegmentationRegion);
            HOperatorSet.GenEmptyObj(out ho_SegmentationResultScaled);
            HOperatorSet.GenEmptyObj(out ho_CCDImage);

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            hv_ExpDefaultWinHandle_01 = hSmartWindowControl1.HalconWindow; // 将第一个窗口控件的HalconWindow赋值给hv_ExpDefaultWinHandle_01
            hv_ExpDefaultWinHandle_02 = hSmartWindowControl2.HalconWindow; // 将第二个窗口控件的HalconWindow赋值给hv_ExpDefaultWinHandle_02
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.my_MouseWheel);       // 鼠标滚轮的响应函数注册

            //// 初始化窗口并设置为固定512x512尺寸
            //InitHalconWindow(out hv_ExpDefaultWinHandle_01, 0, 0, "窗口1");   // 第一个窗口
            //InitHalconWindow(out hv_ExpDefaultWinHandle_02, WINDOW_WIDTH + 10, 0, "窗口2"); // 第二个窗口（偏移10像素避免重叠）
        }


        #endregion

        #region 按钮响应区
        /// <summary>
        /// 加载模型文件，并获取模型的输入输出参数，创建预处理参数，查询可用设备并设置模型运行设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                hv_DLModelHandle.Dispose();
                HOperatorSet.ReadDlModel(@"E:\\榛子裂缝模型（语义分割）\\模型1.hdl",
                    out hv_DLModelHandle);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_height", out hv_ImageHeight);
                hv_NumChannels.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_num_channels", out hv_NumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_max", out hv_ImageRangeMax);
                hv_ClassNames.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                hv_ClassIDs.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);

                hv_DLPreprocessParam.Dispose();

                halconClass.create_dl_preprocess_param("segmentation", hv_ImageHeight, hv_ImageWidth, hv_NumChannels,
                    hv_ImageRangeMin, hv_ImageRangeMax, "none", "full_domain", new HTuple(),
                    new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);

                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out hv_DLDeviceHandles);
                if ((int)(new HTuple((new HTuple(hv_DLDeviceHandles.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    throw new HalconException("没有可用设备");
                }
                hv_DLDevice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLDevice = hv_DLDeviceHandles.TupleSelect(
                        0);
                }
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDevice);

                textBox1.AppendText("模型加载成功！\r\n"); 
            }
            catch(Exception ex)
            {

            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 释放旧资源
                ho_Image.Dispose();
                ho_SegmentationResult.Dispose();
                ho_SegmentationResultScaled.Dispose();
                ho_SegmentationRegion.Dispose();
                hv_DLSample.Dispose();
                hv_DLResult.Dispose();
                hv_Keys.Dispose();
                hv_ImgOrgWidth.Dispose();
                hv_ImgOrgHeight.Dispose();


                openFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory();     //注意：路径时要用\\，不是\
                openFileDialog1.Filter = "图像文件|*.BMP;*.JPG;*.TIFF;*.PNG,*.bmp;*.jpg;*.tiff;*.png|所有文件|*.*";
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FilterIndex = 1;

                //----------防止错误----------------------------------
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    textBox1.AppendText(System.Environment.NewLine + DateTime.Now.ToString("HH:mm:ss") + " 请选择文件." + System.Environment.NewLine);
                    return;
                }


                // 1. 读取测试图片，并记录原图原始尺寸
                HOperatorSet.ReadImage(out ho_Image, openFileDialog1.FileName);
                // 获取原图的原始像素尺寸（关键：用于分割结果缩放对齐）
                HOperatorSet.GetImageSize(ho_Image, out hv_ImgOrgWidth, out hv_ImgOrgHeight);

                // 2. 预处理图片并执行推理
                halconClass.gen_dl_samples_from_images(ho_Image, out hv_DLSample);
                halconClass.preprocess_dl_samples(hv_DLSample, hv_DLPreprocessParam);
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSample, new HTuple(), out hv_DLResult);

                // 3. 提取分割结果图像（模型输出尺寸，如512x512）
                HOperatorSet.GetDictObject(out ho_SegmentationResult, hv_DLResult, "segmentation_image");

                // 4. 关键步骤：将分割结果缩放到原图尺寸（保证位置完全匹配）
                // ZoomImageSize：按原图尺寸缩放分割结果，插值方式选'nearest_neighbor'避免模糊（分割结果是二值/分类图）
                HOperatorSet.ZoomImageSize(ho_SegmentationResult, out ho_SegmentationResultScaled,
                    hv_ImgOrgWidth, hv_ImgOrgHeight, "nearest_neighbor");

                // 5. 将缩放后的分割结果转换为区域（匹配原图像素位置）
                HOperatorSet.Threshold(ho_SegmentationResultScaled, out ho_SegmentationRegion, 1, 255);
                HOperatorSet.Connection(ho_SegmentationRegion, out ho_SegmentationRegion);
                HOperatorSet.Union1(ho_SegmentationRegion, out ho_SegmentationRegion);

                // 6. 计算分割区域的几何特征（基于原图尺寸，结果更准确）
                HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
                HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple(), hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
                HOperatorSet.AreaCenter(ho_SegmentationRegion, out hv_Area, out hv_Row, out hv_Column);
                HOperatorSet.SmallestRectangle1(ho_SegmentationRegion, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                HTuple hv_Width = hv_Column2 - hv_Column1;
                HTuple hv_Height = hv_Row2 - hv_Row1;

                // 7. 显示几何数据到MessageBox（尺寸是原图真实像素）
                string msg = $"分割区域几何数据（原图尺寸）：\n" +
                             $"面积：{hv_Area.D.ToString("F2")} 像素\n" +
                             $"中心坐标：({hv_Row.D.ToString("F2")}, {hv_Column.D.ToString("F2")})\n" +
                             $"最小外接矩形宽度：{hv_Width.D.ToString("F2")} 像素\n" +
                             $"最小外接矩形高度：{hv_Height.D.ToString("F2")} 像素\n" +
                             $"矩形对角点：({hv_Row1.D}, {hv_Column1.D}) - ({hv_Row2.D}, {hv_Column2.D})";
               textBox1.AppendText(msg + "\r\n");
                //MessageBox.Show(msg, "分割区域几何信息", MessageBoxButtons.OK, MessageBoxIcon.Information);


                // 8. 显示：窗口1显示分割结果，窗口2显示原图+分割区域（精准覆盖）

                Pubilc_Image_Display(ho_SegmentationResultScaled, hv_ExpDefaultWinHandle_01);

                Pubilc_Image_Display(ho_Image, hv_ExpDefaultWinHandle_02);
                // 窗口2：先显示原图（自适应），再叠加分割区域（已对齐原图位置）



                // 设置区域显示样式（绿色轮廓，2像素宽）
                HOperatorSet.SetColor(hv_ExpDefaultWinHandle_02, "green");
                HOperatorSet.SetLineWidth(hv_ExpDefaultWinHandle_02, 2);
                HOperatorSet.SetPaint(hv_ExpDefaultWinHandle_02, "default");
                // 显示分割区域（精准覆盖原图对应位置）
                HOperatorSet.DispObj(ho_SegmentationRegion, hv_ExpDefaultWinHandle_02);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }    

        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                hv_AcqHandle.Dispose();

                HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive",
                            -1, "gray", -1, "false", "default", "34bd207bca1c_Hikrobot_MVCH24010TMCNN",
                            0, -1, out hv_AcqHandle);

                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "[Consumer]gain_auto", "Continuous");
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

                //grab_image_async (Image, AcqHandle, -1)
                //while ((int)(1) != 0)
                //{
                //    ho_Image.Dispose();
                //    HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                //    //Image Acquisition 01: Do something
                //}

                ho_Image.Dispose();
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);

                Pubilc_Image_Display(ho_Image, hv_ExpDefaultWinHandle_01);


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        #endregion

        #region 子函数区

        //************************************************
        //       鼠标滚轮缩放窗口(静态照片窗口)          *
        //************************************************
        private void my_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Drawing.Point pt = this.Location;


            //-----------01#窗口鼠标响应-----------------------------------------------------------------
            int leftBorder01 = hSmartWindowControl1.Location.X;
            int rightBorder01 = hSmartWindowControl1.Location.X + hSmartWindowControl1.Size.Width;
            int topBorder01 = hSmartWindowControl1.Location.Y;
            int bottomBorder01 = hSmartWindowControl1.Location.Y + hSmartWindowControl1.Size.Height;

            //----只有在鼠标停留在hSmartWindowControl100窗口内部，滚轮才起作用--------------------------
            if (e.X > leftBorder01 && e.X < rightBorder01 && e.Y > topBorder01 && e.Y < bottomBorder01)
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
                hSmartWindowControl1.HSmartWindowControl_MouseWheel(sender, newe);
            }



            //-----------02#窗口鼠标响应-----------------------------------------------------------------
            int leftBorder02 = hSmartWindowControl2.Location.X;
            int rightBorder02 = hSmartWindowControl2.Location.X + hSmartWindowControl2.Size.Width;
            int topBorder02 = hSmartWindowControl2.Location.Y;
            int bottomBorder02 = hSmartWindowControl2.Location.Y + hSmartWindowControl2.Size.Height;

            //----只有在鼠标停留在hSmartWindowControl200窗口内部，滚轮才起作用--------------------------
            if (e.X > leftBorder02 && e.X < rightBorder02 && e.Y > topBorder02 && e.Y < bottomBorder02)
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
                hSmartWindowControl2.HSmartWindowControl_MouseWheel(sender, newe);
            }



            ////-----------03#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder03 = hSmartWindowControl3.Location.X;
            //int rightBorder03 = hSmartWindowControl3.Location.X + hSmartWindowControl3.Size.Width;
            //int topBorder03 = hSmartWindowControl3.Location.Y;
            //int bottomBorder03 = hSmartWindowControl3.Location.Y + hSmartWindowControl3.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl300窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder03 && e.X < rightBorder03 && e.Y > topBorder03 && e.Y < bottomBorder03)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl3.HSmartWindowControl_MouseWheel(sender, newe);
            //}


            ////-----------04#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder04 = hSmartWindowControl4.Location.X;
            //int rightBorder04 = hSmartWindowControl4.Location.X + hSmartWindowControl4.Size.Width;
            //int topBorder04 = hSmartWindowControl4.Location.Y;
            //int bottomBorder04 = hSmartWindowControl4.Location.Y + hSmartWindowControl4.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl400窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder04 && e.X < rightBorder04 && e.Y > topBorder04 && e.Y < bottomBorder04)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl4.HSmartWindowControl_MouseWheel(sender, newe);
            //}



            ////-----------05#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder05 = hSmartWindowControl5.Location.X;
            //int rightBorder05 = hSmartWindowControl5.Location.X + hSmartWindowControl5.Size.Width;
            //int topBorder05 = hSmartWindowControl5.Location.Y;
            //int bottomBorder05 = hSmartWindowControl5.Location.Y + hSmartWindowControl5.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl500窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder05 && e.X < rightBorder05 && e.Y > topBorder05 && e.Y < bottomBorder05)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl5.HSmartWindowControl_MouseWheel(sender, newe);
            //}



            ////-----------06#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder06 = hSmartWindowControl6.Location.X;
            //int rightBorder06 = hSmartWindowControl6.Location.X + hSmartWindowControl6.Size.Width;
            //int topBorder06 = hSmartWindowControl6.Location.Y;
            //int bottomBorder06 = hSmartWindowControl6.Location.Y + hSmartWindowControl6.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl600窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder06 && e.X < rightBorder06 && e.Y > topBorder06 && e.Y < bottomBorder06)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl6.HSmartWindowControl_MouseWheel(sender, newe);
            //}



            ////-----------07#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder07 = hSmartWindowControl7.Location.X;
            //int rightBorder07 = hSmartWindowControl7.Location.X + hSmartWindowControl7.Size.Width;
            //int topBorder07 = hSmartWindowControl7.Location.Y;
            //int bottomBorder07 = hSmartWindowControl7.Location.Y + hSmartWindowControl7.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl700窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder07 && e.X < rightBorder07 && e.Y > topBorder07 && e.Y < bottomBorder07)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl7.HSmartWindowControl_MouseWheel(sender, newe);
            //}

            ////-----------08#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder08 = hSmartWindowControl8.Location.X;
            //int rightBorder08 = hSmartWindowControl8.Location.X + hSmartWindowControl8.Size.Width;
            //int topBorder08 = hSmartWindowControl8.Location.Y;
            //int bottomBorder08 = hSmartWindowControl8.Location.Y + hSmartWindowControl8.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl800窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder08 && e.X < rightBorder08 && e.Y > topBorder08 && e.Y < bottomBorder08)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl8.HSmartWindowControl_MouseWheel(sender, newe);
            //}



            ////-----------09#窗口鼠标响应-----------------------------------------------------------------
            //int leftBorder09 = hSmartWindowControl9.Location.X;
            //int rightBorder09 = hSmartWindowControl9.Location.X + hSmartWindowControl9.Size.Width;
            //int topBorder09 = hSmartWindowControl9.Location.Y;
            //int bottomBorder09 = hSmartWindowControl9.Location.Y + hSmartWindowControl9.Size.Height;

            ////----只有在鼠标停留在hSmartWindowControl900窗口内部，滚轮才起作用--------------------------
            //if (e.X > leftBorder09 && e.X < rightBorder09 && e.Y > topBorder09 && e.Y < bottomBorder09)
            //{
            //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - pt.X, e.Y - pt.Y, e.Delta);
            //    hSmartWindowControl9.HSmartWindowControl_MouseWheel(sender, newe);
            //}



        }







        //*********************************************************************************
        //                                                                                *
        //              1、原比例显示图形  （原点在0，0，原比例，不变形填充图框）         *
        //                            Pubilc_Image_Display                                *
        //                                                                                *
        //   输入：  Image_Display   要显示的图形名称          HObject                    *
        //           DisWinHandle    HWindowXCtrl窗口指针      HTuple                     *
        //   注意：  Win_Width / Win_Height-1 运算时不要加括弧，否则取整运算误差很大      *
        //                                                                                *
        //*********************************************************************************
        public void Pubilc_Image_Display(HObject Image_Display, HTuple DisWinHandle)
        {
            //-----------变量定义-----------------------------------------------------------
            HTuple Image_Width;
            HTuple Image_Height;

            HTuple Win_Row;
            HTuple Win_Col;
            HTuple Win_Width;
            HTuple Win_Height;

            //----------变量初始化----------------------------------------------------------
            HOperatorSet.GetWindowExtents(DisWinHandle, out Win_Row, out Win_Col, out Win_Width, out Win_Height);
            HOperatorSet.GetImageSize(Image_Display, out Image_Width, out Image_Height);

            //-----------程序区-------------------------------------------------------------
            try
            {
                //----假如窗口是横向长方形，则把窗口高度设置成图像宽度，原图像扩大到与窗口比例相同-------
                if ((Win_Width / Win_Height) >= (Image_Width / Image_Height))
                {
                    HOperatorSet.SetPart(DisWinHandle, 0, 0, Image_Height - 1, Image_Height * Win_Width / Win_Height - 1);        //注意;Win_Width / Win_Height不要加括弧，否则取整运算误差很大
                }

                //----假如窗口是竖向长方形，则把窗口宽度设置成图像宽度，原图像扩大到与窗口比例相同--------
                if ((Win_Width / Win_Height) < (Image_Width / Image_Height))
                {
                    HOperatorSet.SetPart(DisWinHandle, 0, 0, Image_Width * Win_Height / Win_Width, Image_Width - 1);            //注意;Win_Width / Win_Height不要加括弧，否则取整运算误差很大  
                }

                //-------显示----------------------------------------------------------------
                HOperatorSet.ClearWindow(DisWinHandle);
                HOperatorSet.DispObj(Image_Display, DisWinHandle);
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        //************************************
        //*         报告错误消息             *
        //************************************
        private void ReportError(string message, bool lockdown)
        {
            int errc;
            lock (ErrLock) { errc = ++ErrCount; }
            if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lock (ErrLock) { ErrCount--; }

        }



        #endregion


    }
}
