// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace Gallio.SharpDevelop
{
	public class GallioTestResult : TestResult
	{
		public GallioTestResult(TestResult testResult)
			: base(testResult.Name)
		{
			Message = testResult.Message;
			ResultType = testResult.ResultType;
			StackTrace = testResult.StackTrace;
		}
		
		protected override void OnStackTraceChanged()
		{
			GetFilePositionFromStackTrace();
		}
		
		/// <summary>
		/// Stack trace: 
		/// at GallioTest.MyClass.AssertWithFailureMessage() in d:\temp\test\..\GallioTest\MyClass.cs:line 46 
		/// </summary>
		void GetFilePositionFromStackTrace()
		{
			Match match = Regex.Match(StackTrace, @"\sin\s(.*?):line\s(\d+)", RegexOptions.Multiline);
			if (match.Success) {
				SetStackTraceFilePosition(match.Groups);
			} else {
				StackTraceFilePosition = FilePosition.Empty;
			}
		}
		
		void SetStackTraceFilePosition(GroupCollection groups)
		{
			string fileName = Path.GetFullPath(groups[1].Value);
			int line = Convert.ToInt32(groups[2].Value);
			int column = 1;
			
			StackTraceFilePosition = new FilePosition(fileName, line, column);
		}
	}
}
