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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.NRefactory.Refactoring;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Performs code analysis in the background and creates text markers to show warnings.
	/// </summary>
	public class IssueManager : IDisposable, IContextActionProvider
	{
		static readonly Lazy<IReadOnlyList<IssueProvider>> issueProviders = new Lazy<IReadOnlyList<IssueProvider>>(
			() => AddInTree.BuildItems<CodeIssueProvider>("/SharpDevelop/ViewContent/TextEditor/C#/IssueProviders", null, false)
			.Select(p => new IssueProvider(p)).ToList());
		
		internal static IReadOnlyList<IssueProvider> IssueProviders {
			get { return issueProviders.Value; }
		}
		
		internal class IssueProvider
		{
			public readonly Type ProviderType;
			public readonly IssueDescriptionAttribute Attribute;
			
			public IssueProvider(CodeIssueProvider provider)
			{
				if (provider == null)
					throw new ArgumentNullException("provider");
				this.ProviderType = provider.GetType();
				var attributes = ProviderType.GetCustomAttributes(typeof(IssueDescriptionAttribute), true);
				
				Severity defaultSeverity = Severity.Hint;
				if (attributes.Length == 1) {
					this.Attribute = (IssueDescriptionAttribute)attributes[0];
					defaultSeverity = this.Attribute.Severity;
					IsRedundancy = this.Attribute.Category == IssueCategories.RedundanciesInCode || this.Attribute.Category == IssueCategories.RedundanciesInDeclarations;
				} else {
					SD.Log.Warn("Issue provider without attribute: " + ProviderType);
				}
				var properties = PropertyService.NestedProperties("CSharpIssueSeveritySettings");
				this.CurrentSeverity = properties.Get(ProviderType.FullName, defaultSeverity);
			}
			
			public Severity CurrentSeverity { get; set; }
			public bool IsRedundancy { get; set; }
			
			public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
			{
				// use a separate instance for every call, this is necessary
				// for thread-safety
				var provider = (CodeIssueProvider)Activator.CreateInstance(ProviderType);
				return provider.GetIssues(context);
			}
		}
		
		static event EventHandler IssueSeveritySettingsSaved;
		
		internal static void SaveIssueSeveritySettings()
		{
			var properties = PropertyService.NestedProperties("CSharpIssueSeveritySettings");
			foreach (var provider in issueProviders.Value) {
				if (provider.Attribute != null) {
					if (provider.CurrentSeverity == provider.Attribute.Severity)
						properties.Remove(provider.ProviderType.FullName);
					else
						properties.Set(provider.ProviderType.FullName, provider.CurrentSeverity);
				}
			}
			if (IssueSeveritySettingsSaved != null)
				IssueSeveritySettingsSaved(null, EventArgs.Empty);
		}
		
		readonly ITextEditor editor;
		readonly ITextMarkerService markerService;
		
		public IssueManager(ITextEditor editor)
		{
			this.editor = editor;
			this.markerService = editor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			//SD.ParserService.ParserUpdateStepFinished += ParserService_ParserUpdateStepFinished;
			SD.ParserService.ParseInformationUpdated += SD_ParserService_ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished += RerunAnalysis;
			IssueSeveritySettingsSaved += RerunAnalysis; // re-run analysis when settings are changed
			editor.ContextActionProviders.Add(this);
		}
		
		public void Dispose()
		{
			editor.ContextActionProviders.Remove(this);
			//SD.ParserService.ParserUpdateStepFinished -= ParserService_ParserUpdateStepFinished;
			SD.ParserService.ParseInformationUpdated -= SD_ParserService_ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished -= RerunAnalysis;
			IssueSeveritySettingsSaved -= RerunAnalysis;
			if (cancellationTokenSource != null)
				cancellationTokenSource.Cancel();
			Clear();
		}
		
		sealed class InspectionTag
		{
			readonly IssueManager manager;
			public readonly IssueProvider Provider;
			public readonly ITextSourceVersion InspectedVersion;
			public readonly string Description;
			public readonly int StartOffset;
			public readonly int EndOffset;
			public readonly IReadOnlyList<IContextAction> Actions;
			public readonly Severity Severity;
			public readonly IssueMarker MarkerType;
			
			public InspectionTag(IssueManager manager, IssueProvider provider, ITextSourceVersion inspectedVersion, string description, int startOffset, int endOffset, IssueMarker markerType, IEnumerable<CodeAction> actions)
			{
				this.manager = manager;
				this.Provider = provider;
				this.InspectedVersion = inspectedVersion;
				this.Description = description;
				this.StartOffset = startOffset;
				this.EndOffset = endOffset;
				this.Severity = provider.CurrentSeverity;
				this.MarkerType = markerType;
				
				this.Actions = actions.Select(Wrap).ToList();
			}
			
			IContextAction Wrap(CodeAction actionToWrap, int index)
			{
				// Take care not to capture 'actionToWrap' in the lambda
				string actionDescription = actionToWrap.Description;
				return new CSharpContextActionWrapper(
					manager, actionToWrap,
					context => {
						// Look up the new issue position
						int newStart = InspectedVersion.MoveOffsetTo(context.Version, StartOffset, AnchorMovementType.Default);
						int newEnd = InspectedVersion.MoveOffsetTo(context.Version, EndOffset, AnchorMovementType.Default);
						// If the length changed, don't bother looking up the issue again
						if (newEnd - newStart != EndOffset - StartOffset)
							return null;
						// Now rediscover this issue in the new context
						var issue = this.Provider.GetIssues(context).FirstOrDefault(
							i => context.GetOffset(i.Start) == newStart && context.GetOffset(i.End) == newEnd && i.Description == this.Description);
						if (issue == null)
							return null;
						// Now look up the action within that issue:
						if (index < issue.Actions.Count && issue.Actions[index].Description == actionDescription)
							return issue.Actions[index];
						else
							return null;
					});
			}
			
			ITextMarker marker;
			
			public void CreateMarker(IDocument document, ITextMarkerService markerService)
			{
				int startOffset = InspectedVersion.MoveOffsetTo(document.Version, this.StartOffset, AnchorMovementType.Default);
				int endOffset = InspectedVersion.MoveOffsetTo(document.Version, this.EndOffset, AnchorMovementType.Default);
				if (this.StartOffset != this.EndOffset && startOffset >= endOffset)
					return;
				marker = markerService.Create(startOffset, endOffset - startOffset);
				marker.ToolTip = this.Description;
				
				Color color = GetColor(this.Severity);
				color.A = 186;
				marker.MarkerColor = color;
				if (!Provider.IsRedundancy)
					marker.MarkerTypes = TextMarkerTypes.ScrollBarRightTriangle;
				switch (MarkerType) {
					case IssueMarker.WavedLine:
						marker.MarkerTypes |= TextMarkerTypes.SquigglyUnderline;
						break;
					case IssueMarker.DottedLine:
						marker.MarkerTypes |= TextMarkerTypes.DottedUnderline;
						break;
					case IssueMarker.GrayOut:
						marker.ForegroundColor = SystemColors.GrayTextColor;
						break;
				}
				marker.Tag = this;
			}
			
			static Color GetColor(Severity severity)
			{
				switch (severity) {
					case Severity.Error:
						return Colors.Red;
					case Severity.Warning:
						return Colors.Orange;
					case Severity.Suggestion:
						return Colors.Green;
					default:
						return Colors.Blue;
				}
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
		
		void SD_ParserService_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			var parseInfo = e.NewParseInformation as CSharpFullParseInformation;
			ITextSourceVersion currentVersion  = editor.Document.Version;
			if (parseInfo == null)
				return;
			ITextSourceVersion parsedVersion = parseInfo.ParsedVersion;
			if (parsedVersion != null && currentVersion != null && parsedVersion.BelongsToSameDocumentAs(currentVersion)) {
				if (analyzedVersion != null && analyzedVersion.CompareAge(parsedVersion) == 0) {
					// don't analyze the same version twice
					return;
				}
				RunAnalysis(editor.Document.CreateSnapshot(), parseInfo);
			}
		}
		
		async void RerunAnalysis(object sender, EventArgs e)
		{
			var snapshot = editor.Document.CreateSnapshot();
			var parseInfo = await SD.ParserService.ParseAsync(editor.FileName, snapshot) as CSharpFullParseInformation;
			if (parseInfo != null)
				RunAnalysis(snapshot, parseInfo);
			else
				Clear();
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
						var compilation = SD.ParserService.GetCompilationForFile(parseInfo.FileName);
						var resolver = parseInfo.GetResolver(compilation);
						var context = new SDRefactoringContext(textSource, resolver, new TextLocation(0, 0), 0, 0, cancellationToken);
						foreach (var issueProvider in issueProviders.Value) {
							if (issueProvider.CurrentSeverity == Severity.None)
								continue;
							
							foreach (var issue in issueProvider.GetIssues(context)) {
								if (issue.Start.IsEmpty || issue.End.IsEmpty) {
									// Issues can occur on invalid locations when analyzing incomplete code.
									// We'll just ignore them.
									continue;
								}
								results.Add(new InspectionTag(
									this,
									issueProvider,
									textSource.Version,
									issue.Description,
									context.GetOffset(issue.Start),
									context.GetOffset(issue.End),
									issue.IssueMarker,
									issue.Actions));
							}
						}
					}, cancellationToken);
			} catch (TaskCanceledException) {
			} catch (OperationCanceledException) {
			} catch (Exception ex) {
				SD.Log.WarnFormatted("IssueManager crashed: {0}", ex);
				SD.AnalyticsMonitor.TrackException(ex);
			}
			if (!cancellationToken.IsCancellationRequested) {
				analyzedVersion = textSource.Version;
				Clear();
				foreach (var newResult in results) {
					newResult.CreateMarker(editor.Document, markerService);
				}
				existingResults = results;
			}
			if (cancellationTokenSource != null && cancellationTokenSource.Token == cancellationToken) {
				// Dispose the cancellation token source if it's still the same one as we originally created
				cancellationTokenSource.Dispose();
				cancellationTokenSource = null;
			}
		}
		
		#region IContextActionProvider implementation
		string IContextActionProvider.ID {
			get { return "C# IssueManager"; }
		}
		
		string IContextActionProvider.DisplayName {
			get { return "C# IssueManager"; }
		}
		
		string IContextActionProvider.Category {
			get { return string.Empty; }
		}
		
		bool IContextActionProvider.AllowHiding {
			get { return false; }
		}
		
		bool IContextActionProvider.IsVisible {
			get { return true; }
			set { }
		}
		
		Task<IContextAction[]> IContextActionProvider.GetAvailableActionsAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
		{
			List<IContextAction> result = new List<IContextAction>();
			if (existingResults != null) {
				var markers = markerService.GetMarkersAtOffset(context.CaretOffset);
				foreach (var tag in markers.Select(m => m.Tag).OfType<InspectionTag>()) {
					result.AddRange(tag.Actions);
					string issueName;
					if (CanSuppress(tag, out issueName)) {
						result.Add(new SuppressIssueContextAction(issueName));
					}
				}
			}
			return Task.FromResult(result.ToArray());
		}
		
		bool CanSuppress(InspectionTag tag, out string issueName)
		{
			var attr = (IssueDescriptionAttribute)tag.Provider.ProviderType.GetCustomAttributes(false).FirstOrDefault(a => a is IssueDescriptionAttribute);
			issueName = null;
			if (attr == null || attr.AnalysisDisableKeyword == null)
				return false;
			issueName = attr.AnalysisDisableKeyword;
			return true;
		}
		
		[ContextAction("Suppress issue", Description = "Suppresses an issue.")]
		class SuppressIssueContextAction : ContextAction
		{
			string issueName;
			
			public SuppressIssueContextAction(string issueName)
			{
				this.issueName = issueName;
			}
			
			public override Task<bool> IsAvailableAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
			{
				return Task.FromResult(true);
			}
			
			public override string DisplayName
			{
				get { return string.Format("Suppress '{0}'", issueName); }
			}
			
			public override void Execute(EditorRefactoringContext context)
			{
				SD.AnalyticsMonitor.TrackFeature(typeof(SuppressIssueContextAction), issueName);
				var lineNo = context.CaretLocation.Line;
				var document = context.Editor.Document;
				
				var line = document.GetLineByNumber(lineNo);
				string indentation = DocumentUtilities.GetIndentation(document, lineNo);
				string newLine = DocumentUtilities.GetLineTerminator(document, lineNo);
				document.Insert(line.Offset, indentation + "// disable once " + issueName + newLine);
			}
		}
	}
	#endregion
}
