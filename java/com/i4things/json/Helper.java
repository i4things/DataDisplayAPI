package com.i4things.json;

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

class Helper
{
    static StartEnd trim(StartEnd se, char[] ar)
    {
        int s = se.getS();
        int e = se.getE();

        for (; (s < e) && (ar[s] <= ' '); s++)
        {
        }

        for (; (e > s) && (ar[e-1] <= ' '); e--)
        {
        }

        return new StartEnd(s, e);
    }

    static boolean startsWith(StartEnd se, char[] ar, char startChar)
    {
        if (ar[se.getS()] == startChar)
        {
            return true;
        }

        return false;
    }

    static boolean endsWith(StartEnd se, char[] ar, char endChar)
    {
        if ((se.getE() > se.getS()) && (ar[se.getE()-1] == endChar))
        {
            return true;
        }

        return false;
    }

    static StartEnd substring(StartEnd se, char[] ar, int start)
    {
        return new StartEnd(se.getS() + start, se.getE());
    }

    static int indexOf(StartEnd se, char[] ar, char searchChar)
    {
        int s = se.getS();
        int e = se.getE();

        for (int i = 0; (s + i) < e; i++)
        {
            if (ar[s + i] == searchChar)
            {
                return i;
            }
        }

        return -1;
    }

    static int indexOfComma(StartEnd se, char[] ar)
    {
        int s = se.getS();
        int e = se.getE();

        boolean ignoreComma = false;
        for (int i = 0; (s + i) < e; i++)
        {
            char ch = ar[s + i];
            if (ch == '\\')
            {
                i++;
                ch = ar[s + i];
                if (ch == '"')
                {
                    i++;
                    ch = ar[s + i];
                }
            }
            if (ch == '"')
            {
                ignoreComma = !ignoreComma;
            }
            if ((!ignoreComma) && (ch == ','))
            {
                return i;
            }
        }

        return -1;
    }

    static boolean isEmpty(StartEnd se)
    {
        if (se.getS() == se.getE())
        {
            return true;
        }
        return false;
    }

    static String substring(StartEnd se, char[] ar, int start, int end)
    {
        return new String(ar, se.getS() + start, end - start);
    }

    static String toString(StartEnd se, char[] ar)
    {
        return new String(ar, se.getS(), se.getE() - se.getS());
    }
}
