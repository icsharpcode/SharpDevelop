// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcControllerTextTemplateCollectionAssert
	{
		public static void AreEqual(IEnumerable<MvcControllerTextTemplate> expected, IEnumerable<MvcControllerTextTemplate> actual)
		{
			List<string> expectedAsStrings = ConvertToStrings(expected);
			List<string> actualAsStrings = ConvertToStrings(actual);
			
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		static List<string> ConvertToStrings(IEnumerable<MvcControllerTextTemplate> templates)
		{
			var templatesAsStrings = new List<string>();
			foreach (MvcControllerTextTemplate template in templates) {
				string templateAsString = ConvertToString(template);
				templatesAsStrings.Add(templateAsString);
			}
			return templatesAsStrings;
		}
		
		static string ConvertToString(MvcControllerTextTemplate template)
		{
			return String.Format(
				"Name: {0}, Description: {1}, AddActionMethods: {2}, FileName: {3}",
				template.Name, template.Description, template.AddActionMethods, template.FileName);
		}
	}
}
