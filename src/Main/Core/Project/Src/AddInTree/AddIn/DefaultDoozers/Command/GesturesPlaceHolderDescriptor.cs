using System;

namespace ICSharpCode.Core
{
	public class GesturesPlaceHolderDescriptor
	{
		public string Name {
			get; private set;
		}
		
		public string Text {
			get; private set;
		}
		
		public string Gestures {
			get; private set;
		}
		
		public GesturesPlaceHolderDescriptor(Codon codon)
		{
			Name = codon.Properties["name"];
			Text = codon.Properties["text"];
			Gestures = codon.Properties["gestures"];
		}
	}
}
