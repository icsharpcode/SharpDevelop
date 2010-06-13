// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;

namespace Gallio.Extension
{
	public class TestResultsWriter : ITestResultsWriter, IDisposable
	{
		TextWriter writer;
		
		public TestResultsWriter(string fileName)
			: this(new StreamWriter(fileName, false, Encoding.UTF8))
		{
		}
		
		public TestResultsWriter(TextWriter writer)
		{
			this.writer = writer;
		}
		
		public void Write(TestResult testResult)
		{
			WriteTestName(testResult.Name);
			WriteResult(testResult.ResultType);
			
			if (testResult.ResultType == TestResultType.Failure) {
				WriteTestFailureResult(testResult);
			}
		}
		
		void WriteTestName(string name)
		{
			WriteNameAndValue("Name", name);
		}
		
		void WriteResult(TestResultType resultType)
		{
			string result = GetResult(resultType);
			WriteNameAndValue("Result", result);
		}
		
		void WriteTestFailureResult(TestResult testResult)
		{
			WriteNameAndMultiLineValue("Message", testResult.Message);
			WriteNameAndMultiLineValue("StackTrace", testResult.StackTrace);
		}
		
		void WriteNameAndValue(string name, string value)
		{
			string nameAndValue = String.Format("{0}: {1}", name, value);
			WriteLine(nameAndValue);
		}
		
		string GetResult(TestResultType resultType)
		{
			switch (resultType) {
				case TestResultType.Success:
					return "Success";
				case TestResultType.Ignored:
					return "Ignored";
				case TestResultType.Failure:
					return "Failure";
			}
			return String.Empty;
		}
		
		void WriteNameAndMultiLineValue(string name, string value)
		{
			MultiLineTestResultText multiLineText = new MultiLineTestResultText(value);
			WriteNameAndValue(name, multiLineText.ToString());
		}
		
		void WriteLine(string text)
		{
			writer.WriteLine(text);
		}
		
		public void Dispose()
		{
			writer.Dispose();
		}
	}
}
