// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Menu command selected after right clicking a test method in the text editor
	/// to run tests with code coverage.
	/// </summary>
	public class RunTestWithCodeCoverageCommand : AbstractRunTestCommand
	{
		static MessageViewCategory category;
		ICodeCoverageTestRunnerFactory factory;
		CodeCoverageTestRunner codeCoverageTestRunner;
		
		public RunTestWithCodeCoverageCommand()
			: this(new RunTestCommandContext(), new CodeCoverageTestRunnerFactory())
		{
		}
		
		public RunTestWithCodeCoverageCommand(IRunTestCommandContext context,
			ICodeCoverageTestRunnerFactory factory)
			: base(context)
		{
			this.factory = factory;
		}
		
		protected override void OnBeforeRunTests()
		{
			ClearCodeCoverageResults();
		}
		
		void ClearCodeCoverageResults()
		{
			Category.ClearText();
			Context.Workbench.SafeThreadAsyncCall(CodeCoverageService.ClearResults);
		}
		
		/// <summary>
		/// Gets the message view output window.
		/// </summary>
		protected MessageViewCategory Category {
			get {
				if (category == null) {
					CreateCodeCoverageMessageViewCategory();
				}
				return category;
			}
			set { category = value; }
		}
		
		void CreateCodeCoverageMessageViewCategory()
		{
			string displayCategory = StringParse("${res:ICSharpCode.UnitTesting.CodeCoverage}");
			category = CreateMessageViewCategory("CodeCoverage", displayCategory);
		}
		
		protected virtual string StringParse(string text)
		{
			return StringParser.Parse(text);
		}
		
		protected virtual MessageViewCategory CreateMessageViewCategory(string category, string displayCategory)
		{
			MessageViewCategory view = null;
			MessageViewCategory.Create(ref view, category, displayCategory);
			return view;
		}
		
		protected override void OnAfterRunTests()
		{
			ShowCodeCoverageResultsIfNoCriticalTestFailures();
		}
		
		void ShowCodeCoverageResultsIfNoCriticalTestFailures()
		{
			if (!Context.TaskService.HasCriticalErrors(false)) {
				ShowPad(Context.Workbench.GetPad(typeof(CodeCoveragePad)));
			}
		}
		
		protected override void TestRunnerMessageReceived(object source, MessageReceivedEventArgs e)
		{
			Category.AppendLine(e.Message);
		}

		protected override ITestRunner CreateTestRunner(IProject project)
		{
			codeCoverageTestRunner = factory.CreateCodeCoverageTestRunner();
			codeCoverageTestRunner.AllTestsFinished += CodeCoverageRunFinished;
			return codeCoverageTestRunner;
		}
		
		void CodeCoverageRunFinished(object source, EventArgs e)
		{
			if (codeCoverageTestRunner.HasCodeCoverageResults()) {
				CodeCoverageResults results = codeCoverageTestRunner.ReadCodeCoverageResults();
				DisplayCodeCoverageResults(results);
			} else {
				DisplayNoCodeCoverageResultsGeneratedMessage();
			}
		}
		
		void DisplayCodeCoverageResults(CodeCoverageResults results)
		{
			Context.Workbench.SafeThreadAsyncCall(CodeCoverageService.ShowResults, results);
		}
		
		void DisplayNoCodeCoverageResultsGeneratedMessage()
		{
			Task task = CreateNoCodeCoverageResultsGeneratedTask();
			Context.Workbench.SafeThreadAsyncCall(Context.TaskService.Add, task);		
		}
		
		Task CreateNoCodeCoverageResultsGeneratedTask()
		{
			string description = GetNoCodeCoverageResultsGeneratedTaskDescription();
			return new Task(null, description, 1, 1, TaskType.Error);
		}
		
		string GetNoCodeCoverageResultsGeneratedTaskDescription()
		{
			string message = StringParse("${res:ICSharpCode.CodeCoverage.NoCodeCoverageResultsGenerated}");
			return String.Format("{0} {1}", message, codeCoverageTestRunner.CodeCoverageResultsFileName);
		}
	}
}
