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

using System.ComponentModel;
using System.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.Completion;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public CSharpLanguageBinding()
		{
			this.container.AddService(typeof(IFormattingStrategy), new CSharpFormattingStrategy());
			this.container.AddService(typeof(IBracketSearcher), new CSharpBracketSearcher());
			this.container.AddService(typeof(CodeGenerator), new CSharpCodeGenerator());
			this.container.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), new Microsoft.CSharp.CSharpCodeProvider());
		}
		
		public override ICodeCompletionBinding CreateCompletionBinding(FileName fileName, TextLocation currentLocation, ICSharpCode.NRefactory.Editor.ITextSource fileContent)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			return new CSharpCompletionBinding(fileName, currentLocation, fileContent);
		}
	}
	
	public class CSharpTextEditorExtension : ITextEditorExtension
	{
		ITextEditor editor;
		IssueManager inspectionManager;
		IList<IContextActionProvider> contextActionProviders;
		CodeManipulation codeManipulation;
		CaretReferenceHighlightRenderer renderer;
		CodeEditorFormattingOptionsAdapter options;
		TextEditorOptions originalEditorOptions;
		
		public void Attach(ITextEditor editor)
		{
			this.editor = editor;
			inspectionManager = new IssueManager(editor);
			codeManipulation = new CodeManipulation(editor);
			renderer = new CaretReferenceHighlightRenderer(editor);
			
			// Patch editor options (indentation) to project-specific settings
			if (!editor.ContextActionProviders.IsReadOnly) {
				contextActionProviders = AddInTree.BuildItems<IContextActionProvider>("/SharpDevelop/ViewContent/TextEditor/C#/ContextActions", null);
				editor.ContextActionProviders.AddRange(contextActionProviders);
			}
			
			// Create instance of options adapter and register it as service
			var formattingPolicy = CSharpFormattingPolicies.Instance.GetProjectOptions(
				SD.ProjectService.FindProjectContainingFile(editor.FileName));
			options = new CodeEditorFormattingOptionsAdapter(editor.Options, formattingPolicy.OptionsContainer);
			var textEditor = editor.GetService<TextEditor>();
			if (textEditor != null) {
				var textViewServices = textEditor.TextArea.TextView.Services;
				
				// Unregister any previous ITextEditorOptions instance from editor, if existing, register our impl.
				textViewServices.RemoveService(typeof(ITextEditorOptions));
				textViewServices.AddService(typeof(ITextEditorOptions), options);
				
				// Set TextEditor's options to same object
				originalEditorOptions = textEditor.Options;
				textEditor.Options = options;
			}
		}
		
		public void Detach()
		{
			var textEditor = editor.GetService<TextEditor>();
			if (textEditor != null) {
				var textView = textEditor.TextArea.TextView;
				
				// Unregister our ITextEditorOptions instance from editor
				var optionsService = textView.GetService<ITextEditorOptions>();
				if ((optionsService != null) && (optionsService == options))
					textView.Services.RemoveService(typeof(ITextEditorOptions));
				
				// Reset TextEditor options, too?
				 if ((textEditor.Options != null) && (textEditor.Options == options))
				 	textEditor.Options = originalEditorOptions;
			}
			
			codeManipulation.Dispose();
			if (inspectionManager != null) {
				inspectionManager.Dispose();
				inspectionManager = null;
			}
			if (contextActionProviders != null) {
				editor.ContextActionProviders.RemoveAll(contextActionProviders.Contains);
			}
			renderer.Dispose();
			options = null;
			this.editor = null;
		}
	}
	
	class CodeEditorFormattingOptionsAdapter : TextEditorOptions, ITextEditorOptions, ICodeEditorOptions
	{
		CSharpFormattingOptionsContainer container;
		readonly ITextEditorOptions globalOptions;
		readonly ICodeEditorOptions globalCodeEditorOptions;
		
		public CodeEditorFormattingOptionsAdapter(ITextEditorOptions globalOptions, CSharpFormattingOptionsContainer container)
		{
			if (globalOptions == null)
				throw new ArgumentNullException("globalOptions");
			if (container == null)
				throw new ArgumentNullException("container");
			
			this.globalOptions = globalOptions;
			this.globalCodeEditorOptions = globalOptions as ICodeEditorOptions;
			this.container = container;
			
			CSharpFormattingPolicies.Instance.FormattingPolicyUpdated += OnFormattingPolicyUpdated;
			globalOptions.PropertyChanged += OnGlobalOptionsPropertyChanged;
		}
		
		void OnFormattingPolicyUpdated(object sender, CSharpBinding.FormattingStrategy.CSharpFormattingPolicyUpdateEventArgs e)
		{
			OnPropertyChanged("IndentationSize");
			OnPropertyChanged("ConvertTabsToSpaces");
		}

		void OnGlobalOptionsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}
		
		#region ITextEditorOptions implementation

		public override int IndentationSize {
			get {
				return container.GetEffectiveIndentationSize() ?? globalOptions.IndentationSize;
			}
		}

		public override bool ConvertTabsToSpaces {
			get {
				return container.GetEffectiveConvertTabsToSpaces() ?? globalOptions.ConvertTabsToSpaces;
			}
		}

		public bool AutoInsertBlockEnd {
			get {
				return globalOptions.AutoInsertBlockEnd;
			}
		}

		public int VerticalRulerColumn {
			get {
				return globalOptions.VerticalRulerColumn;
			}
		}

		public bool UnderlineErrors {
			get {
				return globalOptions.UnderlineErrors;
			}
		}

		public string FontFamily {
			get {
				return globalOptions.FontFamily;
			}
		}

		public double FontSize {
			get {
				return globalOptions.FontSize;
			}
		}

		#endregion

		public override bool AllowScrollBelowDocument {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.AllowScrollBelowDocument : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.AllowScrollBelowDocument = value;
				}
			}
		}

		public bool ShowLineNumbers {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.ShowLineNumbers : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.ShowLineNumbers = value;
				}
			}
		}

		public bool EnableChangeMarkerMargin {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.EnableChangeMarkerMargin : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.EnableChangeMarkerMargin = value;
				}
			}
		}

		public bool WordWrap {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.WordWrap : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.WordWrap = value;
				}
			}
		}

		public bool CtrlClickGoToDefinition {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.CtrlClickGoToDefinition : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.CtrlClickGoToDefinition = value;
				}
			}
		}

		public bool MouseWheelZoom {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.MouseWheelZoom : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.MouseWheelZoom = value;
				}
			}
		}

		public bool HighlightBrackets {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.HighlightBrackets : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.HighlightBrackets = value;
				}
			}
		}

		public bool HighlightSymbol {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.HighlightSymbol : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.HighlightSymbol = value;
				}
			}
		}

		public bool EnableAnimations {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.EnableAnimations : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.EnableAnimations = value;
				}
			}
		}

		public bool UseSmartIndentation {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.UseSmartIndentation : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.UseSmartIndentation = value;
				}
			}
		}

		public bool EnableFolding {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.EnableFolding : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.EnableFolding = value;
				}
			}
		}

		public bool EnableQuickClassBrowser {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.EnableQuickClassBrowser : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.EnableQuickClassBrowser = value;
				}
			}
		}

		public bool ShowHiddenDefinitions {
			get {
				return (globalCodeEditorOptions != null) ? globalCodeEditorOptions.ShowHiddenDefinitions : default(bool);
			}
			set {
				if (globalCodeEditorOptions != null) {
					globalCodeEditorOptions.ShowHiddenDefinitions = value;
				}
			}
		}
	}
}
