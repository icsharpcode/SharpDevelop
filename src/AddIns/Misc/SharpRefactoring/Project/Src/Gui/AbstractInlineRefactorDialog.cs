// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public abstract class AbstractInlineRefactorDialog : GroupBox, IOptionBindingContainer, IActiveElement
	{
		protected ITextAnchor anchor;
		protected ITextAnchor insertionEndAnchor;
		protected ITextEditor editor;
		
		ClassFinder classFinderContext;
		protected InsertionContext context;
		
		public IInlineUIElement Element { get; set; }
		
		protected AbstractInlineRefactorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			this.anchor = insertionEndAnchor = anchor;
			this.editor = editor;
			this.context = context;
			
			this.classFinderContext = new ClassFinder(ParserService.ParseCurrentViewContent(), editor.Document.Text, anchor.Offset);
			
			this.Background = SystemColors.ControlBrush;
		}
		
		protected virtual void FocusFirstElement()
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate { this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); }));
		}
		
		protected abstract string GenerateCode(LanguageProperties language, IClass currentClass);
		
		protected virtual void OKButtonClick(object sender, RoutedEventArgs e)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			
			if (optionBindings != null) {
				foreach (OptionBinding binding in optionBindings)
					binding.Save();
			}
			
			if (parseInfo != null) {
				LanguageProperties language = parseInfo.CompilationUnit.Language;
				IClass current = parseInfo.CompilationUnit.GetInnermostClass(anchor.Line, anchor.Column);
				
				using (editor.Document.OpenUndoGroup()) {
					// GenerateCode could modify the document.
					// So read anchor.Offset after code generation.
					string code = GenerateCode(language, current) ?? "";
					editor.Document.Insert(anchor.Offset, code);
				}
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
		
		protected TypeReference ConvertType(IReturnType type)
		{
			return CodeGenerator.ConvertType(type, classFinderContext);
		}
		
		bool IActiveElement.IsEditable {
			get { return false; }
		}
		
		ICSharpCode.AvalonEdit.Document.ISegment IActiveElement.Segment {
			get { return null; }
		}
		
		void IActiveElement.OnInsertionCompleted()
		{
			OnInsertionCompleted();
		}
		
		protected virtual void OnInsertionCompleted()
		{
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

		void Deactivate()
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			if (deactivated)
				return;
			
			deactivated = true;
			Element.Remove();
			
			context.Deactivate(null);
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
