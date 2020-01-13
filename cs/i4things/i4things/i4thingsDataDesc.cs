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

namespace i4things
{
    public class i4thingsPayload
    {
        public long t;
        public Double l;
        public Double n;
        public Byte r;
        public List<Byte> d;

        public long Timestamp { get { return t;} }
        public Double Latitude { get { return l;} }
        public Double Longitude { get { return n; } }
        public Byte RSSI { get { return r; } }
        public Byte[] Data { get { return d.ToArray(); } }


        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"last\": [");
            sb.Append("{");
            sb.Append("\"t\": ").Append(Timestamp).Append(", ");
            sb.Append("\"l\": ").Append(Latitude).Append(", ");
            sb.Append("\"n\": ").Append(Longitude).Append(", ");
            sb.Append("\"r\": ").Append(RSSI).Append(", ");

            sb.Append("\"d\": ").Append("[ ");
            for (int i = 0; i < Data.Length; i++)
            {
                sb.Append(Data[i]);
                if (i < (Data.Length - 1))
                {
                    sb.Append(", ");
                }
            }
            sb.Append("] }");

            sb.Append("]");
            sb.Append("}");

            return sb.ToString();
        }  

    }

    public class i4thingsResponse
    {
        public long thing;
        public List<i4thingsPayload> last;

        public long Thing { get { return thing; } }
        public i4thingsPayload[] Last { get { return last.ToArray(); } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"thing\": ").Append(Thing).Append(",");
            sb.Append("\"last\": [");
            for (int i = 0; i < Last.Length; i++)
            {
                sb.Append("{");
                sb.Append("\"t\": ").Append(Last[i].Timestamp).Append(", ");
                sb.Append("\"l\": ").Append(Last[i].Latitude).Append(", ");
                sb.Append("\"n\": ").Append(Last[i].Longitude).Append(", ");
                sb.Append("\"r\": ").Append(Last[i].RSSI).Append(", ");

                sb.Append("\"d\": ").Append("[ ");
                for (int j = 0; j < Last[i].Data.Length; j++)
                {
                    sb.Append(Last[i].Data[j]);
                    if (j < (Last[i].Data.Length - 1))
                    {
                        sb.Append(", ");
                    }
                }
                
                sb.Append("] }");

                if (i < (Last.Length - 1))
                {
                    sb.Append(", ");
                }
            } 
            sb.Append("]");
            sb.Append("}");

            return sb.ToString();
        }  
    }

    public class i4thingsHistResponse
    {
        public long thing;
        public int hist;
        public List<i4thingsPayload> day;

        public long Thing { get { return thing; } }
        public int Hist { get { return hist; } }
        public i4thingsPayload[] Day { get { return day.ToArray(); } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"thing\": ").Append(Thing).Append(",");
            sb.Append("\"hist\": ").Append(Hist).Append(",");
            sb.Append("\"day\": [");
            for (int i = 0; i < Day.Length; i++)
            {
                sb.Append("{");
                sb.Append("\"t\": ").Append(Day[i].Timestamp).Append(", ");
                sb.Append("\"l\": ").Append(Day[i].Latitude).Append(", ");
                sb.Append("\"n\": ").Append(Day[i].Longitude).Append(", ");
                sb.Append("\"r\": ").Append(Day[i].RSSI).Append(", ");

                sb.Append("\"d\": ").Append("[ ");
                for (int j = 0; j < Day[i].Data.Length; j++)
                {
                    sb.Append(Day[i].Data[j]);
                    if (j < (Day[i].Data.Length - 1))
                    {
                        sb.Append(", ");
                    }
                }

                sb.Append("] }");

                if (i < (Day.Length - 1))
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");
            sb.Append("}");

            return sb.ToString();
        }
    }
}
