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
	/// Compares two strings (case sensitive).
	/// The strings are passed through the StringParser, so it is possible to compare
	/// SharpDevelop properties.<br/>
	/// Useful if you want to run a command only when a setting is active to prevent
	/// loading your addin if that setting isn't set.
	/// </summary>
	/// <attribute name="string">
	/// The first string.
	/// </attribute>
	/// <attribute name="equals">
	/// The second string.
	/// </attribute>
	/// <example title="Test if the browser is showing a HtmlHelp page">
	/// &lt;Condition name = "Compare" string = "${property:SharpDevelop.FiletypesRegisterStartup}" equals = "True"&gt;
	/// </example>
	public class CompareConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return StringParser.Parse(condition.Properties["string"]) == StringParser.Parse(condition.Properties["equals"]);
		}
	}
}
