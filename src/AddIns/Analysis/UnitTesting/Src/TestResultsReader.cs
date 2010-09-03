// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Reads the test results file produced by the custom
	/// nunit-console application.
	/// </summary>
	public class TestResultsReader
	{
		StringBuilder nameBuilder = new StringBuilder();
		StringBuilder valueBuilder = new StringBuilder();
		bool firstNameChar = true;
		TestResult result;
		
		enum State {
			WaitingForEndOfName     = 0,
			WaitingForStartOfValue  = 1,
			WaitingForEndOfValue    = 2
		}
		
		State state = State.WaitingForEndOfName;
		
		public TestResultsReader()
		{
		}
		
		/// <summary>
		/// Returns any TestResults that are in the text.
		/// </summary>
		/// <param name="text">The text read in from the
		/// TestResults file.</param>
		public TestResult[] Read(string text)
		{
			List<TestResult> results = new List<TestResult>();
			foreach (char ch in text) {
				if (ReadNameValuePair(ch)) {
					if (ReadTestResult()) {
						results.Add(result);
					}
				}
			}
			return results.ToArray();
		}
		
		/// <summary>
		/// Reads a name-value pair of the form:
		/// 
		/// Name: Value
		/// 
		/// A value can span multiple lines:
		/// 
		/// Name1: ValueLine1
		/// {SP}ValueLine2
		/// {SP}ValueLine3
		/// Name1: Value2
		/// 
		/// Each continued line of the value must start with
		/// a single space character to distinguish it from
		/// the start of a new name-value pair.
		/// </summary>
		/// <returns>True if a name-value pair has been
		/// successfully read in</returns>
		bool ReadNameValuePair(char ch)
		{
			if (state == State.WaitingForEndOfName) {
				ReadNameChar(ch);
			} else if (state == State.WaitingForStartOfValue) {
				// Makes sure first space is ignored.
				state = State.WaitingForEndOfValue;
			} else {
				return ReadValueChar(ch);
			}
			return false;
		}
		
		void ReadNameChar(char ch)
		{
			if (ch == ':') {
				state = State.WaitingForStartOfValue;
			} else if (ch == '\r' || ch == '\n') {
				nameBuilder = new StringBuilder();
			} else if (ch == ' ' && firstNameChar) {
				state = State.WaitingForEndOfValue;
				valueBuilder.Append("\r\n");
			} else if (firstNameChar) {
				firstNameChar = false;
				nameBuilder = new StringBuilder();
				valueBuilder = new StringBuilder();
				nameBuilder.Append(ch);
			} else {
				nameBuilder.Append(ch);
			}
		}
		
		bool ReadValueChar(char ch)
		{
			if (ch == '\r') {
				// Ignore.
			} else if (ch == '\n') {
				state = State.WaitingForEndOfName;
				firstNameChar = true;
				return true;
			} else {
				valueBuilder.Append(ch);
			}
			return false;
		}
		
		/// <summary>
		/// Creates and updates a TestResult based on the
		/// name-value pair read in,
		/// </summary>
		/// <returns>True if a TestResult is ready to be returned
		/// to the caller.</returns>
		/// <remarks>
		/// The first name-value pair for a test result is the 
		/// test name. The last name-value pair is the result of 
		/// the test (Success, Failure or Ignored).</remarks>
		bool ReadTestResult()
		{
			string name = nameBuilder.ToString();
			if (name == "Name") {
				result = new TestResult(valueBuilder.ToString());
			} else if (result != null) {
				if (name == "Message") {
					result.Message = valueBuilder.ToString();
				} else if (name == "StackTrace") {
					result.StackTrace = valueBuilder.ToString();
				} else if (name == "Result") {
					UpdateTestSuccess();
					return true;
				}
			}
			return false;
		}
		
		void UpdateTestSuccess()
		{
			string value = valueBuilder.ToString();
			if (value == "Success") {
				result.ResultType = TestResultType.Success;
			} else if (value == "Failure") {
				result.ResultType = TestResultType.Failure;
			} else {
				result.ResultType = TestResultType.Ignored;
			}
		}
	}
}
