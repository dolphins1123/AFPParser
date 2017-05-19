﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class UniversalDateandTimeStamp : Triplet
    {
        private static string _desc = "Specifies a date and time in accordance with the format defined in ISO 8601: 1988 (E).";
        private static List<Offset> _oSets = new List<Offset>();

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public UniversalDateandTimeStamp(byte[] allData) : base(allData) { }

        public override string GetDescription()
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            if (Data.Length < 11)
                return $"Invalid data length of {Data.Length}. Expected 11.";

            // Get the year by proper endian
            byte[] orderedBytes = Data.Skip(1).Take(2).ToArray();
            if (BitConverter.IsLittleEndian) orderedBytes = orderedBytes.Reverse().ToArray();
            int year = BitConverter.ToUInt16(orderedBytes, 0);

            // Store everything else
            int month = Data[3];
            int day = Data[4];
            int hour = Data[5];
            int minute = Data[6];
            int second = Data[7];

            // Calculate the date
            DateTime formattedDate = new DateTime(year, month, day, hour, minute, second);

            // Add/subtract time zone info if there is an offset
            if (Data[8] > 0)
            {
                int hoursAhead = Data[9];
                int minutesAhead = Data[10];

                formattedDate = formattedDate.AddHours(hoursAhead).AddMinutes(minutesAhead);
            }

            return sb.ToString();
        }
    }
}