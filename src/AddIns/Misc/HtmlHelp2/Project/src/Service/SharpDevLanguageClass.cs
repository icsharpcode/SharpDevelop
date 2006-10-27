// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Collections.Generic;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Project;
	using MSHelpServices;

	public sealed class SharpDevLanguage
	{
		static Dictionary<string, string> languages = InitializeLanguages();

		static Dictionary<string, string> InitializeLanguages()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("C#", "CSharp");
			result.Add("VBNet", "VB");

			return result;
		}

		SharpDevLanguage()
		{
		}

		private static int DevLangCounter(IHxTopic topic)
		{
			if (topic == null)
			{
				return 0;
			}

			int counter                      = 0;
			IHxAttributeList topicAttributes = topic.Attributes;
			if (topicAttributes == null || topicAttributes.Count == 0)
			{
				return 0;
			}

			foreach (IHxAttribute attr in topicAttributes)
			{
				if (String.Compare(attr.DisplayName, "DevLang") == 0)
				{
					counter++;
				}
			}
			return counter;
		}

		public static bool CheckTopicLanguage(IHxTopic topic)
		{
			if (ProjectService.CurrentProject != null)
			{
				return CheckTopicLanguage(topic, ProjectService.CurrentProject.Language);
			}
			else
			{
				return true;
			}
		}

		public static bool CheckTopicLanguage(IHxTopic topic, string expectedLanguage)
		{
			if (string.IsNullOrEmpty(expectedLanguage))
			{
				return true;
			}
			if (topic == null)
			{
				return false;
			}

			string tempLanguage = String.Empty;
			if (!languages.ContainsKey(expectedLanguage) ||
			    !languages.TryGetValue(expectedLanguage, out tempLanguage))
			{
				tempLanguage = expectedLanguage;
			}

			return (string.IsNullOrEmpty(tempLanguage) || topic.HasAttribute("DevLang", tempLanguage));
		}

		public static bool CheckUniqueTopicLanguage(IHxTopic topic)
		{
			if (ProjectService.CurrentProject != null)
			{
				return CheckUniqueTopicLanguage(topic, ProjectService.CurrentProject.Language);
			}
			else
			{
				return CheckUniqueTopicLanguage(topic, string.Empty);
			}
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
