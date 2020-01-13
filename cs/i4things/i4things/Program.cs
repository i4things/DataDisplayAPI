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
using Newtonsoft.Json;

namespace i4things
{
    class Program
    {
        static void Print(String r)
        {
            Console.WriteLine("\n#######################");
            i4thingsResponse deserializedResponse = JsonConvert.DeserializeObject<i4thingsResponse>(r);
            Console.WriteLine(deserializedResponse.ToString());
            Console.WriteLine("\n#######################\n");

            // actual data decryprion with the private key
            Console.WriteLine("\n+++ response(Last[0].Data) decrypted +++++++\n");
            Byte[] decrypted = i4things.Decrypt(deserializedResponse.Last[0].Data, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            for (int i = 0; i < decrypted.Length; i++)
            {
                Console.Write(decrypted[i]);
                Console.Write(" ");
            }
            
            Console.WriteLine("\n++++++++++++++++++++++++++++++++++++++++++++\n");
        }

        static void PrintHist(String r)
        {
            Console.WriteLine("\n#######################");
            i4thingsHistResponse deserializedHistResponse = JsonConvert.DeserializeObject<i4thingsHistResponse>(r);
            Console.WriteLine(deserializedHistResponse.ToString());
            Console.WriteLine("\n#######################\n");
        }

        static void Main(string[] args)
        {

            String r = i4things.GetData(14, "B273A8E19FB72FC2981366CF54D5A0CB");
            Console.WriteLine(r);
            Print(r);
            /*
            Console.WriteLine("---------------");
            r = i4things.GetLast(14, "B273A8E19FB72FC2981366CF54D5A0CB");
            Console.WriteLine(r);
            Print(r);
            Console.WriteLine("---------------");
            r = i4things.GetFrom(14, 1548239349728, "B273A8E19FB72FC2981366CF54D5A0CB");
            Console.WriteLine(r);
            Print(r);
            Console.WriteLine("---------------");
            r = i4things.SetData(14,new Byte[] {10,20,30},  "B273A8E19FB72FC2981366CF54D5A0CB", new Byte[] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15});
            Console.WriteLine((r == null) ? "OK" : r );
            Console.WriteLine("---------------");
            r = i4things.GetHist(14, 0, "B273A8E19FB72FC2981366CF54D5A0CB");
            Console.WriteLine(r);
            PrintHist(r);
            Console.WriteLine("---------------"); */
            Console.ReadLine();
        }
    }
}
