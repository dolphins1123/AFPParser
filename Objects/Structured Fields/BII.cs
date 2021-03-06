using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class BII : StructuredField
    {
        private static string _abbr = "BII";
        private static string _title = "Begin Image Object IM";
        private static string _desc = "The Begin IM Image Object structured field begins an IM image data object, which becomes the current data object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Image Object Name")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public string ObjectName { get; private set; }

        public BII(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            ObjectName = GetReadableDataPiece(0, 8);
        }
    }
}