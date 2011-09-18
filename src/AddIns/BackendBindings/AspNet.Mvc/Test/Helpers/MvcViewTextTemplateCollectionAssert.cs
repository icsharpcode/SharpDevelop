// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Helpers
{
	public static class MvcViewTextTemplateCollectionAssert
	{
		public static void AreEqual(IEnumerable<MvcViewTextTemplate> expected, IEnumerable<MvcViewTextTemplate> actual)
		{
			List<string> expectedAsStrings = ConvertToStrings(expected);
			List<string> actualAsStrings = ConvertToStrings(actual);
			
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		static List<string> ConvertToStrings(IEnumerable<MvcViewTextTemplate> templates)
		{
			var templatesAsStrings = new List<string>();
			foreach (MvcViewTextTemplate template in templates) {
				string templateAsString = ConvertToString(template);
				templatesAsStrings.Add(templateAsString);
			}
			return templatesAsStrings;
		}
		
		static string ConvertToString(MvcViewTextTemplate template)
		{
			return String.Format(
				"Name: {0}, FileName: {1}",
				template.Name, template.FileName);
		}
	}
}
