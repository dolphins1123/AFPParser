using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BOC : StructuredField
	{
		private static string _abbr = "BOC";
		private static string _title = "Begin Object Container";
		private static string _desc = "The Begin Object Container structured field begins an object container, which may be used to envelop and carry object data.The object data may or may not bedefined by an AFP presentation architecture.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BOC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}