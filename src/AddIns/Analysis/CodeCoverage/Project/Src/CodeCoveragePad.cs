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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoveragePad : AbstractPadContent
	{
		static CodeCoveragePad instance;
		bool disposed;
		CodeCoverageControl codeCoverageControl;
		
		public CodeCoveragePad()
		{
			instance = this;
			
			codeCoverageControl = new CodeCoverageControl();
			codeCoverageControl.UpdateToolbar();
					
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.SolutionLoaded += SolutionLoaded;
			
			ShowSourceCodePanel = CodeCoverageOptions.ShowSourceCodePanel;
			ShowVisitCountPanel = CodeCoverageOptions.ShowVisitCountPanel;
		}
		
		public static CodeCoveragePad Instance {
			get {
				return instance;
			}
		}

		public override object Control {
			get {
				return codeCoverageControl;
			}
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				ProjectService.SolutionClosed -= SolutionClosed;
				ProjectService.SolutionLoaded -= SolutionLoaded;
				codeCoverageControl.Dispose();
			}
		}
		
		public void UpdateToolbar()
		{
			codeCoverageControl.UpdateToolbar();
		}
		
		public void ShowResults(CodeCoverageResults results)
		{
			if (results != null) {
				codeCoverageControl.AddModules(results.Modules);
			}
		}
		
		public void ClearCodeCoverageResults()
		{
			codeCoverageControl.Clear();
		}
		
		public bool ShowSourceCodePanel {
			get {
				return codeCoverageControl.ShowSourceCodePanel;
			}
			set {
				codeCoverageControl.ShowSourceCodePanel = value;
			}
		}
		
		public bool ShowVisitCountPanel {
			get {
				return codeCoverageControl.ShowVisitCountPanel;
			}
			set {
				codeCoverageControl.ShowVisitCountPanel = value;
			}
		}
		
		void SolutionLoaded(object sender, EventArgs e)
		{
			codeCoverageControl.UpdateToolbar();
		}
		
		void SolutionClosed(object sender, EventArgs e)
		{
			ClearCodeCoverageResults();
			codeCoverageControl.UpdateToolbar();
			ClearCodeCoverageResults();
		}
	}
}
