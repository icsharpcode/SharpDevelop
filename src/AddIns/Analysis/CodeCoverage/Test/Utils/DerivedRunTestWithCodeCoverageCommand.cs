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

//using System;
//using ICSharpCode.CodeCoverage;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.UnitTesting;
//
//namespace ICSharpCode.CodeCoverage.Tests.Utils
//{
//	public class DerivedRunTestWithCodeCoverageCommand : RunTestWithCodeCoverageCommand
//	{	
//		public string ParsedStringToReturn = String.Empty;
//		public string ParsedString;
//		
//		public DerivedRunTestWithCodeCoverageCommand(IRunTestCommandContext context, 
//			ICodeCoverageTestRunnerFactory factory)
//			: base(context, factory)
//		{
//		}
//		
//		public void CallOnBeforeRunTests()
//		{
//			base.OnBeforeRunTests();
//		}
//		
//		public void CallOnAfterRunTests()
//		{
//			base.OnAfterRunTests();
//		}
//		
//		protected override MessageViewCategory CreateMessageViewCategory(string category, string displayCategory)
//		{
//			return new MessageViewCategory(category, displayCategory);
//		}
//		
//		public MessageViewCategory CodeCoverageMessageViewCategory {
//			get { return base.Category; }
//			set { base.Category = value;}
//		}
//		
//		protected override string StringParse(string text)
//		{
//			ParsedString = text;
//			return ParsedStringToReturn;
//		}
//		
//		public void CallTestRunnerMessageReceived(object source, MessageReceivedEventArgs e)
//		{
//			base.TestRunnerMessageReceived(source, e);
//		}
//		
//		public ITestRunner CallCreateTestRunner(IProject project)
//		{
//			return base.CreateTestRunner(project);
//		}
//	}
//}
