using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class CompareAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			return StringParser.Parse(condition.Properties["string"]) == StringParser.Parse(condition.Properties["equals"]);
		}
	}
}
