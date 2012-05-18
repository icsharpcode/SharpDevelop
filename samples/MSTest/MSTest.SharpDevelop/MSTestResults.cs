// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
