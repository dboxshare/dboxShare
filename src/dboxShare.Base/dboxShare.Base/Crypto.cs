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
        public static void FileEncrypt(string InputFilePath, string Key)
        {
            string OutputFilePath = "" + InputFilePath + ".encrypted";

            FileEncrypt(InputFilePath, OutputFilePath, Key);
        }


        public static void FileEncrypt(string InputFilePath, string OutputFilePath, string Key)
        {
            if (File.Exists(InputFilePath) == false)
            {
                return;
            }

            byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

            try
            {
                using (FileStream InputStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (FileStream OutputStream = File.Create(OutputFilePath, 4096, FileOptions.Asynchronous))
                    {
                        string FileMD5 = FileHash(InputFilePath);
                        byte[] FileMD5Bytes = Encoding.UTF8.GetBytes(FileMD5);

                        KeyBytes = FileMD5Bytes.Concat(KeyBytes).ToArray();

                        OutputStream.Write(FileMD5Bytes, 0, (int)FileMD5Bytes.Length);

                        using (RijndaelManaged AES = new RijndaelManaged())
                        {
                            var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                            AES.Mode = CipherMode.CBC;
                            AES.KeySize = 256;
                            AES.BlockSize = 128;
                            AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                            AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                            using (CryptoStream CStream = new CryptoStream(OutputStream, AES.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                byte[] Buffer = new byte[65536];
                                int Read = 0;

                                while ((Read = InputStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                                {
                                    CStream.Write(Buffer, 0, Read);
                                }

                                CStream.FlushFinalBlock();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 文件解密
        /// </summary>
        public static void FileDecrypt(string InputFilePath, string Key)
        {
            string OutputFilePath = "" + InputFilePath + ".decrypted";

            FileDecrypt(InputFilePath, OutputFilePath, Key);
        }


        public static void FileDecrypt(string InputFilePath, string OutputFilePath, string Key)
        {
            if (File.Exists(InputFilePath) == false)
            {
                return;
            }

            byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));

            try
            {
                using (FileStream InputStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string FileMD5 = "";
                    byte[] FileMD5Bytes = new byte[32];

                    InputStream.Read(FileMD5Bytes, 0, (int)FileMD5Bytes.Length);

                    FileMD5 = Encoding.UTF8.GetString(FileMD5Bytes);

                    if (Common.StringCheck(FileMD5, @"^[\w]{32}$") == false)
                    {
                        File.Copy(InputFilePath, OutputFilePath, true);
                        return;
                    }

                    KeyBytes = FileMD5Bytes.Concat(KeyBytes).ToArray();

                    using (FileStream OutputStream = File.Create(OutputFilePath, 4096, FileOptions.Asynchronous))
                    {
                        using (RijndaelManaged AES = new RijndaelManaged())
                        {
                            var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                            AES.Mode = CipherMode.CBC;
                            AES.KeySize = 256;
                            AES.BlockSize = 128;
                            AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                            AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                            using (CryptoStream CStream = new CryptoStream(OutputStream, AES.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                byte[] Buffer = new byte[65536];
                                int Read = 0;

                                while ((Read = InputStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                                {
                                    CStream.Write(Buffer, 0, Read);
                                }

                                CStream.FlushFinalBlock();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
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

            byte[] TextBytes = Encoding.UTF8.GetBytes(Text);
            byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
            byte[] ResultBytes = { };

            try
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                    AES.Mode = CipherMode.CBC;
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                    AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                    using (MemoryStream MStream = new MemoryStream())
                    {
                        using (CryptoStream CStream = new CryptoStream(MStream, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            CStream.Write(TextBytes, 0, TextBytes.Length);
                            CStream.FlushFinalBlock();
                        }

                        ResultBytes = MStream.ToArray();
                    }
                }
            }
            catch (Exception)
            {

            }

            return System.Convert.ToBase64String(ResultBytes);
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

            byte[] TextBytes = Encoding.UTF8.GetBytes(Text);
            byte[] KeyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
            byte[] ResultBytes = { };

            try
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var RFC = new Rfc2898DeriveBytes(KeyBytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000);

                    AES.Mode = CipherMode.CBC;
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = RFC.GetBytes((AES.KeySize / 8).TypeInt());
                    AES.IV = RFC.GetBytes((AES.BlockSize / 8).TypeInt());

                    using (MemoryStream MStream = new MemoryStream())
                    {
                        using (CryptoStream CStream = new CryptoStream(MStream, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            CStream.Write(TextBytes, 0, TextBytes.Length);
                            CStream.FlushFinalBlock();
                        }

                        ResultBytes = MStream.ToArray();
                    }
                }
            }
            catch (Exception)
            {

            }

            return System.Convert.ToBase64String(ResultBytes);
        }


        /// <summary>
        /// 获取文件哈希码
        /// </summary>
        public static string FileHash(string FilePath)
        {
            string Hash = "";

            using (FileStream FStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
                {
                    byte[] Bytes = MD5.ComputeHash(FStream);

                    for (int i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }

            return Hash;
        }


    }


}
