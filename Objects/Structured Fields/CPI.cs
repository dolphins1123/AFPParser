using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AFPParser.StructuredFields
{
    public class CPI : StructuredField
    {
        private static string _abbr = "CPI";
        private static string _title = "Code Page Index";
        private static string _desc = "Associates one or more character IDs with code points.";

        // This incomplete set of offsets will be parsed manually, but store them for ease of use
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Graphic Character GID"),
            new Offset(8, Lookups.DataTypes.BITS, "Graphic Character Use Flags")
            {
                // Same mappings as the CPC offset
                Mappings = PageControl.Semantics.Offsets[1].Mappings
            },
            new Offset(9, Lookups.DataTypes.UBIN, "Code Point")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        // Store the related CPC field
        private static CPC _pageControl = null;
        private static CPC PageControl
        {
            get
            {
                if (_pageControl == null)
                    _pageControl = (CPC)Parser.AfpFile.FirstOrDefault(f => f.GetType() == typeof(CPC));

                return _pageControl;
            }
        }

        public CPI(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

           // CPI will have one or more repeating groups. The length of each is found in the CPC field
            if (PageControl == null)
                return "ERROR: Code Page Control field not found in file! Could not determine repeating group length.";

            // Single byte code points are length 10, double are length 11
            bool isSingleByte = new[] { 0x0A, 0xFE }.Contains(PageControl.Data[9]);
            int standardLength = isSingleByte ? 10 : 11;

            // Loop through however many sections we need to
            for (int curIndex = 0; curIndex < Data.Length;)
            {
                // Retrieve the byte sections
                byte[] GCGID = GetSectionedData(curIndex, 8);
                byte[] PrtFlags = GetSectionedData(curIndex + 8, 1);
                byte[] CodePoint = GetSectionedData(curIndex + 9, isSingleByte ? 1 : 2);

                // Display first 3 semantics based on predefined offsets above
                sb.AppendLine(Offsets[0].DisplayDataByType(GCGID));
                sb.AppendLine(Offsets[1].DisplayDataByType(PrtFlags));
                sb.AppendLine(Offsets[2].DisplayDataByType(CodePoint));

                // If this code point includes unicode scalar value entries, parse them here
                if (!isSingleByte)
                {
                    int numScalarValues = Data[standardLength];

                    for (int i = 0; i < numScalarValues; i += 4)
                    {
                        Offset fakeOffset = new Offset(0, Lookups.DataTypes.UBIN, $"Unicode Scalar Value {i + 1}");

                        // Each scalar value is a four byte UBIN
                        int startingIndex = standardLength + (i * 4);
                        byte[] scalarValue = GetSectionedData(startingIndex, 4);
                        sb.AppendLine(fakeOffset.DisplayDataByType(scalarValue));
                    }

                    // Take extra semantics into account
                    curIndex += 1 + (numScalarValues * 4);
                }
                sb.AppendLine();

                // Go to the next one
                curIndex += standardLength;
            }

            return sb.ToString();
        }
    }
}