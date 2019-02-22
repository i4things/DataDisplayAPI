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
        public static void NewMessage(long nodeId, String data)
        {
            Console.WriteLine(nodeId + " " + data);
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
