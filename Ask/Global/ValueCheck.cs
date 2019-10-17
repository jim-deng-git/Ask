using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Global
{
    public class ValueCheck
    {
        public static bool IsTwIDCard(string idCard)
        {
            if (idCard.Trim() != "")
            {
                string vid = idCard.Trim();
                string[] FirstEng = "A, B, C, D, E, F, G, H, J, K, L, M, N, P, Q, R, S, T, U, V, X, Y, W, Z, I, O".Split(','); //英文字母順序不可變動
                string aa = vid.ToUpper();
                bool chackFirstEnd = false;
                if (aa.Trim().Length == 10)
                {
                    try
                    {
                        byte firstNo = Convert.ToByte(aa.Trim().Substring(1, 1));
                        if (firstNo > 2 || firstNo < 1)
                        {
                            return false;
                        }
                        else
                        {
                            int x; for (x = 0; x < FirstEng.Length; x++)
                            {
                                if (aa.Substring(0, 1) == FirstEng[x].Trim())
                                {
                                    aa = string.Format("{0}{1}", x + 10, aa.Substring(1, 9));
                                    chackFirstEnd = true;
                                    break;
                                }
                            }
                            if (!chackFirstEnd)
                            {
                                return false; //return "3";
                            }
                            int i = 1;
                            int ss = int.Parse(aa.Substring(0, 1));
                            while (aa.Length > i)
                            {
                                ss = ss + (int.Parse(aa.Substring(i, 1)) * (10 - i)); i++;
                            }
                            aa = ss.ToString();
                            if (vid.Substring(9, 1) == "0")
                            {
                                if (aa.Substring(aa.Length - 1, 1) == "0")
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                string temp = vid.Substring(9, 1);
                                string temp2 = (10 - int.Parse(aa.Substring(aa.Length - 1, 1))).ToString();
                                string temp3 = aa.Substring(aa.Length - 1, 1);
                                if (vid.Substring(9, 1) == (10 - int.Parse(aa.Substring(aa.Length - 1, 1))).ToString())
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsTwMobile(string mobile)
        {
            if (mobile.Length != 10)
                return false;
            if (!mobile.StartsWith("09"))
                return false;
            for (int i = 0; i < mobile.Length; i++)
            {
                if (!WorkLib.uCheck.IsNumeric(mobile[i]))
                    return false;
            }
            return true;
            //System.Text.RegularExpressions.Regex  regex = new  System.Text.RegularExpressions.Regex(@"/^09\d{8}$/");
            //return regex.IsMatch(mobile.Trim());
        }
        public static bool IsTwPhone(string phone)
        {

            if (phone.Length <8 || phone.Length >10)
                return false;
            for (int i = 0; i < phone.Length; i++)
            {
                if (!WorkLib.uCheck.IsNumeric(phone[i]))
                    return false;
            }
            int areaCode = int.Parse(phone.Substring(0, 2));
            if(areaCode<2 || areaCode > 9)
                return false;
            return true;
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"/^0[2-9]\d{7,8}$/");
            //return regex.IsMatch(phone.Trim());
        }
    }
}