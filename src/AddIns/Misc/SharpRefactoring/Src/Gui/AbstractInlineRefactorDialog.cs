// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public abstract class AbstractInlineRefactorDialog : DockPanel, IOptionBindingContainer
	{
		protected ITextAnchor anchor;
		protected ITextEditor editor;
		
		public IInlineUIElement Element { get; set; }
		
		public AbstractInlineRefactorDialog(ITextEditor editor, ITextAnchor anchor)
		{
			this.anchor = anchor;
			this.editor = editor;
			
			this.Background = SystemColors.ControlBrush;
			
			Initialize();
		}
		
		protected virtual void Initialize()
		{
			UIElement content = CreateContentElement();
			
			Button okButton = new Button() {
				Content = StringParser.Parse("${res:Global.OKButtonText}"),
				Margin = new Thickness(3)
			};
			
			okButton.Click += OKButtonClick;
			
			Button cancelButton = new Button() {
				Content = StringParser.Parse("${res:Global.CancelButtonText}"),
				Margin = new Thickness(3)
			};
			
			cancelButton.Click += CancelButtonClick;
			
			StackPanel buttonsPanel = new StackPanel() {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Children = {
					okButton,
					cancelButton
				}
			};
			
			buttonsPanel.SetValue(DockPanel.DockProperty, Dock.Bottom);
			
			this.Children.Add(buttonsPanel);
			this.Children.Add(content);
		}
		
		protected abstract UIElement CreateContentElement();
		protected abstract string GenerateCode(CodeGenerator generator, IClass currentClass);
		
		void OKButtonClick(object sender, RoutedEventArgs e)
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			
			if (parseInfo != null) {
				CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
				IClass current = parseInfo.CompilationUnit.GetInnermostClass(editor.Caret.Line, editor.Caret.Column);
				
				editor.Document.Insert(anchor.Offset, GenerateCode(generator, current) ?? "");
			}
			
			if (optionBindings != null) {
				foreach (OptionBinding binding in optionBindings)
					binding.Save();
			}
			
			Element.Remove();
		}
		
		void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			
			Element.Remove();
		}
		
		List<OptionBinding> optionBindings;
		
		public void AddBinding(OptionBinding binding)
		{
			if (optionBindings == null)
				optionBindings = new List<OptionBinding>();
			
			optionBindings.Add(binding);
		}
	}
}
