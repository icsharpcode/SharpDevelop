// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
			if (CodeCoverageService.Results != null) {
				codeCoverageControl.AddModules(CodeCoverageService.Results.Modules);
			}
			codeCoverageControl.UpdateToolbar();
					
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.SolutionLoaded += SolutionLoaded;
		}
		
		public static CodeCoveragePad Instance {
			get {
				return instance;
			}
		}

		public override Control Control {
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
			codeCoverageControl.Clear();
			if (results != null) {
				codeCoverageControl.AddModules(results.Modules);
			}
		}
		
		public void ClearCodeCoverageResults()
		{
			codeCoverageControl.Clear();
		}
		
		void SolutionLoaded(object sender, EventArgs e)
		{
			codeCoverageControl.UpdateToolbar();
		}
		
		void SolutionClosed(object sender, EventArgs e)
		{
			codeCoverageControl.UpdateToolbar();
			ClearCodeCoverageResults();
		}
	}
}
