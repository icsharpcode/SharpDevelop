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
using CSharpBinding.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public abstract class AbstractInlineRefactorDialog : GroupBox, IOptionBindingContainer, IActiveElement
	{
		private class UndoableInlineDialogCreation : IUndoableOperation
		{
			private AbstractInlineRefactorDialog _dialog;
			
			public UndoableInlineDialogCreation(AbstractInlineRefactorDialog dialog)
			{
				_dialog = dialog;
			}
			
			public void Reset()
			{
				// Remove dialog reference
				_dialog = null;
			}
			
			public void Undo()
			{
				if (_dialog != null) {
					// Close the dialog
					_dialog.Deactivate();
					Reset();
				}
			}
			
			public void Redo()
			{
				// We don't react to Redo command here...
			}
		}
		
		protected ITextAnchor anchor;
		protected ITextAnchor insertionEndAnchor;
		protected ITextEditor editor;
		
		protected SDRefactoringContext refactoringContext;
		protected InsertionContext insertionContext;
		
		private UndoableInlineDialogCreation undoableCreationOperation;
		
		public IInlineUIElement Element { get; set; }
		
		protected AbstractInlineRefactorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			this.anchor = insertionEndAnchor = anchor;
			this.editor = editor;
			this.insertionContext = context;
			
			this.Background = SystemColors.ControlBrush;
			
			undoableCreationOperation = new UndoableInlineDialogCreation(this);
		}
		
		public IUndoableOperation UndoableCreationOperation
		{
			get
			{
				return undoableCreationOperation;
			}
		}
		
		protected virtual void FocusFirstElement()
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate { this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); }));
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
			
			// Cut connection with UndoableInlineDialogCreation
			undoableCreationOperation.Reset();
			
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
