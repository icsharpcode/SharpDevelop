// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public abstract class InlineRefactorDialog : DockPanel
	{
		protected ITextAnchor anchor;
		protected ITextEditor editor;
		
		public IInlineUIElement Element { get; set; }
		
		public InlineRefactorDialog(ITextEditor editor, ITextAnchor anchor)
		{
			this.anchor = anchor;
			this.editor = editor;
			
			this.Background = SystemColors.ControlBrush;
			
			Initialize();
		}
		
		void Initialize()
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
		protected abstract void GenerateCode(CodeGenerator generator, IClass currentClass);
		
		void OKButtonClick(object sender, RoutedEventArgs e)
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			
			if (parseInfo != null) {
				CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
				IClass current = parseInfo.CompilationUnit.GetInnermostClass(editor.Caret.Line, editor.Caret.Column);
				
				GenerateCode(generator, current);
			}
			
			Element.Remove();
		}
		
		void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			if (Element == null)
				throw new InvalidOperationException("no IInlineUIElement set!");
			
			Element.Remove();
		}
	}
	
	public class OverrideToStringMethodDialog : InlineRefactorDialog
	{
		ListBox listBox;
		List<EntityWrapper> fields;
		ITextAnchor parameterListAnchor;
		
		public OverrideToStringMethodDialog(ITextEditor editor, ITextAnchor anchor, ITextAnchor parameterListAnchor, IList<IField> fields)
			: base(editor, anchor)
		{
			this.parameterListAnchor = parameterListAnchor;
			this.fields = fields.Select(f => new EntityWrapper() { Entity = f }).ToList();
			this.listBox.ItemsSource = this.fields.Select(i => i.Create());
		}
		
		protected override UIElement CreateContentElement()
		{
			listBox = new ListBox() {
				Margin = new Thickness(3)
			};
			
			return listBox;
		}
		
		protected override void GenerateCode(CodeGenerator generator, IClass currentClass)
		{
			var fields = this.fields
				.Where(f => f.IsChecked)
				.Select(f2 => CreateAssignment(f2.Entity.Name, f2.Entity.Name))
				.ToArray();
			generator.InsertCodeInClass(currentClass, new RefactoringDocumentAdapter(editor.Document), anchor.Line, fields);
		}
		
		Statement CreateAssignment(string memberName, string parameter)
		{
			return new ExpressionStatement(
				new AssignmentExpression(
					new MemberReferenceExpression(new ThisReferenceExpression(), memberName),
					AssignmentOperatorType.Assign,
					new IdentifierExpression(parameter)
				)
			);
		}
	}
	
	class EntityWrapper
	{
		public IEntity Entity { get; set; }
		public bool IsChecked { get; set; }
		
		public object Create()
		{
			CheckBox box = new CheckBox() {
				Content = Entity.Name
			};
			
			box.Checked += delegate { this.IsChecked = true; };
			box.Unchecked += delegate { this.IsChecked = false; };
			
			return box;
		}
	}
}
