/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * SharpDevLanguage Class
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2.SharpDevLanguageClass
{
	using System;
	using System.Collections.Generic;
	using ICSharpCode.SharpDevelop.Project;
	using MSHelpServices;

	public sealed class SharpDevLanguage
	{
		private static Dictionary<string, string>languages = new Dictionary<string, string>();

		static SharpDevLanguage()
		{
			languages.Add("C#", "CSharp");
			languages.Add("VBNet", "VB");
		}

		SharpDevLanguage()
		{
		}

		public static bool CheckTopicLanguage(IHxTopic topic)
		{
			return CheckTopicLanguage(topic, ProjectService.CurrentProject.Language);
		}

		public static bool CheckTopicLanguage(IHxTopic topic, string expectedLanguage)
		{
			if(expectedLanguage == String.Empty) { return true; }
			if(topic == null) { return false; }

			string tempLanguage = String.Empty;
			if(!languages.ContainsKey(expectedLanguage) ||
			   !languages.TryGetValue(expectedLanguage, out tempLanguage))
			{
				tempLanguage = expectedLanguage;
			}

			return (tempLanguage == String.Empty || topic.HasAttribute("DevLang", tempLanguage));
		}

		public static string GetPatchedLanguage()
		{
			return GetPatchedLanguage(ProjectService.CurrentProject.Language);
		}

		public static string GetPatchedLanguage(string expectedLanguage)
		{
			string tempLanguage = expectedLanguage;

			if(tempLanguage != String.Empty)
			{
				if(!languages.ContainsKey(expectedLanguage) ||
				   !languages.TryGetValue(expectedLanguage, out tempLanguage))
				{
					tempLanguage = expectedLanguage;
				}
			}

			return tempLanguage;
		}
	}
}

