// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
