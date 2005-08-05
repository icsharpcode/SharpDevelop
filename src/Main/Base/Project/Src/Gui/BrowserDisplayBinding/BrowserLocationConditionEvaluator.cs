// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	/// <summary>
	/// Conditions that tries to match the URL of a <see cref="HtmlViewPane"/> with a regex.
	/// </summary>
	public class BrowserLocationConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			HtmlViewPane pane = (HtmlViewPane)caller;
			Uri uri = pane.Url;
			if (uri == null)
				return false;
			string url = uri.ToString();
			string pattern = condition.Properties["urlRegex"];
			string options = condition.Properties["options"];
			if (options != null && options.Length > 0)
				return Regex.IsMatch(url, pattern, (RegexOptions)Enum.Parse(typeof(RegexOptions), options, true));
			else
				return Regex.IsMatch(url, pattern);
		}
	}
}
