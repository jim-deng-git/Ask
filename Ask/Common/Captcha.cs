using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Drawing;

namespace WorkV3.Common
{    
    public class Captcha
    {
        private static string sessionKey = "Captcha";
        public string Value { get; set; }
        public string Code { get; set; }
        public bool Validate() {
            DateTime now = DateTime.Now;
            if (MD5(now.ToString("yyyyMMddHH") + ":" + Value.ToLower()) == Code)
                return true;

            if (MD5(now.AddHours(-1).ToString("yyyyMMddHH") + ":" + Value.ToLower()) == Code)
                return true;

            return false;
        }

        public Bitmap CreateImage() {
            int randAngle = 45; //随机转动角度
            int mapwidth = Value.Length * 24;
            int mapheight = 28;

            Bitmap map = new Bitmap(mapwidth, mapheight);//创建图片背景
            Graphics graph = Graphics.FromImage(map);
            graph.Clear(Color.AliceBlue);//清除画面，填充背景
            graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
            //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//模式

            Random rand = new Random();

            // 画干扰线
            for (int i = 0; i != 15; ++i) {
                Point start = new Point(rand.Next(mapwidth), rand.Next(mapheight));
                Point end = new Point(rand.Next(mapwidth), rand.Next(mapheight));
                graph.DrawLine(new Pen(Color.FromArgb(rand.Next()), 2), start, end);
            }

            //背景噪点生成
            Pen blackPen = new Pen(Color.LightGray, 0);
            for (int i = 0; i < 50; i++) {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }
            
            string val = string.Empty;
            for (int i = 0, len = Value.Length; i < len; ++i)
                val += Value[i].ToString() + " ";

            //验证码旋转，防止机器识别
            char[] chars = val.Trim().ToCharArray();//拆散字符串成单字符数组

            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //定义字体
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋體" };

            for (int i = 0; i < chars.Length; i++) {
                int cindex = rand.Next(7);
                int findex = rand.Next(5);

                Font f = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)
                Brush b = new System.Drawing.SolidBrush(c[cindex]);

                Point dot = new Point(10, 10); // 文字间距
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                float angle = rand.Next(-randAngle, randAngle);//转动的度数

                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                graph.RotateTransform(-angle);//转回去
                graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置
            }
            //graph.DrawString(Value,fontstyle,new SolidBrush(Color.Blue),2,2); //标准随机码

            return map;
        }

        public static Captcha Create(int length = 4, bool IsJustNum = false) {
            string charList = IsJustNum ? "123456789" : "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
            string value = string.Empty;
            Random rand = new Random();            
            for (int i = 0; i < length; ++i)
                value += charList[rand.Next(charList.Length)];

            Captcha captcha = new Captcha {
                Value = value,
                Code = MD5(DateTime.Now.ToString("yyyyMMddHH") + ":" + value.ToLower())
            };

            HttpContext.Current.Session[sessionKey] = captcha;
            return captcha;
        }

        public static Captcha Current {
            get {
                Captcha captcha = HttpContext.Current.Session[sessionKey] as Captcha;
                HttpContext.Current.Session.Remove(sessionKey);
                return captcha;
            }
        } 

        private static string MD5(string text) {
            text += ":C@W0rk";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(text);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string result = string.Empty;
            for (int i = 0; i < md5data.Length - 1; i++) {
                result += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return result;
        }
    }
}