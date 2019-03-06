
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace CCShop.Common.Util
{
    public class PicUtil
    {
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="sm">文件流</param>
        /// <param name="dirpath">图片存放路径(D:/image)</param>
        /// <param name="prefix">图片文件名前缀(ps)</param>
        /// <param name="no">数据编号(180596)</param>
        /// <param name="extension">图片文件名后缀(.jpg)</param>
        /// <param name="sizes">缩略图尺寸(120_160,330_440)</param>
        /// <returns></returns>
        public static string UploadImage(Stream sm, string dirpath, string prefix, string no, string extension, string sizes = "")
        {
            try
            {
                if (sm == null)
                {
                    return "-1";
                }

                if (!IsInArray(extension, ".jpg") && !IsInArray(extension, ".png") && !IsInArray(extension, ".doc") && !IsInArray(extension, ".docx"))
                {
                    return "-2";
                }

                if (sm.Length / 1024 > 2048)
                {
                    return "-3";
                }

                string name = "";
                if (!string.IsNullOrEmpty(no))
                {
                    name = prefix + "_" + no + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                }
                else
                {
                    name = prefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                }

                string newFileName = name + extension;

                //原图存放路径
                string sourceDirPath = string.Format("{0}source/", dirpath);
                if (!Directory.Exists(sourceDirPath))
                {
                    Directory.CreateDirectory(sourceDirPath);
                }

                //保存原图
                string sourcePath = sourceDirPath + newFileName;

                try
                {
                    byte[] bytes = new byte[sm.Length]; //把Stream转换成 byte[]
                    sm.Read(bytes, 0, bytes.Length);
                    sm.Seek(0, SeekOrigin.Begin); //设置当前流的位置为流的开始  

                    FileStream fs = File.Create(sourcePath);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();
                }
                catch (Exception ex1)
                {
                    //Log.Err(ex1);
                    return "-4";
                }

                if (!string.IsNullOrEmpty(sizes))
                {
                    Image image = Image.FromFile(sourcePath);
                    int width = image.Width;
                    int height = image.Height;

                    string[] sizeList = sizes.Split(',');
                    foreach (string size in sizeList)
                    {
                        //缩略图存放路径
                        string thumbDirPath = string.Format("{0}thumb{1}/", dirpath, size);
                        if (!Directory.Exists(thumbDirPath))
                        {
                            Directory.CreateDirectory(thumbDirPath);
                        }
                        string thumbPath = thumbDirPath + newFileName;

                        string[] thumbSize = size.Split('_');
                        int toWidth = Convert.ToInt32(thumbSize[0]);
                        int toHeight = Convert.ToInt32(thumbSize[1]);

                        //保存缩略图
                        try
                        {
                            if (width > toWidth && height > toHeight)
                            {
                                GenerateThumb(sourcePath, thumbPath, toWidth, toHeight, "Cut");
                            }
                            else
                            {
                                MakeThumbnail(sourcePath, thumbPath, toWidth, toHeight);
                            }
                        }
                        catch (Exception ex2)
                        {
                            //Log.Err(ex2);
                            return "-4";
                        }
                    }
                }

                return newFileName;
            }
            catch (Exception ex)
            {
                //Log.Err(ex);
                return "-4";
            }
        }


        public static string UploadImage(Stream sm, string dirpath, string filename, string extension, out string msg, string sizes = "")
        {
            try
            {
                if (sm == null)
                {
                    msg = "sm 为空";

                    return "-1";
                }

                if (!IsInArray(extension, ".jpg") && !IsInArray(extension, ".png") && !IsInArray(extension, ".doc") && !IsInArray(extension, ".docx"))
                {
                    msg = "文件后缀名不正确";
                    return "-2";
                }

                if (sm.Length / 1024 > 2048)
                {
                    msg = "文件超出了2048默认大小";
                    return "-3";
                }

                string name = "";
                if (string.IsNullOrEmpty(filename))
                {
                    name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                }
                else
                {
                    name = filename;
                }

                string newFileName = name + extension;

                //原图存放路径
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

                //保存原图
                string sourcePath = dirpath + newFileName;

                try
                {
                    byte[] bytes = new byte[sm.Length]; //把Stream转换成 byte[]
                    sm.Read(bytes, 0, bytes.Length);
                    sm.Seek(0, SeekOrigin.Begin); //设置当前流的位置为流的开始  

                    FileStream fs = File.Create(sourcePath);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();
                }
                catch (Exception ex1)
                {
                    msg = ex1.ToString();
                    return "-4";
                }

                if (!string.IsNullOrEmpty(sizes))
                {
                    Image image = Image.FromFile(sourcePath);
                    int width = image.Width;
                    int height = image.Height;

                    string[] sizeList = sizes.Split(',');
                    foreach (string size in sizeList)
                    {
                        //缩略图存放路径
                        string thumbDirPath = dirpath;
                        if (!Directory.Exists(thumbDirPath))
                        {
                            Directory.CreateDirectory(thumbDirPath);
                        }
                        string thumbPath = thumbDirPath + name + "_thumb" + extension;

                        string[] thumbSize = size.Split('_');
                        int toWidth = Convert.ToInt32(thumbSize[0]);
                        int toHeight = Convert.ToInt32(thumbSize[1]);

                        //保存缩略图
                        try
                        {
                            if (width > toWidth && height > toHeight)
                            {
                                GenerateThumb(sourcePath, thumbPath, toWidth, toHeight, "Cut");
                            }
                            else
                            {
                                MakeThumbnail(sourcePath, thumbPath, toWidth, toHeight);
                            }
                        }
                        catch (Exception ex2)
                        {
                            msg = ex2.ToString();
                            return "-4";
                        }
                    }
                }

                msg = "上传成功";

                return newFileName;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();

                return "-4";
            }
        }


        private static void GenerateThumb(string imagePath, string thumbPath, int width, int height, string mode)
        {
            Image image = Image.FromFile(imagePath);

            string extension = imagePath.Substring(imagePath.LastIndexOf(".")).ToLower();
            ImageFormat imageFormat = null;
            switch (extension)
            {
                case ".jpg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    imageFormat = ImageFormat.Png;
                    break;
                case ".gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            int toWidth = width > 0 ? width : image.Width;
            int toHeight = height > 0 ? height : image.Height;

            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）
                    toHeight = toWidth;
                    break;
                case "W"://指定宽，高按比例             
                    toHeight = image.Height * width / image.Width;
                    break;
                case "H"://指定高，宽按比例
                    toWidth = image.Width * height / image.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）           
                    if ((double)image.Width / (double)image.Height > (double)toWidth / (double)toHeight)
                    {
                        oh = image.Height;
                        ow = image.Height * toWidth / toHeight;
                        y = 0;
                        x = (image.Width - ow) / 2;
                    }
                    else
                    {
                        ow = image.Width;
                        oh = image.Width * height / toWidth;
                        x = 0;
                        y = (image.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp
            Image bitmap = new Bitmap(toWidth, toHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(ColorTranslator.FromHtml("#FFFFFF"));

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(image,
                        new Rectangle(0, 0, toWidth, toHeight),
                        new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);

            try
            {
                bitmap.Save(thumbPath, imageFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
                if (image != null)
                    image.Dispose();
            }
        }

        private static void MakeThumbnail(string imagePath, string thumbPath, int width, int height)
        {
            //获取原始图片  
            Image originalImage = Image.FromFile(imagePath);
            //缩略图画布宽高  
            int towidth = width;
            int toheight = height;
            //原始图片写入画布坐标和宽高(用来设置裁减溢出部分)  
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            //原始图片画布,设置写入缩略图画布坐标和宽高(用来原始图片整体宽高缩放)  
            int bg_x = 0;
            int bg_y = 0;
            int bg_w = towidth;
            int bg_h = toheight;
            //倍数变量  
            double multiple = 0;
            //获取宽长的或是高长与缩略图的倍数  
            if (originalImage.Width >= originalImage.Height)
            {
                multiple = (double)originalImage.Width / (double)width;
            }
            else
            {
                multiple = (double)originalImage.Height / (double)height;
            }

            //上传的图片的宽和高小等于缩略图  
            if (ow <= width && oh <= height)
            {
                //缩略图按原始宽高  
                bg_w = originalImage.Width;
                bg_h = originalImage.Height;
                //空白部分用背景色填充  
                bg_x = Convert.ToInt32(((double)towidth - (double)ow) / 2);
                bg_y = Convert.ToInt32(((double)toheight - (double)oh) / 2);
            }
            else //上传的图片的宽和高大于缩略图  
            {
                //宽高按比例缩放  
                bg_w = Convert.ToInt32((double)originalImage.Width / multiple);
                bg_h = Convert.ToInt32((double)originalImage.Height / multiple);
                //空白部分用背景色填充  
                bg_y = Convert.ToInt32(((double)height - (double)bg_h) / 2);
                bg_x = Convert.ToInt32(((double)width - (double)bg_w) / 2);
            }

            //新建一个bmp图片,并设置缩略图大小.  
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板  
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法  
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并设置背景色  
            //g.Clear(Color.White);
            g.Clear(ColorTranslator.FromHtml("#FFFFFF"));

            //在指定位置并且按指定大小绘制原图片的指定部分  
            //第一个System.Drawing.Rectangle是原图片的画布坐标和宽高,第二个是原图片写在画布上的坐标和宽高,最后一个参数是指定数值单位为像素  
            g.DrawImage(originalImage, new Rectangle(bg_x, bg_y, bg_w, bg_h), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

            try
            {
                //获取图片类型  
                string fileExtension = Path.GetExtension(imagePath).ToLower();
                //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                switch (fileExtension)
                {
                    case ".gif": bitmap.Save(thumbPath, ImageFormat.Gif); break;
                    case ".jpg": bitmap.Save(thumbPath, ImageFormat.Jpeg); break;
                    case ".jpeg": bitmap.Save(thumbPath, ImageFormat.Jpeg); break;
                    case ".bmp": bitmap.Save(thumbPath, ImageFormat.Bmp); break;
                    case ".png": bitmap.Save(thumbPath, ImageFormat.Png); break;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        /// <summary>
        /// 图片下载
        /// </summary>
        /// <param name="url">图片的读取路径(http://abc.jpg)</param>
        /// <returns></returns>
        public static MemoryStream DownLoadImage(string url)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();

                //Image image = Image.FromStream(stream);
                //image.Save(ms, ImageFormat.Png);

                //Image<Rgba32> image = Image.Load(stream);
                //image.SaveAsJpeg<Rgba32>(ms);
                //response.Dispose();

                return ms;
            }
        }

        /// <summary>
        /// 判断字符串是否在字符串中
        /// </summary>
        private static bool IsInArray(string s, string array)
        {
            return IsInArray(s, array.Split(','), false);
        }

        /// <summary>
        /// 判断字符串是否在字符串数组中
        /// </summary>
        private static bool IsInArray(string s, string[] array, bool ignoreCase)
        {
            return GetIndexInArray(s, array, ignoreCase) > -1;
        }

        /// <summary>
        /// 获得字符串在字符串数组中的位置
        /// </summary>
        private static int GetIndexInArray(string s, string[] array, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(s) || array == null || array.Length == 0)
                return -1;

            int index = 0;
            string temp = null;

            if (ignoreCase)
                s = s.ToLower();

            foreach (string item in array)
            {
                if (ignoreCase)
                    temp = item.ToLower();
                else
                    temp = item;

                if (s == temp)
                    return index;
                else
                    index++;
            }

            return -1;
        }

        /// <summary>
        /// 是否是图片文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool IsImgFileName(string fileName)
        {
            if (fileName.IndexOf(".") == -1)
                return false;

            string tempFileName = fileName.Trim().ToLower();
            string extension = tempFileName.Substring(tempFileName.LastIndexOf("."));
            return extension == ".png" || extension == ".bmp" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif";
        }
    }
}
