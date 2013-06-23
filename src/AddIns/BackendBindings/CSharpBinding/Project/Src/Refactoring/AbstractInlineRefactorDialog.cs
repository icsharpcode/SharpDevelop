// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using CSharpBinding.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public abstract class AbstractInlineRefactorDialog : GroupBox, IOptionBindingContainer, IActiveElement
	{
		protected ITextAnchor anchor;
		protected ITextAnchor insertionEndAnchor;
		protected ITextEditor editor;
		
		protected SDRefactoringContext refactoringContext;
		protected InsertionContext insertionContext;
		
		public IInlineUIElement Element { get; set; }
		
		protected AbstractInlineRefactorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			this.anchor = insertionEndAnchor = anchor;
			this.editor = editor;
			this.insertionContext = context;
			
			this.Background = SystemColors.ControlBrush;
		}
		
		protected virtual void FocusFirstElement()
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate { this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); }));
		}
		
		protected void AppendNewLine(Script script, AstNode afterNode, NewLineNode newLineNode)
		{
			if (newLineNode != null)
				script.InsertAfter(afterNode, newLineNode.Clone());
		}
		
		protected abstract string GenerateCode(ITypeDefinition currentClass);
		
		protected virtual void OKButtonClick(object sender, RoutedEventArgs e)
		{
			if (optionBindings != null) {
				foreach (OptionBinding binding in optionBindings)
					binding.Save();
			}
			
			var typeResolveContext = refactoringContext.GetTypeResolveContext();
			if (typeResolveContext == null) {
				return;
			}
			var current = typeResolveContext.CurrentTypeDefinition;
			
			using (editor.Document.OpenUndoGroup()) {
				// GenerateCode could modify the document.
				// So read anchor.Offset after code generation.
				GenerateCode(current);
			}
			
			Deactivate();
		}
		
		protected virtual void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			Deactivate();
		}
		
		List<OptionBinding> optionBindings;
		
		public void AddBinding(OptionBinding binding)
		{
			if (optionBindings == null)
				optionBindings = new List<OptionBinding>();
			
			optionBindings.Add(binding);
		}
		
		protected AstType ConvertType(KnownTypeCode knownTypeCode)
		{
			IType type = refactoringContext.Compilation.FindType(knownTypeCode);
			if (type != null)
				return ConvertType(type);
			
			// Backup solution
			return new SimpleType(KnownTypeReference.GetCSharpNameByTypeCode(knownTypeCode));
		}
		
		protected AstType ConvertType(IType type)
		{
			return refactoringContext.CreateShortType(type);
		}
		
		bool IActiveElement.IsEditable {
			get { return false; }
		}
		
		ISegment IActiveElement.Segment {
			get { return null; }
		}
		
		void IActiveElement.OnInsertionCompleted()
		{
			OnInsertionCompleted();
		}
		
		protected virtual void Initialize()
		{
			this.refactoringContext = SDRefactoringContext.Create(editor, CancellationToken.None);
			
			TextDocument textDocument = editor.Document as TextDocument;
			if (textDocument != null) {
				textDocument.UndoStack.PropertyChanged += UndoStackPropertyChanged;
			}
		}
		
		void UndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CanRedo") {
				// Undo command has been triggered?
				OnUndoTriggered();
				
				// Unregister from event, again
				TextDocument textDocument = editor.Document as TextDocument;
				if (textDocument != null) {
					textDocument.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
				}
			}
		}
		
		protected virtual void OnUndoTriggered()
		{
		}
		
		protected virtual void OnInsertionCompleted()
		{
			Initialize();
			FocusFirstElement();
		}
		
		void IActiveElement.Deactivate(SnippetEventArgs e)
		{
			if (e.Reason == DeactivateReason.Deleted) {
				Deactivate();
				return;
			}
			
			TextDocument textDocument = editor.Document as TextDocument;
			if (textDocument != null) {
				textDocument.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
			}
			
			if (e.Reason == DeactivateReason.ReturnPressed)
				OKButtonClick(null, null);
			
			if (e.Reason == DeactivateReason.EscapePressed)
				CancelButtonClick(null, null);
			
			Deactivate();
		}
		
		bool deactivated;

		protected void Deactivate()
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			if (deactivated)
				return;
			
			deactivated = true;
			Element.Remove();
			
			insertionContext.Deactivate(null);
		}
		
		protected Key? GetAccessKeyFromButton(ContentControl control)
		{
			if (control == null)
				return null;
			string text = control.Content as string;
			if (text == null)
				return null;
			int index = text.IndexOf('_');
			if (index < 0 || index > text.Length - 2)
				return null;
			char ch = text[index + 1];
			// works only for letter keys!
			return (Key)new KeyConverter().ConvertFrom(ch.ToString());
		}
	}
}
