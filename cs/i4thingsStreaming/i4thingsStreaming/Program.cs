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

namespace i4thingsStreaming
{
    class Program
    {
        private static readonly long Epoch = new DateTime(1970, 1, 1).Ticks;

        public static void NewMessage(i4thingsData data)
        {
            Console.WriteLine("Thing :" + data.Thing);
            Console.WriteLine("Time :" + data.Timestamp);
            DateTime localTime =  new DateTime(Epoch + data.Timestamp * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc).ToLocalTime();
            Console.WriteLine("Local Time :" + localTime);
            Console.WriteLine("Lat :" + data.Latitude);
            Console.WriteLine("Lon :" + data.Longitude);
            Console.WriteLine("Rssi(%) :" + data.RSSI);
            Console.WriteLine("Data:");
            foreach (Byte b in data.Data)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();

            Console.WriteLine("Decrypted Data:");
            Byte[] decrypterData = i4thingsStreaming.Decrypt(data.Data, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            foreach (Byte b in decrypterData)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();

            Console.WriteLine("JSON : " + data.ToString());

            
        }

        static void Main(string[] args)
        {
            // cleate the streamer
            i4thingsStreaming streamer = new i4thingsStreaming("661B3F9D-DAFF-7B2C-3368-F8D4F81531DB", NewMessage);

            // subscribe for messages froma  node
            streamer.Subscribe(14);
            
            // wait until enter is pressed
            Console.ReadLine();
        }
    }
}
