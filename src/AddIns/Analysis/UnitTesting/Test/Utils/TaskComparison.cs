// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.Utils
{
	public class TaskComparison
	{
		bool match;
		Task lhs;
		Task rhs;
		string shortMismatchReason = String.Empty;
		StringBuilder mismatchReason = new StringBuilder();
		
		public TaskComparison(Task lhs, Task rhs)
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
