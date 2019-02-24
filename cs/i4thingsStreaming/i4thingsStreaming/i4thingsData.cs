﻿using System;
using System.Collections.Generic;
using System.Text;

namespace i4thingsStreaming
{
    public class i4thingsData
    {
        private long thing;
        private long timestamp;
        private Double latitude;
        private Double longitude;
        private Byte rssi;
        private Byte[] data;

        public i4thingsData(long thing,
                     long timestamp,
                     Double latitude,
                     Double longitude,
                     Byte rssi,
                     Byte[] data)
        {
            this.thing = thing;
            this.timestamp = timestamp;
            this.latitude = latitude;
            this.longitude = longitude;
            this.rssi = rssi;
            this.data = data;
        }

        public long Thing
        {
            get { return thing; }
        }

        public long Timestamp
        {
            get { return timestamp; }
        }

        public Double Latitude
        {
            get { return latitude; }
        }

        public Double Longitude
        {
            get { return longitude; }
        }

        public Byte RSSI
        {
            get { return rssi; }
        }

        public Byte[] Data
        {
            get { return data; }
        }

        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"thing\": ").Append(thing).Append(",");
            sb.Append("\"last\": [");
            sb.Append("{");
            sb.Append("\"t\": ").Append(timestamp).Append(", ");
            sb.Append("\"l\": ").Append(latitude).Append(", ");
            sb.Append("\"n\": ").Append(longitude).Append(", ");
            sb.Append("\"r\": ").Append(rssi).Append(", ");

            sb.Append("\"d\": ").Append("[ ");
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i]);
                if (i < (data.Length - 1))
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
}
