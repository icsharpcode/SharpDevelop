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
using System.Xml;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestResults : List<TestResult>
	{
		Dictionary<string, MSTestResult> testDefinitions = new Dictionary<string, MSTestResult>();
		
		public MSTestResults(string fileName)
			: this(new XmlTextReader(fileName))
		{
		}
		
		public MSTestResults(XmlTextReader reader)
		{
			ReadResults(reader);
		}
		
		void ReadResults(XmlTextReader reader)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						if (reader.Name == "UnitTest") {
							ReadUnitTest(reader);
						} else if (reader.Name == "UnitTestResult") {
							ReadUnitTestResult(reader);
						}
						break;
				}
			}
		}
		
		void ReadUnitTest(XmlTextReader reader)
		{
			var testResult = new MSTestResult(reader.GetAttribute("id"));
			testDefinitions.Add(testResult.Id, testResult);
			
			if (reader.ReadToDescendant("TestMethod")) {
				testResult.UpdateTestName(reader.GetAttribute("className"), reader.GetAttribute("name"));
			}
		}
		
		void ReadUnitTestResult(XmlTextReader reader)
		{
			string testId = reader.GetAttribute("testId");
			MSTestResult testResult = FindTestResult(testId);
			if (testResult != null) {
				testResult.UpdateResult(reader.GetAttribute("outcome"));
				if (testResult.IsError) {
					ReadErrorInformation(testResult, reader);
				}
				Add(testResult.ToTestResult());
			}
		}
		
		void ReadErrorInformation(MSTestResult testResult, XmlTextReader reader)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.Name == "UnitTestResult") {
							return;
						}
						break;
					case XmlNodeType.Element:
						if (reader.Name == "Message") {
							testResult.Message = reader.ReadElementContentAsString();
						} else if (reader.Name == "StackTrace") {
							testResult.StackTrace = reader.ReadElementContentAsString();
						}
						break;
				}
			}
		}
		
		MSTestResult FindTestResult(string testId)
		{
			MSTestResult testResult = null;
			testDefinitions.TryGetValue(testId, out testResult);
			return testResult;
		}
	}
}
