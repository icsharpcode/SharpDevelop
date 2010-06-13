// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestResult : TestResult
	{
		public NUnitTestResult(TestResult testResult)
			: base(testResult.Name)
		{
			Message = testResult.Message;
			ResultType = testResult.ResultType;
			StackTrace = testResult.StackTrace;
		}
		
		protected override void OnStackTraceChanged()
		{
			FileLineReference fileLineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(StackTrace, true);
			if (fileLineRef != null) {
				StackTraceFilePosition = CreateFilePosition(fileLineRef);
			} else {
				StackTraceFilePosition = FilePosition.Empty;
			}
		}
		
		FilePosition CreateFilePosition(FileLineReference fileLineRef)
		{
			string fileName = Path.GetFullPath(fileLineRef.FileName);
			return new FilePosition(fileName, fileLineRef.Line, fileLineRef.Column + 1);
		}
	}
}
