using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SubDownloader
{
    class ShooterUtil
    {
        public static string CaculateFileHash(string filePath)
        {
            string hashString = "";
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            long fileLength = file.Length;
            long[] offset = new long[4];
            if (fileLength < 8192)
            {
                //a video file less then 8k? impossible!

            }
            else
            {
                const int BlockSize = 4096;
                const int NumOfSegments = 4;

                offset[3] = fileLength - 8192;
                offset[2] = fileLength / 3;
                offset[1] = fileLength / 3 * 2;
                offset[0] = BlockSize;

                MD5 md5 = new MD5CryptoServiceProvider();

                BinaryReader reader = new BinaryReader(file);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < NumOfSegments; i++)
                {
                    file.Seek(offset[i], SeekOrigin.Begin);
                    byte[] dataBlock = reader.ReadBytes(BlockSize);
                    MD5 md5Crypt = new MD5CryptoServiceProvider();
                    byte[] hash = md5Crypt.ComputeHash(dataBlock);
                    if (sb.Length > 0)
                    {
                        sb.Append(';');
                    }
                    foreach (byte a in hash)
                    {
                        if (a < 16)
                            sb.AppendFormat("0{0}", a.ToString("x"));
                        else
                            sb.Append(a.ToString("x"));
                    }
                }

                reader.Close();
                hashString = sb.ToString();
            }

            return hashString;
        }
    }
}
