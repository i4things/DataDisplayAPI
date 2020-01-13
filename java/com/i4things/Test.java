package com.i4things;

import com.i4things.json.Parser;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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

public class Test
{

    private static void print(String r)
    {
        // The server response is in standard JSON format
        // e.g. feel free to use your own favorite JSON parser ( the example is with simple custom one located in com.i4things.json packet )
        Map<String, Object> root = new HashMap<String, Object>();
        try
        {
            Parser.parseJson(r, root);
            System.out.println("Thing : "  + root.get("thing"));
            for (Object e : (List<Object>)root.get("last"))
            {
                System.out.println();

                Map<String,Object> m = (Map<String,Object>) e;
                System.out.println(" Timestamp : "  + m.get("t"));
                System.out.println(" Latitude : "  + m.get("l"));
                System.out.println(" Lontitude : "  + m.get("n"));
                System.out.println(" RSSI : "  + m.get("r"));
                // print encrypted data
                System.out.print(" Encrypted Data[ ");
                for (Object d :  (List<Object>)m.get("d"))
                {
                    // remember byte in Java is from -127 to +128
                    System.out.print((byte)Integer.parseInt(d.toString()));
                    System.out.print(" ");
                }
                System.out.println("]");

                //decrypt data
                byte[] encryptedData = new byte[ ((List<Object>) m.get("d")).size()];
                int encryptedDataIndex = 0;
                for (Object d :  (List<Object>)m.get("d"))
                {
                    // remember byte in Java is from -127 to +128
                    encryptedData[encryptedDataIndex++] = (byte)Integer.parseInt(d.toString());
                }
                byte[] decryptedData = i4things.Decrypt(encryptedData,new byte[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});

                // print decrypted data
                System.out.print(" Decrypted Data[ ");
                for (byte b :  decryptedData)
                {
                    // remember byte in Java is from -127 to +128
                    System.out.print(b);
                    System.out.print(" ");
                }
                System.out.println("]");
            }

        }
        catch (Exception ex)
        {
            ex.printStackTrace();
        }
    }

    private static void printHist(String r)
    {
        // The server response is in standard JSON format
        // e.g. feel free to use your own favorite JSON parser ( the example is with simple custom one located in com.i4things.json packet )
        Map<String, Object> root = new HashMap<String, Object>();
        try
        {
            Parser.parseJson(r, root);
            System.out.println("Thing : "  + root.get("thing"));
            System.out.println("History day index : "  + root.get("hist"));
            for (Object e : (List<Object>)root.get("day"))
            {
                System.out.println();

                Map<String,Object> m = (Map<String,Object>) e;
                System.out.println(" Timestamp : "  + m.get("t"));
                System.out.println(" Latitude : "  + m.get("l"));
                System.out.println(" Lontitude : "  + m.get("n"));
                System.out.println(" RSSI : "  + m.get("r"));
                // print encrypted data
                System.out.print(" Encrypted Data[ ");
                for (Object d :  (List<Object>)m.get("d"))
                {
                    // remember byte in Java is from -127 to +128
                    System.out.print((byte)Integer.parseInt(d.toString()));
                    System.out.print(" ");
                }
                System.out.println("]");

                //decrypt data
                byte[] encryptedData = new byte[ ((List<Object>) m.get("d")).size()];
                int encryptedDataIndex = 0;
                for (Object d :  (List<Object>)m.get("d"))
                {
                    // remember byte in Java is from -127 to +128
                    encryptedData[encryptedDataIndex++] = (byte)Integer.parseInt(d.toString());
                }
                byte[] decryptedData = i4things.Decrypt(encryptedData,new byte[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});

                // print decrypted data
                System.out.print(" Decrypted Data[ ");
                for (byte b :  decryptedData)
                {
                    // remember byte in Java is from -127 to +128
                    System.out.print(b);
                    System.out.print(" ");
                }
                System.out.println("]");
            }

        }
        catch (Exception ex)
        {
            ex.printStackTrace();
        }
    }

    public static void main(String[] args)
    {

        try {
            String r = i4things.GetData(14, "B273A8E19FB72FC2981366CF54D5A0CB");
            System.out.println(r);
            print(r);
            System.out.println("---------------");
            r = i4things.GetLast(14, "B273A8E19FB72FC2981366CF54D5A0CB");
            System.out.println(r);
            print(r);
            System.out.println("---------------");
            r = i4things.GetFrom(14, 1548239349728L, "B273A8E19FB72FC2981366CF54D5A0CB");
            System.out.println(r);
            print(r);
            System.out.println("---------------");
            r = i4things.SetData(14, new byte[]{10, 20, 30}, "B273A8E19FB72FC2981366CF54D5A0CB", new byte[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});
            System.out.println((r == null) ? "OK" : r);
            System.out.println("---------------");
            r = i4things.GetHist(14, 0, "B273A8E19FB72FC2981366CF54D5A0CB");
            System.out.println(r);
            printHist(r);
            System.out.println("---------------");
        }
        catch (IOException ex)
        {
            ex.printStackTrace();
        }
    }
}
