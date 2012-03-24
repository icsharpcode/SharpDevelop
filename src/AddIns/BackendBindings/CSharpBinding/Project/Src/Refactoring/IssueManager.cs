// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
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
			() => AddInTree.BuildItems<ICodeIssueProvider>("/SharpDevelop/ViewContent/TextEditor/C#/IssueProviders", null, false)
			.Select(p => new IssueProvider(p)).ToList());
		
		internal static IReadOnlyList<IssueProvider> IssueProviders {
			get { return issueProviders.Value; }
		}
		
		internal class IssueProvider
		{
			readonly ICodeIssueProvider provider;
			public readonly Type ProviderType;
			public readonly IssueDescriptionAttribute Attribute;
			
			public IssueProvider(ICodeIssueProvider provider)
			{
				if (provider == null)
					throw new ArgumentNullException("provider");
				this.provider = provider;
				this.ProviderType = provider.GetType();
				var attributes = ProviderType.GetCustomAttributes(typeof(IssueDescriptionAttribute), true);
				
				Severity defaultSeverity = Severity.Hint;
				if (attributes.Length == 1) {
					this.Attribute = (IssueDescriptionAttribute)attributes[0];
					defaultSeverity = this.Attribute.Severity;
				}
				var properties = PropertyService.NestedProperties("CSharpIssueSeveritySettings");
				this.CurrentSeverity = properties.Get(ProviderType.FullName, defaultSeverity);
			}
			
			public Severity CurrentSeverity { get; set; }
			
			public IssueMarker MarkerType {
				get { return Attribute != null ? Attribute.IssueMarker : IssueMarker.Underline; }
			}
			
			public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
			{
				return provider.GetIssues(context);
			}
		}
		
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
		}
		
		readonly ITextEditor editor;
		readonly ITextMarkerService markerService;
		
		public IssueManager(ITextEditor editor)
		{
			this.editor = editor;
			this.markerService = editor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			ParserService.ParserUpdateStepFinished += ParserService_ParserUpdateStepFinished;
			editor.ContextActionProviders.Add(this);
		}
		
		public void Dispose()
		{
			editor.ContextActionProviders.Remove(this);
			ParserService.ParserUpdateStepFinished -= ParserService_ParserUpdateStepFinished;
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
			
			public InspectionTag(IssueManager manager, IssueProvider provider, ITextSourceVersion inspectedVersion, string description, int startOffset, int endOffset, IEnumerable<CodeAction> actions)
			{
				this.manager = manager;
				this.Provider = provider;
				this.InspectedVersion = inspectedVersion;
				this.Description = description;
				this.StartOffset = startOffset;
				this.EndOffset = endOffset;
				this.Severity = provider.CurrentSeverity;
				
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
							i => context.GetOffset(i.Start) == newStart && context.GetOffset(i.End) == newEnd && i.Desription == this.Description);
						if (issue == null)
							return null;
						// Now look up the action within that issue:
						if (issue.Action != null && issue.Action.Description == actionDescription)
							return issue.Action;
						else
							return null;
					});
			}
			
			ITextMarker marker;
			
			public void CreateMarker(IDocument document, ITextMarkerService markerService)
			{
				int startOffset = InspectedVersion.MoveOffsetTo(document.Version, this.StartOffset, AnchorMovementType.Default);
				int endOffset = InspectedVersion.MoveOffsetTo(document.Version, this.EndOffset, AnchorMovementType.Default);
				if (startOffset >= endOffset)
					return;
				marker = markerService.Create(startOffset, endOffset - startOffset);
				marker.ToolTip = this.Description;
				
				Color color = GetColor(this.Severity);
				color.A = 186;
				marker.MarkerColor = color;
				marker.MarkerTypes = TextMarkerTypes.ScrollBarRightTriangle;
				switch (Provider.MarkerType) {
					case IssueMarker.Underline:
						marker.MarkerTypes |= TextMarkerTypes.SquigglyUnderline;
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
						foreach (var issueProvider in issueProviders.Value) {
							if (issueProvider.CurrentSeverity == Severity.None)
								continue;
							
							foreach (var issue in issueProvider.GetIssues(context)) {
								results.Add(new InspectionTag(
									this,
									issueProvider,
									textSource.Version,
									issue.Desription,
									context.GetOffset(issue.Start),
									context.GetOffset(issue.End),
									issue.Action != null ? new [] { issue.Action } : new CodeAction[0]));
							}
						}
					}, cancellationToken);
			} catch (TaskCanceledException) {
			}
			if (!cancellationToken.IsCancellationRequested) {
				analyzedVersion = textSource.Version;
				Clear();
				foreach (var newResult in results) {
					newResult.CreateMarker(editor.Document, markerService);
				}
				existingResults = results;
			}
			cancellationTokenSource.Dispose();
			cancellationTokenSource = null;
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
				}
			}
			return Task.FromResult(result.ToArray());
		}
		#endregion
	}
}
