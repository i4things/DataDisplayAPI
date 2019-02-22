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
using System.Collections.Generic;
using System.Text;
using MarketHub.OTS;
using MarketHub.MD2;

namespace i4thingsStreaming
{
    public class i4thingsStreaming : IDisposable, IServiceListener, ISecurityListener, ISubscriptionListener
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

        private static Byte[] XXTEADecrypt(Byte[] data, Byte[] key)
        {
            if (data.Length == 0)
            {
                return data;
            }
            return ToByteArraySize(Decrypt(ToUInt32Array(data), ToUInt32Array(key)));
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

        private const String serviceName = "I4THINGS";
        private const String uriBaseString = "tcp://server.i4things.com:5407?machineId=";
        private const String dataRecordName = "Data";
        private readonly FeedAdapterFactory factory;
        private readonly SubscriptionFeedAdapter subAdapter;
        private readonly MDWithReconnection md;
        private readonly SubscriptionFeed feed;
        private readonly Service service;
        private readonly ConnectionToken connection;
        private readonly NewMessage callback;

        private readonly Dictionary<long, Security> securities = new Dictionary<long, Security>();
        
        private Boolean closed = false;

        private Record dataRecord = null;

        private static int resultStart = 16;
        private static int resultEnd = 2;

        //Implementation of ServiceListener
        public void NewStatus(Service service, Status oldStatus, Status newStatus)
        {
            // not interested
        }

        public void ServiceAttached(Service service, Database database, bool isAttached)
        {
            // not interested
        }


        //Implementation of SecurityListener
        public void Update(Security security, Message message, bool isImage)
        {
            if (dataRecord == null)
            {
                dataRecord = message.Database.GetRecord(dataRecordName);
            }

            String r = message[dataRecord].Value.String.Trim();
            r = r.Substring(resultStart, r.Length - resultStart - resultEnd);
            
            long nodeId = Convert.ToInt64(security.Name);

            callback(nodeId, r);
        }

        public void NewStatus(Security security, Status oldStatus, Status newStatus)
        {
            // not interested
        }

        public void SecurityDropped(Security security, DropType dropType, string reason)
        {
            // not interested
        }

        //Implementation of SubscriptionListener
        public IServiceListener NewService(Service service)
        {
            return this;
        }

        private void Close()
        {
            if (!closed)
            {
                closed = true;
                md.Stop();
                if (connection != null)
                {
                    subAdapter.Disconnect(connection);
                }
                subAdapter.Stop();
                MD2.Shutdown();
            }
        }

        ~i4thingsStreaming()
        {
            Close();
        }
#endregion

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
        /// Delegate to receive message callbacks
        /// </summary>
        /// <param name="nodeId">Node Id</param>
        /// <param name="data"> actual data in JSON format</param>
        public delegate void NewMessage(long nodeId, String data);
        
        /// <summary>
        /// Construct a streaming class and start receiving messages
        /// </summary>
        /// <param name="accountId">Your account id ( without { } )</param>
        public i4thingsStreaming(String accountId,
                                 NewMessage callback)
        {
            this.callback = callback;
            String path = System.IO.Path.GetDirectoryName(
                              System.Reflection.Assembly.GetExecutingAssembly().Location) +
                          "\\MarketHubFeedAdapters.dll";

            factory = new FeedAdapterFactory(path);

            // Create a new subscription adapter
            subAdapter = factory.CreateSubscriptionAdapter("");

            md = new MDWithReconnection(10000);

            // Create a new feed that will be served by the OTS subscription adapter
            feed = md.MD2.AddFeed(subAdapter, this, 1000, true);


            // Open a service named TEST SERVICE on the feed
            // it doesn't matter that the connection is not created yet, the service object will be attached
            // when/if the actual service becomes available
            service = feed.Services.OpenService(serviceName, ServiceStatusPropagation.Stale, 1000);

            subAdapter.Start();

            // Connect to the authority
            connection = subAdapter.Connect(uriBaseString + accountId.Replace('-','_'));

            if (connection == null)
            {
                return;
            }
            
        }

        /// <summary>
        /// Subscribe to receive mesdsages for specific node id
        /// </summary>
        /// <param name="nodeId">Node Id</param>
        public void Subscribe(long nodeId)
        {
            lock (this)
            {
                // And request a subscription for a security.
                // When/if security data/status become available, the securityListener will be notified.
                Security security = service.CreateSecurity(nodeId.ToString(), this, false, null, true);
                securities.Add(nodeId, security);
            }
        }

        /// <summary>
        /// Unsubscribe to receive mesdsages for specific node id
        /// </summary>
        /// <param name="nodeId">Node Id</param>
        public void UnSubscribe(long nodeId)
        {
            lock (this)
            {
                MarketHub.MD2.Security security = null;
                if (securities.TryGetValue(nodeId, out security))
                {
                    security.Close();
                }
            }
        }

        /// <summary>
        /// Use to dispose of all non native resources
        /// Can be called multiple times
        /// </summary>
        public void Dispose()
        {
            Close(); ;
        }
#endregion

    }
}
