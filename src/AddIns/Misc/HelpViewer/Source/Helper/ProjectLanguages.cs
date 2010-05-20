using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace MSHelpSystem.Helper
{
	internal sealed class ProjectLanguages
	{
		ProjectLanguages()
		{
		}

		#region Supported project languages

		static Dictionary<string, string> languages = InitializeLanguages();

		static Dictionary<string, string> InitializeLanguages()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("C++", "C%2B%2B");
			result.Add("C#", "CSharp");
			result.Add("VBNet", "VB");
			return result;
		}

		#endregion

		public static string Formatted()
		{
			string output = string.Empty;
			if (ProjectService.CurrentProject != null) {
				string devLang = ProjectService.CurrentProject.Language;
				if (string.IsNullOrEmpty(devLang)) { throw new ArgumentNullException("devLang"); }
				output = devLang;

				if (!languages.ContainsKey(devLang) || !languages.TryGetValue(devLang, out output)) {
					output = devLang;
				}
				LoggingService.Debug(string.Format("Help 3.0: Project language \"{0}\" formatted to \"{1}\"", devLang, output));
			}
			return output.ToLower();
		}

		public static string FormattedAsHttpParam()
		{
			string devLang = Formatted();
			if (string.IsNullOrEmpty(devLang)) return string.Empty;
			else return string.Format("&category=DevLang%3a{0}", devLang);
		}
	}
}
