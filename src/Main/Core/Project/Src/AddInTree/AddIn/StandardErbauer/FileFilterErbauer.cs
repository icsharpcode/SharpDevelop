using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class FileFilterErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return StringParser.Parse(codon.Properties["name"]) + "|" + codon.Properties["extensions"];
		}
	}
}
