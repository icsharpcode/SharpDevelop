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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Xml;

using ICSharpCode.WpfDesign.XamlDom;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	public class TestHelper
	{
		public static T[] ToArray<T>(IEnumerable<T> e)
		{
			return new List<T>(e).ToArray();
		}
		
		public static void TestLoading(string xaml)
		{
			Debug.WriteLine("Load using builtin XamlReader:");
			ExampleClass.nextUniqueIndex = 0;
			TestHelperLog.logBuilder = new StringBuilder();
			object officialResult = XamlReader.Load(new XmlTextReader(new StringReader(xaml)));
			string officialLog = TestHelperLog.logBuilder.ToString();
			Assert.IsNotNull(officialResult, "officialResult is null");
			
			Debug.WriteLine("Load using own XamlParser:");
			ExampleClass.nextUniqueIndex = 0;
			TestHelperLog.logBuilder = new StringBuilder();
			XamlDocument doc = XamlParser.Parse(new StringReader(xaml));
			Assert.IsNotNull(doc, "doc is null");
			object ownResult = doc.RootInstance;
			string ownLog = TestHelperLog.logBuilder.ToString();
			Assert.IsNotNull(ownResult, "ownResult is null");
			
			TestHelperLog.logBuilder = null;
			// compare:
			string officialSaved = XamlWriter.Save(officialResult);
			string ownSaved = XamlWriter.Save(ownResult);
			
			Debug.WriteLine("Official saved:");
			Debug.WriteLine(officialSaved);
			Debug.WriteLine("Own saved:");
			Debug.WriteLine(ownSaved);
			
			Assert.AreEqual(officialSaved, ownSaved);
			
			Debug.WriteLine("Official log:");
			Debug.WriteLine(officialLog);
			Debug.WriteLine("Own log:");
			Debug.WriteLine(ownLog);
			
			// compare logs:
			Assert.AreEqual(officialLog, ownLog);
		}
	}
	
	internal static class TestHelperLog
	{
		internal static StringBuilder logBuilder;
		
		internal static void Log(string text)
		{
			if (logBuilder != null) {
				logBuilder.AppendLine(text);
				Debug.WriteLine(text);
			}
		}
	}
}
