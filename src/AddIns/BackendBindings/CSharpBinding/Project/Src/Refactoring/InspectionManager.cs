// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding.Refactoring
{
	/*
	/// <summary>
	/// Performs code analysis in the background and creates text markers to show warnings.
	/// </summary>
	public class InspectionManager : IDisposable
	{
		static readonly Lazy<IList<IInspector>> inspectors = new Lazy<IList<IInspector>>(
			() => AddInTree.BuildItems<IInspector>("/SharpDevelop/ViewContent/TextEditor/C#/Inspectors", null, false));
		readonly ITextEditor editor;
		readonly ITextMarkerService markerService;
		
		public InspectionManager(ITextEditor editor)
		{
			this.editor = editor;
			this.markerService = editor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			ParserService.ParserUpdateStepFinished += ParserService_ParserUpdateStepFinished;
		}
		
		public void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= ParserService_ParserUpdateStepFinished;
			if (cancellationTokenSource != null)
				cancellationTokenSource.Cancel();
			Clear();
		}
		
		sealed class InspectionTag
		{
			public readonly IInspector Inspector;
			public readonly string Title;
			public readonly int StartOffset;
			public readonly int EndOffset;
			public readonly bool CanFix;
			
			public InspectionTag(IInspector inspector, string title, int startOffset, int endOffset, bool canFix)
			{
				this.Inspector = inspector;
				this.Title = title;
				this.StartOffset = startOffset;
				this.EndOffset = endOffset;
				this.CanFix = canFix;
			}
			
			ITextMarker marker;
			
			public void CreateMarker(ITextSourceVersion inspectedVersion, IDocument document, ITextMarkerService markerService)
			{
				int startOffset = inspectedVersion.MoveOffsetTo(document.Version, this.StartOffset, AnchorMovementType.Default);
				int endOffset = inspectedVersion.MoveOffsetTo(document.Version, this.EndOffset, AnchorMovementType.Default);
				if (startOffset >= endOffset)
					return;
				marker = markerService.Create(startOffset, endOffset - startOffset);
				marker.ToolTip = this.Title;
				marker.MarkerType = TextMarkerType.SquigglyUnderline;
				marker.MarkerColor = Colors.Blue;
				marker.Tag = this;
			}
			
			public void RemoveMarker()
			{
				if (marker != null) {
					marker.Delete();
					marker = null;
				}
			}
		}
		
		CancellationTokenSource cancellationTokenSource;
		ITextSourceVersion analyzedVersion;
		List<InspectionTag> existingResults;
		
		void Clear()
		{
			if (existingResults != null) {
				foreach (var oldResult in existingResults) {
					oldResult.RemoveMarker();
				}
				existingResults = null;
			}
			analyzedVersion = null;
		}
		
		void ParserService_ParserUpdateStepFinished(object sender, ParserUpdateStepEventArgs e)
		{
			var parseInfo = e.ParseInformation as CSharpFullParseInformation;
			ITextSourceVersion currentVersion  = editor.Document.Version;
			ITextSourceVersion parsedVersion = e.Content.Version;
			if (parseInfo != null && parsedVersion != null && currentVersion != null && parsedVersion.BelongsToSameDocumentAs(currentVersion)) {
				if (analyzedVersion != null && analyzedVersion.CompareAge(parsedVersion) == 0) {
					// don't analyze the same version twice
					return;
				}
				RunAnalysis(e.Content, parseInfo);
			}
		}
		
		async void RunAnalysis(ITextSource textSource, CSharpFullParseInformation parseInfo)
		{
			if (markerService == null)
				return;
			if (cancellationTokenSource != null)
				cancellationTokenSource.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;
			List<InspectionTag> results = new List<InspectionTag>();
			try {
				await Task.Run(
					delegate {
						var compilation = ParserService.GetCompilationForFile(parseInfo.FileName);
						var resolver = parseInfo.GetResolver(compilation);
						var context = new SDRefactoringContext(textSource, resolver, new TextLocation(0, 0), 0, 0, cancellationToken);
						foreach (var inspector in inspectors.Value) {
							foreach (var issue in inspector.Run(context)) {
								results.Add(new InspectionTag(
									inspector,
									issue.Title,
									context.GetOffset(issue.Start),
									context.GetOffset(issue.End),
									issue.Fix != null));
							}
						}
					}, cancellationToken);
			} catch (TaskCanceledException) {
			}
			if (!cancellationToken.IsCancellationRequested) {
				analyzedVersion = textSource.Version;
				Clear();
				foreach (var newResult in results) {
					newResult.CreateMarker(textSource.Version, editor.Document, markerService);
				}
				existingResults = results;
			}
			cancellationTokenSource.Dispose();
			cancellationTokenSource = null;
		}
	}
	*/
}
