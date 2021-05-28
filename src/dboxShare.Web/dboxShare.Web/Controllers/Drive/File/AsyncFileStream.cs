using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class AsyncFileStream
    {


        private string FilePath = "";
        private bool DeleteCompleted = false;


        public AsyncFileStream(string _FilePath, bool _DeleteCompleted)
        {
            FilePath = _FilePath;
            DeleteCompleted = _DeleteCompleted;
        }


        /// <summary>
        /// 输出 HTTP 文件流
        /// </summary>
        public async void Output(Stream Stream, HttpContent Content, TransportContext Context)
        {
            if (File.Exists(FilePath) == false)
            {
                return;
            }

            try
            {
                using (var FStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new byte[65536];
                    var Read = 0;

                    while ((Read = FStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                    {
                        await Stream.WriteAsync(Buffer, 0, Read);
                    }
                }
            }
            finally
            {
                if (Base.Common.IsNothing(Stream) == false)
                {
                    Stream.Close();
                    Stream.Dispose();
                }

                if (DeleteCompleted == true)
                {
                    if (File.Exists(FilePath) == true)
                    {
                        File.Delete(FilePath);
                    }
                }
            }
        }


    }


}
