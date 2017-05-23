using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BDG : StructuredField
	{
		private static string _abbr = "BDG";
		private static string _title = "Begin Document Environment Group";
		private static string _desc = "The Begin Document Environment Group structured field begins a document environment group, which establishes the environment parameters for the form map object. The scope of the document environment group is the containing form map.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BDG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}