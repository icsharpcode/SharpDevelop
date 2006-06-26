// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.SharpDevLanguageClass
{
	using System;
	using System.Collections.Generic;
	using ICSharpCode.Core;
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

		private static int DevLangCounter(IHxTopic topic)
		{
			try
			{
				int counter                      = 0;
				IHxAttributeList topicAttributes = topic.Attributes;
				foreach (IHxAttribute attr in topicAttributes)
				{
					if (String.Compare(attr.DisplayName, "DevLang") == 0)
					{
						counter++;
					}
				}
				return counter;
			}
			catch
			{
				return 0;
			}
		}

		public static bool CheckTopicLanguage(IHxTopic topic)
		{
			if (ProjectService.CurrentProject != null) {
				return CheckTopicLanguage(topic, ProjectService.CurrentProject.Language);
			} else {
				return true;
			}
		}

		public static bool CheckTopicLanguage(IHxTopic topic, string expectedLanguage)
		{
			if (string.IsNullOrEmpty(expectedLanguage)) { return true; }
			if (topic == null) { return false; }

			string tempLanguage = String.Empty;
			if (!languages.ContainsKey(expectedLanguage) ||
			    !languages.TryGetValue(expectedLanguage, out tempLanguage))
			{
				tempLanguage = expectedLanguage;
			}

			return (tempLanguage == String.Empty || topic.HasAttribute("DevLang", tempLanguage));
		}

		public static bool CheckUniqueTopicLanguage(IHxTopic topic)
		{
			return CheckUniqueTopicLanguage(topic, ProjectService.CurrentProject.Language);
		}

		public static bool CheckUniqueTopicLanguage(IHxTopic topic, string expectedLanguage)
		{
			return (CheckTopicLanguage(topic, expectedLanguage) && DevLangCounter(topic) == 1);
		}

		public static string GetPatchedLanguage()
		{
			if (ProjectService.CurrentProject == null)
				return GetPatchedLanguage(AmbienceService.DefaultAmbienceName);
			else
				return GetPatchedLanguage(ProjectService.CurrentProject.Language);
		}

		public static string GetPatchedLanguage(string expectedLanguage)
		{
			string tempLanguage = expectedLanguage;

			if (!string.IsNullOrEmpty(tempLanguage))
			{
				if (!languages.ContainsKey(expectedLanguage) ||
				    !languages.TryGetValue(expectedLanguage, out tempLanguage))
				{
					tempLanguage = expectedLanguage;
				}
			}

			return tempLanguage;
		}
	}
}
