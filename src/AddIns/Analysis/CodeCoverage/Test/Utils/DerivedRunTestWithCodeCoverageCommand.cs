// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.CodeCoverage;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class DerivedRunTestWithCodeCoverageCommand : RunTestWithCodeCoverageCommand
	{	
		public string ParsedStringToReturn = String.Empty;
		public string ParsedString;
		
		public DerivedRunTestWithCodeCoverageCommand(IRunTestCommandContext context, 
			ICodeCoverageTestRunnerFactory factory)
			: base(context, factory)
		{
		}
		
		public void CallOnBeforeRunTests()
		{
			base.OnBeforeRunTests();
		}
		
		public void CallOnAfterRunTests()
		{
			base.OnAfterRunTests();
		}
		
		protected override MessageViewCategory CreateMessageViewCategory(string category, string displayCategory)
		{
			return new MessageViewCategory(category, displayCategory);
		}
		
		public MessageViewCategory CodeCoverageMessageViewCategory {
			get { return base.Category; }
			set { base.Category = value;}
		}
		
		protected override string StringParse(string text)
		{
			ParsedString = text;
			return ParsedStringToReturn;
		}
		
		public void CallTestRunnerMessageReceived(object source, MessageReceivedEventArgs e)
		{
			base.TestRunnerMessageReceived(source, e);
		}
		
		public ITestRunner CallCreateTestRunner(IProject project)
		{
			return base.CreateTestRunner(project);
		}
	}
}
