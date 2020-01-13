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

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class Parser
{

    public static void parseJson(String json, Map<String, Object> root) throws Exception
    {
        parseJson(new StartEnd(0, json.length()), json.toCharArray(), root);
    }

    public static StartEnd parseJson(StartEnd jsonSE, char[] jsonAR, Map<String, Object> root) throws Exception
    {

        jsonSE = Helper.trim(jsonSE, jsonAR);

        if (Helper.startsWith(jsonSE, jsonAR, '[') && Helper.endsWith(jsonSE, jsonAR, ']'))
        {
            List<Object> arrayRoot = new ArrayList<Object>();
            root.put("result", arrayRoot);

            jsonSE = parseJsonArray(jsonSE, jsonAR, arrayRoot);

            jsonSE = Helper.trim(jsonSE, jsonAR);

            return jsonSE;
        }

        if (!(Helper.startsWith(jsonSE, jsonAR, '{')))
        {
            String text = "Corrupted json message: " + Helper.toString(jsonSE, jsonAR);
            throw new Exception(text);
        }

        //remove {
        jsonSE = Helper.substring(jsonSE, jsonAR, 1);
        jsonSE = Helper.trim(jsonSE, jsonAR);

        for (; ; )
        {
            int colonPos = Helper.indexOf(jsonSE, jsonAR, ':');
            if (colonPos < 0)
            {
                return jsonSE;
            }

            String keyName = trimName(Helper.substring(jsonSE, jsonAR, 0, colonPos).trim());
            jsonSE = Helper.substring(jsonSE, jsonAR, colonPos + 1);
            jsonSE = Helper.trim(jsonSE, jsonAR);

            // test if simple value
            if (Helper.startsWith(jsonSE, jsonAR, '{'))
            {
                Map<String, Object> nestedRoot = new HashMap<String, Object>();
                root.put(keyName, nestedRoot);

                jsonSE = parseJson(jsonSE, jsonAR, nestedRoot);

                jsonSE = Helper.trim(jsonSE, jsonAR);

                if (Helper.isEmpty(jsonSE))
                {
                    return jsonSE;
                }

                if (Helper.startsWith(jsonSE, jsonAR, '}'))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);
                }

                if (Helper.isEmpty(jsonSE))
                {
                    return jsonSE;
                }

                if (Helper.startsWith(jsonSE, jsonAR, ','))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);
                }

                if (Helper.isEmpty(jsonSE))
                {
                    return jsonSE;
                }

                continue;
            }

            // simple
            // try for array
            if (Helper.startsWith(jsonSE, jsonAR, '['))
            {

                List<Object> arrayRoot = new ArrayList<Object>();
                root.put(keyName, arrayRoot);

                jsonSE = parseJsonArray(jsonSE, jsonAR, arrayRoot);

                jsonSE = Helper.trim(jsonSE, jsonAR);

                if (Helper.startsWith(jsonSE, jsonAR, '}'))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);

                    return jsonSE;
                }

                if (Helper.startsWith(jsonSE, jsonAR, ','))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);
                }

                if (Helper.isEmpty(jsonSE))
                {
                    return jsonSE;
                }

                continue;
            }
            boolean closeBracketFound = false;
            int valuePos = Helper.indexOfComma(jsonSE, jsonAR);//json.indexOf(',');
            int continuePos = valuePos + 1;
            if (valuePos < 0)
            {
                valuePos = Helper.indexOf(jsonSE, jsonAR, '}');
                continuePos = valuePos + 1;
                closeBracketFound = true;
                if (valuePos < 0)
                {
                    String text = "Corrupted json message. Cannot find next/end key/value delimiter: " + Helper.toString(jsonSE, jsonAR);
                    throw new Exception(text);
                }
            }
            else
            {
                int indexOfBracket = Helper.indexOf(new StartEnd(jsonSE.getS(), jsonSE.getS() + valuePos), jsonAR, '}'); //json.substring(0, valuePos).indexOf("}");
                if (indexOfBracket >= 0)
                {
                    valuePos = indexOfBracket;
                    continuePos = valuePos + 1;
                    closeBracketFound = true;
                }

            }

            String value = trimName(Helper.substring(jsonSE, jsonAR, 0, valuePos).trim()); //trimName(json.substring(0, valuePos).trim());

            root.put(keyName, value);

            jsonSE = Helper.substring(jsonSE, jsonAR, continuePos);

            jsonSE = Helper.trim(jsonSE, jsonAR);

            if (Helper.isEmpty(jsonSE) || closeBracketFound)
            {
                return jsonSE;
            }

        }

    }


    private static StartEnd parseJsonArray(StartEnd jsonSE, char[] jsonAR, List<Object> arrayRoot) throws Exception
    {

        //remove [
        jsonSE = Helper.substring(jsonSE, jsonAR, 1);
        jsonSE = Helper.trim(jsonSE, jsonAR);

        // handle empty array
        if (Helper.startsWith(jsonSE, jsonAR, ']'))
        {
            //remove ]
            jsonSE = Helper.substring(jsonSE, jsonAR, 1);
            jsonSE = Helper.trim(jsonSE, jsonAR);
            return jsonSE;
        }
        for (; ; )
        {

            // test if nested value
            if (Helper.startsWith(jsonSE, jsonAR, '['))
            {
                List<Object> nestedRoot = new ArrayList<Object>();
                arrayRoot.add(nestedRoot);

                jsonSE = parseJsonArray(jsonSE, jsonAR, nestedRoot);

                jsonSE = Helper.trim(jsonSE, jsonAR);

                if (Helper.startsWith(jsonSE, jsonAR, ']'))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);

                    return jsonSE;
                }

                if (Helper.startsWith(jsonSE, jsonAR, ','))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);
                }

                continue;
            }
            // test if array of objects
            if (Helper.startsWith(jsonSE, jsonAR, '{'))
            {
                HashMap<String, Object> resu = new HashMap<String, Object>();
                arrayRoot.add(resu);

                jsonSE = parseJson(jsonSE, jsonAR, resu);

                jsonSE = Helper.trim(jsonSE, jsonAR);

                if (Helper.startsWith(jsonSE, jsonAR, ','))
                {
                    jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                    jsonSE = Helper.trim(jsonSE, jsonAR);
                }

                continue;
            }

            // test if array end
            if (Helper.startsWith(jsonSE, jsonAR, ']'))
            {
                jsonSE = Helper.substring(jsonSE, jsonAR, 1);
                jsonSE = Helper.trim(jsonSE, jsonAR);

                return jsonSE;
            }
            // simple

            boolean closeBracketFound = false;
            int valuePos = Helper.indexOfComma(jsonSE, jsonAR);//json.indexOf(',');
            int continuePos = valuePos + 1;
            if (valuePos < 0)
            {
                valuePos = Helper.indexOf(jsonSE, jsonAR, ']');
                continuePos = valuePos + 1;
                closeBracketFound = true;
                if (valuePos < 0)
                {
                    String text = "Corrupted json message. Cannot find next/end key/value delimiter: " + Helper.toString(jsonSE, jsonAR);
                    throw new Exception(text);
                }
            }
            else
            {
                int indexOfBracket = Helper.indexOf(new StartEnd(jsonSE.getS(), jsonSE.getS() + valuePos), jsonAR, ']');// json.substring(0, valuePos).indexOf("]");
                if (indexOfBracket >= 0)
                {
                    valuePos = indexOfBracket;
                    continuePos = valuePos + 1;
                    closeBracketFound = true;
                }

            }
            String value = trimName(Helper.substring(jsonSE, jsonAR, 0, valuePos).trim()); //trimName(json.substring(0, valuePos).trim());

            arrayRoot.add(value);

            jsonSE = Helper.substring(jsonSE, jsonAR, continuePos);
            jsonSE = Helper.trim(jsonSE, jsonAR);

            if (Helper.isEmpty(jsonSE) || closeBracketFound)
            {
                return jsonSE;
            }

        }

    }

    private static String trimName(String name)
    {
        if (name.startsWith("\""))
        {
            name = name.substring(1);
        }

        if (name.endsWith("\""))
        {
            name = name.substring(0, name.length() - 1);
        }

        return name.replace("\\\"", "\"");
    }



}
