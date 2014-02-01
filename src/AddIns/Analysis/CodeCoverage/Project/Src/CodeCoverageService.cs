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
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

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
			SD.Workbench.ViewOpened += ViewOpened;
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
			foreach (IViewContent view in SD.Workbench.ViewContentCollection) {
				ShowCodeCoverage(view);
			}
		}
		
		static void HideCodeCoverage()
		{
			foreach (IViewContent view in SD.Workbench.ViewContentCollection) {
				ITextEditor textEditor = view.GetService<ITextEditor>();
				if (textEditor != null) {
					codeCoverageHighlighter.RemoveMarkers(textEditor.Document);
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
			ITextEditor textEditor = view.GetService<ITextEditor>();
			if (textEditor != null && view.PrimaryFileName != null) {
				ShowCodeCoverage(textEditor, view.PrimaryFileName);
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
