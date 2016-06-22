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
using System.Text;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.Utils
{
	public class TaskComparison
	{
		bool match;
		SDTask lhs;
		SDTask rhs;
		string shortMismatchReason = String.Empty;
		StringBuilder mismatchReason = new StringBuilder();
		
		public TaskComparison(SDTask lhs, SDTask rhs)
		{
			this.lhs = lhs;
			this.rhs = rhs;
			Compare();
		}
		
		void Compare()
		{
			match = false;
			
			if ((lhs == null) && (rhs == null)) {
				match = true;
				return;
			}
			
			if ((lhs == null) || (rhs == null)) {
				AddNullTaskMismatchReason();
			} else if (lhs.TaskType != rhs.TaskType) {
				AddTaskTypeMismatchReason();
			} else if (lhs.FileName != rhs.FileName) {
				AddFileNameMismatchReason();
			} else if (lhs.Description != rhs.Description) {
				AddDescriptionMismatchReason();
			} else if (lhs.Column != rhs.Column) {
				AddColumnMismatchReason();
			} else if (lhs.Line != rhs.Line) {
				AddLineMismatchReason();
			} else {
				match = true;
			}
		}
		
		public bool IsMatch {
			get { return match; }
		}
		
		public string MismatchReason {
			get { return mismatchReason.ToString(); }
		}
		
		void AddNullTaskMismatchReason()
		{
			shortMismatchReason = "One task is null.";
			AddMismatchedExpectedAndActualValues(lhs, rhs);
		}
		
		void AddTaskTypeMismatchReason()
		{
			shortMismatchReason = "TaskTypes are different.";
			AddMismatchedExpectedAndActualValues(lhs.TaskType, rhs.TaskType);
		}
		
		void AddMismatchedExpectedAndActualValues(object expected, object actual)
		{
			mismatchReason.AppendLine(shortMismatchReason);
			mismatchReason.AppendLine("Expected: " + ObjectToString(expected));
			mismatchReason.AppendLine("But was: " + ObjectToString(actual));
		}
		
		static string ObjectToString(object obj)
		{
			if (obj != null) {
				return obj.ToString();
			}
			return "(null)";
		}
		
		void AddFileNameMismatchReason()
		{
			shortMismatchReason = "FileNames are different.";
			AddMismatchedExpectedAndActualValues(lhs.FileName, rhs.FileName);
		}
		
		void AddDescriptionMismatchReason()
		{
			shortMismatchReason = "Descriptions are different.";
			AddMismatchedExpectedAndActualValues(lhs.Description, rhs.Description);
		}
		
		void AddColumnMismatchReason()
		{
			shortMismatchReason = "Columns are different.";
			AddMismatchedExpectedAndActualValues(lhs.Column, rhs.Column);
		}
		
		void AddLineMismatchReason()
		{
			shortMismatchReason = "Lines are different.";
			AddMismatchedExpectedAndActualValues(lhs.Line, rhs.Line);
		}
	}
}
