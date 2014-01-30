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
