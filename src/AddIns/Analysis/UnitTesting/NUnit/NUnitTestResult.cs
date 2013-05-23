// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;
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
				StackTraceFilePosition = DomRegion.Empty;
			}
		}
		
		DomRegion CreateFilePosition(FileLineReference fileLineRef)
		{
			string fileName = Path.GetFullPath(fileLineRef.FileName);
			return new DomRegion(fileName, fileLineRef.Line, fileLineRef.Column + 1);
		}
	}
}
