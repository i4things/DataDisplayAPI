/**
 * Copyright (c) 2018-2019 i4things All Rights Reserved.
 * 
 * This SOURCE CODE FILE, which has been provided by i4things as part
 * of an B2N Ltd. product for use ONLY by licensed users of the product,
 * includes CONFIDENTIAL and PROPRIETARY information of i4things.
 * 
 * USE OF THIS SOFTWARE IS GOVERNED BY THE TERMS AND CONDITIONS
 * OF THE LICENSE STATEMENT AND LIMITED WARRANTY FURNISHED WITH
 * THE PRODUCT.
 * 
 * IN PARTICULAR, YOU WILL INDEMNIFY AND HOLD B2N LTD., ITS
 * RELATED COMPANIES AND ITS SUPPLIERS, HARMLESS FROM AND AGAINST ANY
 * CLAIMS OR LIABILITIES ARISING OUT OF THE USE, REPRODUCTION, OR
 * DISTRIBUTION OF YOUR PROGRAMS, INCLUDING ANY CLAIMS OR LIABILITIES
 * ARISING OUT OF OR RESULTING FROM THE USE, MODIFICATION, OR
 * DISTRIBUTION OF PROGRAMS OR FILES CREATED FROM, BASED ON, AND/OR
 * DERIVED FROM THIS SOURCE CODE FILE.
 *
 * @version 2.84
 */

using System;
using System.IO;
using System.Net;
using System.Text;

namespace i4things
{
    public class i4things
    {
#region PRIVATE
        /**********************************************************\
        |                                                          |                                             |
        |                                                          |
        | XXTEA encryption algorithm library for .NET.             |
        |                                                          |
        | Encryption Algorithm Authors:                            |
        |      David J. Wheeler                                    |
        |      Roger M. Needham                                    |
        |                                                          |
        | Code Author:  Ma Bingyao <mabingyao@gmail.com>           |
        | LastModified: Mar 10, 2015                               |
        | Part modified by i4things                                | 
        |                                                          |
        \**********************************************************/

        private static Byte[] XXTEAEncrypt(Byte[] data, Byte[] key)
        {
            if (data.Length == 0)
            {
                return data;
            }
            return ToByteArray(Encrypt(ToUInt32ArraySize(data), ToUInt32Array(key)));
        }

        private static Byte[] XXTEADecrypt(Byte[] data, Byte[] key)
        {
            if (data.Length == 0)
            {
                return data;
            }
            return ToByteArraySize(Decrypt(ToUInt32Array(data), ToUInt32Array(key)));
        }



        private static UInt32[] ToUInt32ArraySize(Byte[] data)
        {
            Byte[] dataSize = new Byte[data.Length + 1];
            dataSize[0] = (Byte)data.Length;                                // set the prepended value
            Array.Copy(data, 0, dataSize, 1, data.Length);

            return ToUInt32Array(dataSize);
        }

        private static UInt32[] ToUInt32Array(Byte[] data)
        {
            Int32 length = data.Length;
            Int32 n = (((length & 3) == 0) ? (length >> 2) : ((length >> 2) + 1));
            UInt32[] result = new UInt32[n];
            for (Int32 i = 0; i < length; i++)
            {
                result[i >> 2] |= (UInt32)data[i] << ((i & 3) << 3);
            }
            return result;
        }

        private static Byte[] ToByteArray(UInt32[] data)
        {
            Int32 n = data.Length << 2;
            Byte[] result = new Byte[n];
            for (Int32 i = 0; i < n; i++)
            {
                result[i] = (Byte)(data[i >> 2] >> ((i & 3) << 3));
            }
            return result;
        }

        private static Byte[] ToByteArraySize(UInt32[] data)
        {
            Byte[] dataSize = ToByteArray(data);
            Byte[] result = new Byte[dataSize[0]];
            Array.Copy(dataSize, 1, result, 0, result.Length);
            return result;
        }

        private const UInt32 delta = 0x9E3779B9;

        private static UInt32 MX(UInt32 sum, UInt32 y, UInt32 z, Int32 p, UInt32 e, UInt32[] k)
        {
            return (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
        }

        private static UInt32[] Encrypt(UInt32[] v, UInt32[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1)
            {
                return v;
            }
            UInt32 z = v[n], y, sum = 0, e;
            Int32 p, q = 6 + 52 / (n + 1);
            unchecked
            {
                while (0 < q--)
                {
                    sum += delta;
                    e = sum >> 2 & 3;
                    for (p = 0; p < n; p++)
                    {
                        y = v[p + 1];
                        z = v[p] += MX(sum, y, z, p, e, k);
                    }
                    y = v[0];
                    z = v[n] += MX(sum, y, z, p, e, k);
                }
            }
            return v;
        }

        private static UInt32[] Decrypt(UInt32[] v, UInt32[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1)
            {
                return v;
            }
            UInt32 z, y = v[0], sum, e;
            Int32 p, q = 6 + 52 / (n + 1);
            unchecked
            {
                sum = (UInt32)(q * delta);
                while (sum != 0)
                {
                    e = sum >> 2 & 3;
                    for (p = n; p > 0; p--)
                    {
                        z = v[p - 1];
                        y = v[p] -= MX(sum, y, z, p, e, k);
                    }
                    z = v[n];
                    y = v[0] -= MX(sum, y, z, p, e, k);
                    sum -= delta;
                }
            }
            return v;
        }
        /**********************************************************\
                XXTEA END
        \**********************************************************/

        private static String ToHEX(Byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (Byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private static Byte[] FromHEX(String hex)
        {
            int NumberChars = hex.Length;
            Byte[] bytes = new Byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private static Byte[] ToByteArray(UInt64 l)
        {
            Byte[] ba = { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < ba.Length; i++)
            {
                Byte b = (Byte)(l & 0xFF);
                ba[i] = b;
                l = (l - b) / 256;
            }

            return ba;
        }

        private static UInt64 ToLong(Byte[] ba)
        {
            UInt64 v = 0;
            for (int i = ba.Length - 1; i >= 0; i--)
            {
                v = (v * 256) + ba[i];
            }

            return v;
        }

        private static UInt32 CRC4(Byte[] c)
        {
            UInt32 crc = 0;
            for (int i = 0; i < c.Length; i++)
            {
                Byte b = (Byte)(c[i] & 0xFF);
                crc = (crc << 1) ^ b;
                crc = crc & 0xFFFFFFFF;
            };
            return crc;
        }

        private static Byte CRC(Byte[] c)
        {
            UInt32 crc = 8606;
            for (int i = 0; i < c.Length; i++)
            {
                Byte b = (Byte)(c[i] & 0xFF);
                crc = (crc << 1) ^ b;
                crc = crc & 0xFFFFFFFF;
            };
            return (Byte)(crc & 0xFF);
        }

        private static readonly UInt64 epoch = (UInt64)new DateTime(1970, 1, 1).Ticks;

        private static Byte[] Challenge(String networkKey)
        {
            UInt64 t = (((UInt64)(new DateTime().ToUniversalTime().Ticks)) - epoch) / ((UInt64)TimeSpan.TicksPerMillisecond);
            Byte[] c = ToByteArray(t);
            // gen CRC
            UInt32 crc = CRC4(c);
            // and in front of challenge
            Byte[] cRet = new Byte[c.Length + 4];
            cRet[0] = (Byte)(crc & 0xFF);
            cRet[1] = (Byte)(crc >> 8 & 0xFF);
            cRet[2] = (Byte)(crc >> 16 & 0xFF);
            cRet[3] = (Byte)(crc >> 24 & 0xFF);
            Array.Copy(c, 0, cRet, 4, c.Length);
            // encrypt
            return XXTEAEncrypt(cRet, FromHEX(networkKey));
        }

        // id integer. network_key in HEX format 32 chars
        private static String GetDataRequest(UInt64 id, String networkKey)
        {
            // gen challenge
            Byte[] c = Challenge(networkKey);
            StringBuilder sb = new StringBuilder();
            sb.Append(id.ToString()).Append('-').Append(ToHEX(c).ToUpper());
            return sb.ToString();
        }

        // id integer, data byte array, network_key in HEX format 32 chars, private_key 16 bytes array
        private static String SetDataRequest(UInt64 id, Byte[] data, String networkKey, Byte[] privateKey)
        {
            // gen challenge
            Byte[] c = Challenge(networkKey);
            Byte crc = CRC(data);
            Byte[] dataCRC = new Byte[data.Length + 1];
            dataCRC[0] = crc;
            Array.Copy(data, 0, dataCRC, 1, data.Length);
            StringBuilder sb = new StringBuilder();
            sb.Append(id.ToString()).Append('-').Append(ToHEX(c).ToUpper()).Append('-').Append(ToHEX(XXTEAEncrypt(dataCRC, privateKey)).ToUpper());
            return sb.ToString();
        }

        // id integer, hist day index integer, network_key in HEX format 32 chars
        private static String GetDataHistRequest(UInt64 id, Byte dayIdx, String networkKey)
        {
            // gen challenge
            Byte[] c = Challenge(networkKey);
            StringBuilder sb = new StringBuilder();
            sb.Append(id.ToString()).Append('-').Append(dayIdx.ToString()).Append('-').Append(ToHEX(c).ToUpper());
            return sb.ToString();
        }

        // id integer, from timestamp long, network_key in HEX format 32 chars
        private static String GetDataFromRequest(UInt64 id, UInt64 timestamp, String networkKey)
        {
            // gen challenge
            Byte[] c = Challenge(networkKey);
            StringBuilder sb = new StringBuilder();
            sb.Append(id.ToString()).Append('-').Append(timestamp.ToString()).Append('-').Append(ToHEX(c).ToUpper());
            return sb.ToString();
        }

        private static int resultStart = 16;
        private static int resultEnd = 2;
        private static String server = "http://server.i4things.com:5408/";
#endregion
        /**********************************************************\
                      PUBLIC
        \**********************************************************/
#region PUBLIC
        /// <summary>
        /// Use to decrypt received data
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="privateKey">Private key - byte array 16 elements</param>
        /// <returns>Decrypted data</returns>
        public static Byte[] Decrypt(Byte[] data, Byte[] privateKey)
        {
            return XXTEADecrypt(data, privateKey);
        }

        /// <summary>
        /// Use to request/receive all data for the current day from the server
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="networkKey">Network key (HEX format 32 chars)</param>
        /// <returns></returns>
        public static String GetData(long id, String networkKey)
        {
            StringBuilder uri = new StringBuilder(server);
            uri.Append("iot_get/").Append(GetDataRequest((UInt64)id,networkKey));
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.ToString());
            request.Timeout = 300000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String r =  reader.ReadToEnd().Trim();
                return r.Substring(resultStart, r.Length - resultStart - resultEnd);
            }
        }

        /// <summary>
        /// Use to request/receive only last data received from the node
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="networkKey">Network key (HEX format 32 chars)</param>
        /// <returns></returns>
        public static String GetLast(long id, String networkKey)
        {
            StringBuilder uri = new StringBuilder(server);
            uri.Append("iot_get_last/").Append(GetDataRequest((UInt64)id, networkKey));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.ToString());
            request.Timeout = 300000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String r = reader.ReadToEnd().Trim();
                return r.Substring(resultStart, r.Length - resultStart - resultEnd);
            }
        }

        /// <summary>
        /// Use to request/receive history data for specific day
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="day">Day for which the data is requested - 0 yesterday, 1 - the day before yesterday etc.</param>
        /// <param name="networkKey">Network key (HEX format 32 chars)</param>
        /// <returns></returns>
        public static String GetHist(long id, int day, String networkKey)
        {
            StringBuilder uri = new StringBuilder(server);
            uri.Append("iot_get_hist/").Append(GetDataHistRequest((UInt64)id, (Byte)day, networkKey));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.ToString());
            request.Timeout = 300000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String r = reader.ReadToEnd().Trim();
                return r.Substring(resultStart, r.Length - resultStart - resultEnd);
            }
        }

        /// <summary>
        /// Use to request/receive all data after a timestamp from the current day from the server
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="from">timetsamp in millis after 1,1,1970 in UTC</param>
        /// <param name="networkKey">Network key (HEX format 32 chars)</param>
        /// <returns></returns>
        public static String GetFrom(long id, long from, String networkKey)
        {
            StringBuilder uri = new StringBuilder(server);
            uri.Append("iot_get_from/").Append(GetDataFromRequest((UInt64)id, (UInt64)from, networkKey));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.ToString());
            request.Timeout = 300000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String r = reader.ReadToEnd().Trim();
                return r.Substring(resultStart, r.Length - resultStart - resultEnd);
            }
        }

        /// <summary>
        /// Use to send data to node
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="data">Data to be sent - byte array - the data will be signed and encrypted and checked at the node automatically</param>
        /// <param name="networkKey">Network key (HEX format 32 chars)</param>
        /// <param name="privateKey">Private key - byte array 16 elements</param>
        /// 
        public static String SetData(long id, Byte[] data, String networkKey, Byte[] privateKey)
        {
            StringBuilder uri = new StringBuilder(server);
            uri.Append("iot_set/").Append(SetDataRequest((UInt64)id, data, networkKey, privateKey));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.ToString());
            request.Timeout = 300000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String r = reader.ReadToEnd().Trim();
                if (r.IndexOf("OK") >= 0)
                {
                    return null;
                }
                else
                {
                    return r.Substring(resultStart, r.Length - resultStart - resultEnd);
                }
            }
        }
#endregion
    }
}
