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
using System.Linq;
using System.Reflection;
using ICSharpCode.AvalonEdit;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
namespace CSharpBinding
{
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
			var formattingPolicy = CSharpFormattingPolicies.Instance.GetProjectOptions(SD.ProjectService.FindProjectContainingFile(editor.FileName));
			var textEditor = editor.GetService<TextEditor>();
			
			if (textEditor != null) {
				options = new CodeEditorFormattingOptionsAdapter(textEditor.Options, editor.Options, formattingPolicy.OptionsContainer);
				var textViewServices = textEditor.TextArea.TextView.Services;
				
				// Unregister any previous ITextEditorOptions instance from editor, if existing, register our impl.
				textViewServices.RemoveService(typeof(ITextEditorOptions));
				textViewServices.AddService(typeof(ITextEditorOptions), options);
				
				// Set TextEditor's options to same object
				originalEditorOptions = textEditor.Options;
				textEditor.Options = options.TextEditorOptions;
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
				if ((textEditor.Options != null) && (textEditor.Options == options.TextEditorOptions))
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
	
	class CodeEditorFormattingOptionsAdapter : ITextEditorOptions, INotifyPropertyChanged
	{
		CSharpFormattingOptionsContainer container;
		readonly TextEditorOptions avalonEditOptions;
		readonly TextEditorOptions originalAvalonEditOptions;
		readonly ITextEditorOptions originalSDOptions;
		
		public CodeEditorFormattingOptionsAdapter(TextEditorOptions originalAvalonEditOptions, ITextEditorOptions originalSDOptions, CSharpFormattingOptionsContainer container)
		{
			if (originalAvalonEditOptions == null)
				throw new ArgumentNullException("originalAvalonEditOptions");
			if (originalSDOptions == null)
				throw new ArgumentNullException("originalSDOptions");
			if (container == null)
				throw new ArgumentNullException("container");
			
			this.originalAvalonEditOptions = originalAvalonEditOptions;
			this.avalonEditOptions = new TextEditorOptions(originalAvalonEditOptions);
			this.originalSDOptions = originalSDOptions;
			this.container = container;
			
			// Update overridden options once
			UpdateOverriddenProperties();
			
			CSharpFormattingPolicies.Instance.FormattingPolicyUpdated += OnFormattingPolicyUpdated;
			this.originalAvalonEditOptions.PropertyChanged += OnOrigAvalonOptionsPropertyChanged;
			this.originalSDOptions.PropertyChanged += OnSDOptionsPropertyChanged;
		}
		
		void OnFormattingPolicyUpdated(object sender, CSharpBinding.FormattingStrategy.CSharpFormattingPolicyUpdateEventArgs e)
		{
			// Update editor options from changed policy
			UpdateOverriddenProperties();
			
			OnPropertyChanged("IndentationSize");
			OnPropertyChanged("IndentationString");
			OnPropertyChanged("ConvertTabsToSpaces");
		}
		
		void UpdateOverriddenProperties()
		{
			avalonEditOptions.IndentationSize = container.GetEffectiveIndentationSize() ?? originalSDOptions.IndentationSize;
			avalonEditOptions.ConvertTabsToSpaces = container.GetEffectiveConvertTabsToSpaces() ?? originalSDOptions.ConvertTabsToSpaces;
		}

		void OnOrigAvalonOptionsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if ((e.PropertyName != "IndentationSize") && (e.PropertyName != "IndentationString") && (e.PropertyName != "ConvertTabsToSpaces")) {
				// Update values in our own TextEditorOptions instance
				PropertyInfo propertyInfo = typeof(TextEditorOptions).GetProperty(e.PropertyName);
				if (propertyInfo != null) {
					propertyInfo.SetValue(avalonEditOptions, propertyInfo.GetValue(originalAvalonEditOptions));
				}
			} else {
				UpdateOverriddenProperties();
			}
			OnPropertyChanged(e.PropertyName);
		}

		void OnSDOptionsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public ICSharpCode.AvalonEdit.TextEditorOptions TextEditorOptions
		{
			get {
				return avalonEditOptions;
			}
		}

		#region Overridden properties
		
		public int IndentationSize {
			get {
				// Get value from own TextEditorOptions instance
				return avalonEditOptions.IndentationSize;
			}
		}

		public string IndentationString {
			get {
				// Get value from own TextEditorOptions instance
				return avalonEditOptions.IndentationString;
			}
		}

		public bool ConvertTabsToSpaces {
			get {
				// Get value from own TextEditorOptions instance
				return avalonEditOptions.ConvertTabsToSpaces;
			}
		}
		
		#endregion
		
		#region Rest of ITextEditorOptions implementation

		public bool AutoInsertBlockEnd {
			get {
				return originalSDOptions.AutoInsertBlockEnd;
			}
		}

		public int VerticalRulerColumn {
			get {
				return originalSDOptions.VerticalRulerColumn;
			}
		}

		public bool UnderlineErrors {
			get {
				return originalSDOptions.UnderlineErrors;
			}
		}

		public string FontFamily {
			get {
				return originalSDOptions.FontFamily;
			}
		}

		public double FontSize {
			get {
				return originalSDOptions.FontSize;
			}
		}

		#endregion

	}
}


