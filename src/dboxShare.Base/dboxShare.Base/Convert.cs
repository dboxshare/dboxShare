using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;



namespace dboxShare.Base
{


    public static class Convert
    {


        /// <summary>
        /// 对象转换为string类型
        /// </summary>
        public static string TypeString(this object Object)
        {
            try
            {
                return System.Convert.ToString(Object);
            }
            catch (Exception)
            {
                return default(string);
            }
        }


        /// <summary>
        /// 对象转换为int类型
        /// </summary>
        public static int TypeInt(this object Object)
        {
            try
            {
                return System.Convert.ToInt32(Object);
            }
            catch (Exception)
            {
                return default(int);
            }
        }


        /// <summary>
        /// 对象转换为long类型
        /// </summary>
        public static long TypeLong(this object Object)
        {
            try
            {
                return System.Convert.ToInt64(Object);
            }
            catch (Exception)
            {
                return default(long);
            }
        }


        /// <summary>
        /// 对象转换为bool类型
        /// </summary>
        public static bool TypeBool(this object Object)
        {
            try
            {
                return System.Convert.ToBoolean(Object);
            }
            catch (Exception)
            {
                return default(bool);
            }
        }


        /// <summary>
        /// 对象转换为byte类型
        /// </summary>
        public static byte TypeByte(this object Object)
        {
            try
            {
                return System.Convert.ToByte(Object);
            }
            catch (Exception)
            {
                return default(byte);
            }
        }


        /// <summary>
        /// 对象转换为char类型
        /// </summary>
        public static char TypeChar(this object Object)
        {
            try
            {
                return System.Convert.ToChar(Object);
            }
            catch (Exception)
            {
                return default(char);
            }
        }


        /// <summary>
        /// 对象转换为datetime类型
        /// </summary>
        public static DateTime TypeDateTime(this object Object)
        {
            try
            {
                return System.Convert.ToDateTime(Object);
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }


        /// <summary>
        /// 对象转换为decimal类型
        /// </summary>
        public static decimal TypeDecimal(this object Object)
        {
            try
            {
                return System.Convert.ToDecimal(Object);
            }
            catch (Exception)
            {
                return default(decimal);
            }
        }


        /// <summary>
        /// 对象转换为double类型
        /// </summary>
        public static double TypeDouble(this object Object)
        {
            try
            {
                return System.Convert.ToDouble(Object);
            }
            catch (Exception)
            {
                return default(double);
            }
        }


        /// <summary>
        /// 对象转换为float类型
        /// </summary>
        public static float TypeFloat(this object Object)
        {
            try
            {
                return System.Convert.ToSingle(Object);
            }
            catch (Exception)
            {
                return default(float);
            }
        }


    }


}

