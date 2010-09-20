// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	/// <summary>
	/// Conditions that tries to match the URL of a <see cref="HtmlViewPane"/> with a regex.
	/// </summary>
	/// <attribute name="urlRegex">
	/// The regular expression that must match the URL.
	/// </attribute>
	/// <attribute name="options">
	/// Optional; options that are passed as <see cref="RegexOptions"/>.
	/// </attribute>
	/// <example title="Test if the browser is showing a HtmlHelp page">
	/// &lt;Condition name = "BrowserLocation" urlRegex = "^ms-help:\/\/"&gt;
	/// </example>
	public class BrowserLocationConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			HtmlViewPane pane = (HtmlViewPane)caller;
			string url = pane.Url.ToString();
			string pattern = condition.Properties["urlRegex"];
			string options = condition.Properties["options"];
			if (options != null && options.Length > 0)
				return Regex.IsMatch(url, pattern, (RegexOptions)Enum.Parse(typeof(RegexOptions), options, true));
			else
				return Regex.IsMatch(url, pattern);
		}
	}
}
