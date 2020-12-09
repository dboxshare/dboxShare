using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;



namespace dboxShare.Base
{


    public static class Crypto
    {


        /// <summary>
        /// 文件加密
        /// </summary>
        public static byte[] FileEncrypt(string FilePath, string Key, bool Backtrack, bool Displace, bool Strengthen)
        {
            byte[] FileByte = default(byte[]);

            if (string.IsNullOrEmpty(FilePath) == true || string.IsNullOrEmpty(Key) == true)
            {
                return FileByte;
            }

            if (File.Exists(FilePath) == false)
            {
                return FileByte;
            }

            try
            {
                byte[] FileBytes = File.ReadAllBytes(FilePath);
                byte[] EncryptBytes = {};

                if (Common.IsNothing(FileBytes) == true)
                {
                    return FileByte;
                }

                if (Strengthen == true)
                {
                    string FileMD5 = Crypto.MD5Code(FileBytes);
                    string KeyEncrypt = Crypto.TextEncrypt(FileMD5, Key);
                    byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(KeyEncrypt));

                    EncryptBytes = Crypto.AESEncrypt(ref FileBytes, KeyBytes);

                    EncryptBytes = Encoding.UTF8.GetBytes(FileMD5).Concat(EncryptBytes).ToArray();
                }
                else
                {
                    byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

                    EncryptBytes = Crypto.AESEncrypt(ref FileBytes, KeyBytes);
                }

                if (Backtrack == true)
                {
                    FileByte = EncryptBytes;
                }
                else
                {
                    if (Displace == false)
                    {
                        FilePath = "" + FilePath + ".encrypted";
                    }

                    File.WriteAllBytes(FilePath, EncryptBytes);
                }

                FileBytes = null;
                EncryptBytes = null;
            }
            catch (Exception)
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return FileByte;
        }


        /// <summary>
        /// 文件解密
        /// </summary>
        public static byte[] FileDecrypt(string FilePath, string Key, bool Backtrack, bool Displace, bool Strengthen)
        {
            byte[] FileByte = default(byte[]);

            if (string.IsNullOrEmpty(FilePath) == true || string.IsNullOrEmpty(Key) == true)
            {
                return FileByte;
            }

            if (File.Exists(FilePath) == false)
            {
                return FileByte;
            }

            try
            {
                byte[] FileBytes = File.ReadAllBytes(FilePath);
                byte[] DecryptBytes = {};

                if (Common.IsNothing(FileBytes) == true)
                {
                    return FileByte;
                }

                if (Strengthen == true)
                {
                    string FileMD5 = Encoding.UTF8.GetString(FileBytes.Skip(0).Take(32).ToArray());
                    string KeyDecrypt = Crypto.TextEncrypt(FileMD5, Key);
                    byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(KeyDecrypt));

                    byte[] StreamBytes = FileBytes.Skip(32).ToArray();
                    DecryptBytes = Crypto.AESDecrypt(ref StreamBytes, KeyBytes);
                }
                else
                {
                    byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

                    DecryptBytes = Crypto.AESDecrypt(ref FileBytes, KeyBytes);
                }

                if (Backtrack == true)
                {
                    FileByte = DecryptBytes;
                }
                else
                {
                    if (Displace == false)
                    {
                        FilePath = "" + FilePath + ".decrypted";
                    }

                    File.WriteAllBytes(FilePath, DecryptBytes);
                }

                FileBytes = null;
                DecryptBytes = null;
            }
            catch (Exception)
            {


            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return FileByte;
        }


        /// <summary>
        /// 文本加密
        /// </summary>
        public static string TextEncrypt(string Text, string Key)
        {
            if (string.IsNullOrEmpty(Text) == true || string.IsNullOrEmpty(Key) == true)
            {
                return "";
            }

            try
            {
                byte[] TextBytes = Encoding.UTF8.GetBytes(Text);
                byte[] EncryptBytes = {};

                byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

                EncryptBytes = Crypto.AESEncrypt(ref TextBytes, KeyBytes);

                return System.Convert.ToBase64String(EncryptBytes);
            }
            catch (Exception)
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return "";
        }


        /// <summary>
        /// 文本解密
        /// </summary>
        public static string TextDecrypt(string Text, string Key)
        {
            if (string.IsNullOrEmpty(Text) == true || string.IsNullOrEmpty(Key) == true)
            {
                return "";
            }

            try
            {
                byte[] TextBytes = System.Convert.FromBase64String(Text);
                byte[] DecryptBytes = {};

                byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

                DecryptBytes = Crypto.AESDecrypt(ref TextBytes, KeyBytes);

                return Encoding.UTF8.GetString(DecryptBytes);
            }
            catch (Exception)
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return "";
        }


        /// <summary>
        /// aes加密算法
        /// </summary>
        private static byte[] AESEncrypt(ref byte[] StreamBytes, byte[] KeyBytes)
        {
            byte[] EncryptBytes = null;

            try
            {
                MemoryStream MS = new MemoryStream();
                RijndaelManaged AES = new RijndaelManaged();
                var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                AES.Mode = CipherMode.CBC;
                AES.KeySize = 256;
                AES.BlockSize = 256;
                AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                var CS = new CryptoStream(MS, AES.CreateEncryptor(), CryptoStreamMode.Write);

                CS.Write(StreamBytes, 0, StreamBytes.Length);
                CS.FlushFinalBlock();
                CS.Close();

                EncryptBytes = MS.ToArray();

                StreamBytes = null;

                MS.Dispose();
                AES.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return EncryptBytes;
        }


        /// <summary>
        /// aes解密算法
        /// </summary>
        private static byte[] AESDecrypt(ref byte[] StreamBytes, byte[] KeyBytes)
        {
            byte[] DecryptBytes = null;

            try
            {
                MemoryStream MS = new MemoryStream();
                RijndaelManaged AES = new RijndaelManaged();
                var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                AES.Mode = CipherMode.CBC;
                AES.KeySize = 256;
                AES.BlockSize = 256;
                AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                var CS = new CryptoStream(MS, AES.CreateDecryptor(), CryptoStreamMode.Write);

                CS.Write(StreamBytes, 0, StreamBytes.Length);
                CS.FlushFinalBlock();
                CS.Close();

                DecryptBytes = MS.ToArray();

                StreamBytes = null;

                MS.Dispose();
                AES.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return DecryptBytes;
        }


        /// <summary>
        /// 获取md5代码
        /// </summary>
        private static string MD5Code(this byte[] Byte)
        {
            byte[] MD5 = new MD5CryptoServiceProvider().ComputeHash(Byte);
            string Hash = "";
            int Index = 0;

            for (Index = 0; Index < MD5.Length; Index++)
            {
                Hash += MD5[Index].ToString("X2");
            }

            return Hash;
        }


    }


}
