package com.i4things;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;

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

public class i4things
{
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

    private static final int DELTA = 0x9E3779B9;

    private static int MX(int sum, int y, int z, int p, int e, int[] k)
    {
        return (z >>> 5 ^ y << 2) + (y >>> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
    }

    private static int[] xxteaToInt32ArrRaw(byte[] bs)
    {
        int length = bs.length;
        int n = length >> 2;
        int[] v = new int[n];
        for (int i = 0; i < n; ++i)
        {
            int j = i << 2;
            v[i] = ((bs[j + 3] & 0xFF) << 24) | ((bs[j + 2] & 0xFF) << 16) | ((bs[j + 1] & 0xFF) << 8) | (bs[j] & 0xFF);
        }
        return v;
    }

    private static int[] xxteaToInt32ArrSize(byte[] bs)
    {
        int length = bs.length;
        ++length;
        byte[] bs_copy = new byte[(((length & 3) == 0) ? length : ((length >> 2) + 1) << 2)];
        --length;
        bs_copy[0] = (byte) length;
        System.arraycopy(bs, 0, bs_copy, 1, bs.length);

        int n = bs_copy.length >> 2;

        if (n < 2)
        {
            n = 2;
        }

        int[] v = new int[n];

        for (int i = 0; i < n; ++i)
        {
            int j = i << 2;
            v[i] = ((bs_copy[j + 3] & 0xFF) << 24) | ((bs_copy[j + 2] & 0xFF) << 16) | ((bs_copy[j + 1] & 0xFF) << 8) | (bs_copy[j] & 0xFF);
        }
        return v;
    }

    private static byte[] xxteaToInt8ArrSize(int[] v)
    {
        byte[] bs = new byte[v.length << 2];
        for (int i = 0; i < v.length; i++)
        {
            int j = (i << 2);
            bs[j + 3] = (byte) (v[i] >> 24 & 0xFF);
            bs[j + 2] = (byte) (v[i] >> 16 & 0xFF);
            bs[j + 1] = (byte) (v[i] >> 8 & 0xFF);
            bs[j] = (byte) (v[i] & 0xFF);
        }
        byte[] ret = new byte[bs[0] & 0xFF];


        try
        {
            System.arraycopy(bs, 1, ret, 0, ret.length);
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }


        return ret;
    }

    private static byte[] xxteaToInt8ArrRaw(int[] v)
    {
        byte[] bs = new byte[v.length << 2];
        for (int i = 0; i < v.length; i++)
        {
            int j = (i << 2);
            bs[j + 3] = (byte) (v[i] >> 24 & 0xFF);
            bs[j + 2] = (byte) (v[i] >> 16 & 0xFF);
            bs[j + 1] = (byte) (v[i] >> 8 & 0xFF);
            bs[j] = (byte) (v[i] & 0xFF);
        }
        return bs;
    }

    private static int[] xxteaDecryptInt32Arr(int[] v, int[] k)
    {
        int n = v.length - 1;

        if (n < 1)
        {
            return v;
        }
        int p, q = 6 + 52 / (n + 1);
        int z, y = v[0], sum = q * DELTA, e;

        while (sum != 0)
        {
            e = sum >>> 2 & 3;
            for (p = n; p > 0; p--)
            {
                z = v[p - 1];
                y = v[p] -= MX(sum, y, z, p, e, k);
            }
            z = v[n];
            y = v[0] -= MX(sum, y, z, p, e, k);
            sum = sum - DELTA;
        }
        return v;
    }

    private static byte[] xxteaDecrypt(byte[] data, byte[] key)
    {
        if (data == null || data.length == 0)
        {
            return data;
        }
        int[] dataAsIntArr = xxteaToInt32ArrRaw(data);
        int[] keyAsIntArr = xxteaToInt32ArrRaw(key);
        int[] decryptedIntArr = xxteaDecryptInt32Arr(dataAsIntArr, keyAsIntArr);

        return xxteaToInt8ArrSize(decryptedIntArr);
    }

    private static int[] xxteaEncryptInt32Arr(int[] v, int[] k)
    {
        int n = v.length - 1;

        if (n < 1)
        {
            return v;
        }
        int p, q = 6 + 52 / (n + 1);
        int z = v[n], y, sum = 0, e;

        while (q-- > 0)
        {
            sum = sum + DELTA;
            e = sum >>> 2 & 3;
            for (p = 0; p < n; p++)
            {
                y = v[p + 1];
                z = v[p] += MX(sum, y, z, p, e, k);
            }
            y = v[0];
            z = v[n] += MX(sum, y, z, p, e, k);
        }
        return v;
    }

    private static byte[] xxteaEncrypt(byte[] data, byte[] key)
    {
        if (data == null || data.length == 0)
        {
            return data;
        }
        int[] dataAsIntArr = xxteaToInt32ArrSize(data);
        int[] keyAsIntArr = xxteaToInt32ArrRaw(key);
        int[] encryptedIntArr = xxteaEncryptInt32Arr(dataAsIntArr, keyAsIntArr);
        return xxteaToInt8ArrRaw(encryptedIntArr);
    }

    /**********************************************************\
     XXTEA END
     \**********************************************************/

    private static String toHex(final byte[] b)
    {
        String result = "";
        for (int i = 0; i < b.length; i++)
        {
            result += Integer.toString((b[i] & 0xff) + 0x100, 16).substring(1);
        }
        return result.toUpperCase();
    }

    private static byte[] fromHex(final String s)
    {
        if ((s.length() % 2) != 0)
        {
            return null;
        }

        final byte result[] = new byte[s.length() / 2];
        final char enc[] = s.toCharArray();
        for (int i = 0; i < enc.length; i += 2)
        {
            StringBuilder curr = new StringBuilder(2);
            curr.append(enc[i]).append(enc[i + 1]);
            result[i / 2] = (byte) Integer.parseInt(curr.toString(), 16);
        }
        return result;
    }

    private static byte[] toByteArray(long l)
    {
        byte[] ba = { 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < ba.length; i++)
        {
            byte b = (byte)(l & 0xFF);
            ba[i] = b;
            l = (l - b) / 256;
        }

        return ba;
    }

    private static long ToLong(byte[] ba) {
        long v = 0;
        for (int i = ba.length - 1; i >= 0; i--) {
            v = (v * 256) + (ba[i] & 0xFF);
        }

        return v;
    }

    // calculate checksum 4b
    private static long crc4(byte[] array)
    {
        long res = 0;
        for (int i = 0; i < array.length; i++)
        {
            int c = array[i] & 0xFF;
            res = (res << 1) ^ c;
            res = res & 0xFFFFFFFFL;
        }
        return res;
    }

    // calculate checksum 1b
    private static byte crc(byte[] array)
    {
        long res = 8606;
        for (int i = 0; i < array.length; i++)
        {
            int c = array[i] & 0xFF;
            res = (res << 1) ^ c;
            res = res & 0xFFFFFFFFL;
        }
        return (byte) (res & 0xFF);
    }

    private static byte[] challenge(String networkKey)
    {
        byte[] c = toByteArray(System.currentTimeMillis());
        // gen CRC
        long crc = crc4(c);
        // and in front of challenge
        byte[] cRet = new byte[c.length + 4];
        cRet[0] = (byte)(crc & 0xFF);
        cRet[1] = (byte)(crc >> 8 & 0xFF);
        cRet[2] = (byte)(crc >> 16 & 0xFF);
        cRet[3] = (byte)(crc >> 24 & 0xFF);
        System.arraycopy(c, 0, cRet, 4, c.length);
        // encrypt
        return xxteaEncrypt(cRet, fromHex(networkKey));
    }

    // id integer. network_key in HEX format 32 chars
    private static String getDataRequest(long id, String networkKey)
    {
        // gen challenge
        byte[] c = challenge(networkKey);
        StringBuilder sb = new StringBuilder();
        sb.append(new Long(id).toString()).append('-').append(toHex(c).toUpperCase());
        return sb.toString();
    }

    // id integer, data byte array, network_key in HEX format 32 chars, private_key 16 bytes array
    private static String setDataRequest(long id, byte[] data, String networkKey, byte[] privateKey)
    {
        // gen challenge
        byte[] c = challenge(networkKey);
        byte crc = crc(data);
        byte[] dataCRC = new byte[data.length + 1];
        dataCRC[0] = crc;
        System.arraycopy(data, 0, dataCRC, 1, data.length);
        StringBuilder sb = new StringBuilder();
        sb.append(new Long(id).toString()).append('-').append(toHex(c).toUpperCase()).append('-').append(toHex(xxteaEncrypt(dataCRC, privateKey)).toUpperCase());
        return sb.toString();
    }

    // id integer, hist day index integer, network_key in HEX format 32 chars
    private static String getDataHistRequest(long id, byte dayIdx, String networkKey)
    {
        // gen challenge
        byte[] c = challenge(networkKey);
        StringBuilder sb = new StringBuilder();
        sb.append(new Long(id).toString()).append('-').append(new Byte(dayIdx).toString()).append('-').append(toHex(c).toUpperCase());
        return sb.toString();
    }

    // id integer, from timestamp long, network_key in HEX format 32 chars
    private static String getDataFromRequest(long id, long timestamp, String networkKey)
    {
        // gen challenge
        byte[] c = challenge(networkKey);
        StringBuilder sb = new StringBuilder();
        sb.append(new Long(id).toString()).append('-').append(new Long(timestamp).toString()).append('-').append(toHex(c).toUpperCase());
        return sb.toString();
    }


    private static int resultStart = 16;
    private static int resultEnd = 2;
    private static String server = "http://server.i4things.com:5408/";

    /**********************************************************\
     PUBLIC
     \**********************************************************/

    /**
     * Use to decrypt received data
     * @param data - Data to decrypt
     * @param privateKey - Private key - byte array 16 elements
     * @return Decrypted data
     */
    public static byte[] Decrypt(byte[] data, byte[] privateKey)
    {
        return xxteaDecrypt(data, privateKey);
    }

    /**
     * Use to request/receive all data for the current day from the server
     *
     * @param id - Node Id
     * @param networkKey - Network key (HEX format 32 chars)
     * @return - Data
     */

    public static String GetData(long id, String networkKey) throws IOException
    {
        StringBuilder uri = new StringBuilder(server);
        uri.append("iot_get/").append(getDataRequest(id,networkKey));

        URL url = new URL(uri.toString());
        URLConnection urlConn = url.openConnection();
        urlConn.setConnectTimeout(30000);
        urlConn.setReadTimeout(30000);

        BufferedReader in = new BufferedReader( new InputStreamReader(urlConn.getInputStream()));
        StringBuilder sr = new StringBuilder();
        String inputLine;
        while ((inputLine = in.readLine()) != null)
        {
            sr.append(inputLine);
        }
        in.close();

        String r = sr.toString().trim();
        return r.substring(resultStart, r.length() - resultEnd);

    }

    /**
     * Use to request/receive only last data received from the node
     *
     * @param id - Node Id
     * @param networkKey - Network key (HEX format 32 chars)
     * @return Data
     */
    public static String GetLast(long id, String networkKey) throws IOException
    {
        StringBuilder uri = new StringBuilder(server);
        uri.append("iot_get_last/").append(getDataRequest(id, networkKey));

        URL url = new URL(uri.toString());
        URLConnection urlConn = url.openConnection();
        urlConn.setConnectTimeout(30000);
        urlConn.setReadTimeout(30000);

        BufferedReader in = new BufferedReader( new InputStreamReader(urlConn.getInputStream()));
        StringBuilder sr = new StringBuilder();
        String inputLine;
        while ((inputLine = in.readLine()) != null)
        {
            sr.append(inputLine);
        }
        in.close();

        String r = sr.toString().trim();
        return r.substring(resultStart, r.length() - resultEnd);
    }

    /**
     * Use to request/receive history data for specific day
     *
     * @param id - Node Id
     * @param day - Day for which the data is requested - 0 yesterday, 1 - the day before yesterday etc.
     * @param networkKey - Network key (HEX format 32 chars)</param>
     * @return Data
     */
    public static String GetHist(long id, int day, String networkKey) throws IOException
    {
        StringBuilder uri = new StringBuilder(server);
        uri.append("iot_get_hist/").append(getDataHistRequest(id, (byte)day, networkKey));

        URL url = new URL(uri.toString());
        URLConnection urlConn = url.openConnection();
        urlConn.setConnectTimeout(30000);
        urlConn.setReadTimeout(30000);

        BufferedReader in = new BufferedReader( new InputStreamReader(urlConn.getInputStream()));
        StringBuilder sr = new StringBuilder();
        String inputLine;
        while ((inputLine = in.readLine()) != null)
        {
            sr.append(inputLine);
        }
        in.close();

        String r = sr.toString().trim();
        return r.substring(resultStart, r.length() - resultEnd);
    }

    /**
     * Use to request/receive all data after a timestamp from the current day from the server
     *
     * @param id - Node Id
     * @param from - timestamp in millis after 1,1,1970 in UTC
     * @param networkKey - Network key (HEX format 32 chars)
     * @return Data
     */
    public static String GetFrom(long id, long from, String networkKey) throws IOException
    {
        StringBuilder uri = new StringBuilder(server);
        uri.append("iot_get_from/").append(getDataFromRequest(id, from, networkKey));

        URL url = new URL(uri.toString());
        URLConnection urlConn = url.openConnection();
        urlConn.setConnectTimeout(30000);
        urlConn.setReadTimeout(30000);

        BufferedReader in = new BufferedReader( new InputStreamReader(urlConn.getInputStream()));
        StringBuilder sr = new StringBuilder();
        String inputLine;
        while ((inputLine = in.readLine()) != null)
        {
            sr.append(inputLine);
        }
        in.close();

        String r = sr.toString().trim();
        return r.substring(resultStart, r.length() - resultEnd);
    }

    /**
     * Use to send data to node
     * @param id - Node Id
     * @param data - Data to be sent - byte array - the data will be signed and encrypted and checked at the node automatically
     * @param networkKey - Network key (HEX format 32 chars)
     * @param privateKey - Private key - byte array 16 elements
     * @return null - all good else error
     */
    public static String SetData(long id, byte[] data, String networkKey, byte[] privateKey) throws IOException
    {
        StringBuilder uri = new StringBuilder(server);
        uri.append("iot_set/").append(setDataRequest(id, data, networkKey, privateKey));

        URL url = new URL(uri.toString());
        URLConnection urlConn = url.openConnection();
        urlConn.setConnectTimeout(30000);
        urlConn.setReadTimeout(30000);

        BufferedReader in = new BufferedReader( new InputStreamReader(urlConn.getInputStream()));
        StringBuilder sr = new StringBuilder();
        String inputLine;
        while ((inputLine = in.readLine()) != null)
        {
            sr.append(inputLine);
        }
        in.close();

        String r = sr.toString().trim();

        if (r.indexOf("OK") >= 0)
        {
            return null;
        }
        else
        {
            return r.substring(resultStart, r.length() - resultEnd);
        }

    }
}
