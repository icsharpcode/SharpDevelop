// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageService
	{
		static List<CodeCoverageResults> results = new List<CodeCoverageResults>();
		static CodeCoverageHighlighter codeCoverageHighlighter = new CodeCoverageHighlighter();
		
		CodeCoverageService()
		{
		}
		
		static CodeCoverageService()
		{
			WorkbenchSingleton.Workbench.ViewOpened += ViewOpened;
			ProjectService.SolutionLoaded += SolutionLoaded;
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
		public static CodeCoverageResults[] Results {
			get {
				return results.ToArray();
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
			results.Clear();
		}
		
		/// <summary>
		/// Shows the code coverage results in the code coverage pad and
		/// highlights any source code files that have been profiled.
		/// </summary>
		public static void ShowResults(CodeCoverageResults results)
		{
			CodeCoverageService.results.Add(results);
			CodeCoveragePad pad = CodeCoveragePad.Instance;
			if (pad != null) {
				pad.ShowResults(results);
			}
			RefreshCodeCoverageHighlights();
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
		
		public static void ShowCodeCoverage(ITextEditor textEditor, string fileName)
		{
			foreach (CodeCoverageResults results in CodeCoverageService.Results) {
				List<CodeCoverageSequencePoint> sequencePoints = results.GetSequencePoints(fileName);
				if (sequencePoints.Count > 0) {
					codeCoverageHighlighter.AddMarkers(textEditor.Document, sequencePoints);
				}
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
				ITextEditorProvider editorProvider = view as ITextEditorProvider;
				if (editorProvider != null) {
					codeCoverageHighlighter.RemoveMarkers(editorProvider.TextEditor.Document);
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
			ITextEditorProvider editorProvider = view as ITextEditorProvider;
			if (editorProvider != null && view.PrimaryFileName != null) {
				ShowCodeCoverage(editorProvider.TextEditor, view.PrimaryFileName);
			}
		}
		
		static bool CodeCoverageResultsExist {
			get {
				return results.Count > 0;
			}
		}
		
		static void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			var solutionCodeCoverageResults = new SolutionCodeCoverageResults(e.Solution);
			foreach (CodeCoverageResults results in solutionCodeCoverageResults.GetCodeCoverageResultsForAllProjects()) {
				ShowResults(results);
			}
		}
	}
}
