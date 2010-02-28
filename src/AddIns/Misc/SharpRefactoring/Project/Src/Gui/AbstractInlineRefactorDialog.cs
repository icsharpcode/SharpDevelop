// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
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
		protected ITextEditor editor;
		
		ClassFinder classFinderContext;
		InsertionContext context;
		
		public IInlineUIElement Element { get; set; }
		
		public AbstractInlineRefactorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
		{
			this.anchor = anchor;
			this.editor = editor;
			this.context = context;
			
			this.classFinderContext = new ClassFinder(ParserService.ParseCurrentViewContent(), editor.Document.Text, editor.Caret.Offset);
			
			this.Background = SystemColors.ControlBrush;
		}
		
		protected abstract string GenerateCode(CodeGenerator generator, IClass currentClass);
		
		protected void OKButtonClick(object sender, RoutedEventArgs e)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			
			try {
				if (optionBindings != null) {
					foreach (OptionBinding binding in optionBindings)
						binding.Save();
				}
				
				if (parseInfo != null) {
					CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
					IClass current = parseInfo.CompilationUnit.GetInnermostClass(editor.Caret.Line, editor.Caret.Column);
					
					editor.Document.Insert(anchor.Offset, GenerateCode(generator, current) ?? "");
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
			
			Deactivate();
		}
		
		protected void CancelButtonClick(object sender, RoutedEventArgs e)
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
		}
		
		void IActiveElement.Deactivate()
		{
			Deactivate();
		}

		void Deactivate()
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");

			Element.Remove();
		}
	}
}
