// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageService
	{
		static CodeCoverageResults results;
		static CodeCoverageHighlighter codeCoverageHighlighter = new CodeCoverageHighlighter();
		
		CodeCoverageService()
		{
		}
		
		static CodeCoverageService()
		{
			WorkbenchSingleton.Workbench.ViewOpened += ViewOpened;
		}
		
		/// <summary>
		/// Shows/hides the code coverage in the source code.
		/// </summary>
		public static bool CodeCoverageHighlighted {
			get {
				return CodeCoverageOptions.CodeCoverageHighlighted;
			}
			set {
				CodeCoveragePad pad = CodeCoveragePad.Instance;
				if (CodeCoverageOptions.CodeCoverageHighlighted != value) {
					CodeCoverageOptions.CodeCoverageHighlighted = value;
					if (CodeCoverageResultsExist) {
						if (value) {
							ShowCodeCoverage();
						} else {
							HideCodeCoverage();
						}
					}
				}
				if (pad != null) {
					pad.UpdateToolbar();
				}
			}
		}
		
		/// <summary>
		/// Gets the results from the last code coverage run.
		/// </summary>
		public static CodeCoverageResults Results {
			get {
				return results;
			}
		}
		
		/// <summary>
		/// Clears any code coverage results currently on display.
		/// </summary>
		public static void ClearResults()
		{
			CodeCoveragePad pad = CodeCoveragePad.Instance;
			if (pad != null) {
				pad.ClearCodeCoverageResults();
			}
			HideCodeCoverage();
			results = null;
		}
		
		/// <summary>
		/// Shows the code coverage results in the code coverage pad and
		/// highlights any source code files that have been profiled.
		/// </summary>
		public static void ShowResults(CodeCoverageResults results)
		{
			CodeCoverageService.results = results;
			CodeCoveragePad pad = CodeCoveragePad.Instance;
			if (pad != null) {
				pad.ShowResults(results);
			}
			if (results != null) {
				RefreshCodeCoverageHighlights();
			}
		}
		
		/// <summary>
		/// Updates the highlighted code coverage text to reflect any changes
		/// in the configured colours.
		/// </summary>
		public static void RefreshCodeCoverageHighlights()
		{
			if (CodeCoverageOptions.CodeCoverageHighlighted && CodeCoverageResultsExist) {
				HideCodeCoverage();
				ShowCodeCoverage();
			}
		}
		
		public static void ShowCodeCoverage(TextEditorControl textEditor, string fileName)
		{
			List<CodeCoverageSequencePoint> sequencePoints = results.GetSequencePoints(fileName);
			if (sequencePoints.Count > 0) {
				codeCoverageHighlighter.AddMarkers(textEditor.Document.MarkerStrategy, sequencePoints);
				textEditor.Refresh();
			}
		}
		
		static void ShowCodeCoverage()
		{
			// Highlight any open files.
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ShowCodeCoverage(view);
			}
		}
		
		static void HideCodeCoverage()
		{
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				TextEditorDisplayBindingWrapper textEditor = view as TextEditorDisplayBindingWrapper;
				if (textEditor != null) {
					codeCoverageHighlighter.RemoveMarkers(textEditor.textAreaControl.Document.MarkerStrategy);
					textEditor.textAreaControl.Refresh();
				}
			}
		}
		
		static void ViewOpened(object sender, ViewContentEventArgs e)
		{
			if (CodeCoverageOptions.CodeCoverageHighlighted && CodeCoverageResultsExist) {
				ShowCodeCoverage(e.Content);
			}
		}
		
		static void ShowCodeCoverage(IViewContent view)
		{
			TextEditorDisplayBindingWrapper displayBindingWrapper = view as TextEditorDisplayBindingWrapper;
			if (displayBindingWrapper != null && displayBindingWrapper.TextEditorControl != null && view.FileName != null) {
				ShowCodeCoverage(displayBindingWrapper.TextEditorControl, view.FileName);
			}
		}
		
		static bool CodeCoverageResultsExist {
			get {
				return results != null;
			}
		}
	}
}
