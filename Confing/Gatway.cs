﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Confing
{
    public class Gatway
    {
        
        /// <summary>
        /// 将设置项记录到本地
        /// </summary>
        /// <param name="form"></param>
        public static void Record(System.Windows.Forms.Form form)
        {
            Helper.XML.Write(form);
        }
        /// <summary>
        /// 将设置项重现到配置界面中
        /// </summary>
        /// <param name="form"></param>
        public static void Restore(System.Windows.Forms.Form form)
        {
            Helper.XML.Read(form);
        }
        #region 获取参数
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="itemname"></param>
        /// <returns></returns>
        public static string Get(string itemname)
        {
            string val = Helper.XML.Read(itemname);
            return val.Trim();
        }
        /// <summary>
        /// 通过base64转换成图片，图片格式为png
        /// </summary>
        /// <param name="itemname"></param>
        /// <returns></returns>
        public static Image GetImage(string itemname)
        {
            string val= Get(itemname);
            Image image = Helper.XML.FromBase64(val);
            return image;
        }
        /// <summary>
        /// 通过base64转换成图片
        /// </summary>
        /// <param name="itemname">配置项名称</param>
        /// <param name="format">图片格式 支持 jpg|bmp|gif|png</param>
        /// <returns></returns>
        public static Image GetImage(string itemname,string format)
        {
            string base64string = Get(itemname);
            //临时文件
            string tmFile = string.Format("{0}/{1}.{2}", Environment.CurrentDirectory, itemname, format);
            try
            {
                byte[] arr = Convert.FromBase64String(base64string);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                if (format.ToLower() == "jpg") bmp.Save(tmFile, ImageFormat.Jpeg);
                if (format.ToLower() == "bmp") bmp.Save(tmFile, ImageFormat.Bmp);
                if (format.ToLower() == "gif") bmp.Save(tmFile, ImageFormat.Gif);
                if (format.ToLower() == "png") bmp.Save(tmFile, ImageFormat.Png);
                return Image.FromFile(tmFile);
            }
            catch
            {
                return null;
            }
        }
        public static Icon GetIcon(string itemname)
        {
            Image image = GetImage(itemname);
            if (image == null) return null;
            using (MemoryStream msImg = new MemoryStream(), msIco = new MemoryStream())
            {
                image.Save(msImg, ImageFormat.Png);
                using (var bin = new BinaryWriter(msIco))
                {
                    //写图标头部
                    bin.Write((short)0);           //0-1保留
                    bin.Write((short)1);           //2-3文件类型。1=图标, 2=光标
                    bin.Write((short)1);           //4-5图像数量（图标可以包含多个图像）
                    bin.Write((byte)image.Width);  //6图标宽度
                    bin.Write((byte)image.Height); //7图标高度
                    bin.Write((byte)0);            //8颜色数（若像素位深>=8，填0。这是显然的，达到8bpp的颜色数最少是256，byte不够表示）
                    bin.Write((byte)0);            //9保留。必须为0
                    bin.Write((short)0);           //10-11调色板
                    bin.Write((short)32);          //12-13位深
                    bin.Write((int)msImg.Length);  //14-17位图数据大小
                    bin.Write(22);                 //18-21位图数据起始字节
                    //写图像数据
                    bin.Write(msImg.ToArray());
                    bin.Flush();
                    bin.Seek(0, SeekOrigin.Begin);
                    return new Icon(msIco);
                }
            }
        }
        public static bool GetBoolean(string itemname)
        {
            bool tm = false;
            string val = Helper.XML.Read(itemname);
            bool.TryParse(val, out tm);
            return tm;
        }
        public static int GetInt(string itemname)
        {
            int tm = 0;
            string val = Helper.XML.Read(itemname);
            int.TryParse(val, out tm);
            return tm;
        }
        #endregion
    }
}
