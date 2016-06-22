// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
