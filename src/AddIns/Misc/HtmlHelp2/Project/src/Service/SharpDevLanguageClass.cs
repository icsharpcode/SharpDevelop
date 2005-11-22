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

		SharpDevLanguage()
		{
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
			return CheckTopicLanguage(topic, ProjectService.CurrentProject.Language);
		}

		public static bool CheckTopicLanguage(IHxTopic topic, string expectedLanguage)
		{
			if (expectedLanguage == String.Empty) { return true; }
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

			if (tempLanguage != String.Empty)
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

