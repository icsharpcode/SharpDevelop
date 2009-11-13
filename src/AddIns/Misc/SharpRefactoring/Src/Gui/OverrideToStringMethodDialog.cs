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
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public class OverrideToStringMethodDialog : InlineRefactorDialog
	{
		ListBox listBox;
		List<Wrapper<IField>> fields;
		
		public OverrideToStringMethodDialog(ITextEditor editor, ITextAnchor anchor, IList<IField> fields)
			: base(editor, anchor)
		{
			this.fields = fields.Select(f => new Wrapper<IField>() { Entity = f }).ToList();
			this.listBox.ItemsSource = this.fields.Select(i => i.Create(null));
		}
		
		protected override UIElement CreateContentElement()
		{
			TextBlock block = new TextBlock() {
				Text = StringParser.Parse("${res:AddIns.SharpRefactoring.OverrideToStringMethod.Description}"),
				Margin = new Thickness(3),
				TextWrapping = TextWrapping.Wrap
			};
			
			listBox = new ListBox() {
				Margin = new Thickness(3)
			};
			
			block.SetValue(DockPanel.DockProperty, Dock.Top);
			
			return new DockPanel() {
				Children = {
					block,
					listBox
				}
			};
		}
		
		protected override string GenerateCode(CodeGenerator generator, IClass currentClass)
		{
			var fields = this.fields
				.Where(f => f.IsChecked)
				.Select(f2 => f2.Entity.Name)
				.ToArray();
			
			if (fields.Any()) {
				StringBuilder formatString = new StringBuilder("[" + currentClass.Name + " ");
				
				for (int i = 0; i < fields.Length; i++) {
					if (i != 0)
						formatString.Append(", ");
					formatString.AppendFormat("{0}={{{1}}}", generator.GetPropertyName(fields[i]), i);
				}
				
				formatString.Append("]");
				
				return "return string.Format(\"" + formatString.ToString() + "\", " + string.Join(", ", fields) + ");";
			}
			
			return "return string.Format(\"[" + currentClass.Name + "]\");";
		}
	}
	
	/* List<Wrapper<IField>> checkedItems = new List<Wrapper<IField>>();
		
		void CheckChange(Wrapper<IField> field)
		{
			if (field.IsChecked)
				checkedItems.Add(field);
			else
				checkedItems.Remove(field);
			
			string text = string.Join(", ", this.checkedItems.Select(f2 => PrintVariableDeclaration(f2.Entity)));
			
			editor.Document.Replace(parameterListAnchorStart.Offset, parameterListAnchorEnd.Offset - parameterListAnchorStart.Offset, text);
		}
		
		string PrintVariableDeclaration(IField field)
		{
			if (field.ReturnType == null)
				return "? " + field.Name;
			else {
				TypeReference type = CodeGenerator.ConvertType(field.ReturnType, new ClassFinder(field));
				return ((type.IsKeyword) ? TypeReference.PrimitiveTypesCSharpReverse[type.Type] : type.Type) + " " + field.Name;
			}
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
	 */
}
