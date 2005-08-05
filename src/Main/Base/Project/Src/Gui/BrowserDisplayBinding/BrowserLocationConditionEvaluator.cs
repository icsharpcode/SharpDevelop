/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 05.08.2005
 * Time: 19:37
 */

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	/// <summary>
	/// Description of BrowserLocationConditionEvaluator.
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
