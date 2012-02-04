// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for OverrideToStringMethodDialog.xaml
	/// </summary>
	public partial class OverrideToStringMethodDialog : AbstractInlineRefactorDialog
	{
		string baseCall;
		string insertedCode;
		
		public OverrideToStringMethodDialog(InsertionContext context, ITextEditor editor, ITextAnchor startAnchor, ITextAnchor anchor, IList<PropertyOrFieldWrapper> fields, string baseCall)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.baseCall = baseCall;
			this.listBox.ItemsSource = fields;
			
			listBox.SelectAll();
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			string[] fields = listBox.SelectedItems.OfType<PropertyOrFieldWrapper>().Select(f2 => f2.MemberName).ToArray();
			
			Ast.PrimitiveExpression formatString = new Ast.PrimitiveExpression(GenerateFormatString(currentClass, language.CodeGenerator, fields));
			List<Ast.Expression> param = new List<Ast.Expression>() { formatString };
			
			Ast.ReturnStatement ret = new Ast.ReturnStatement(new Ast.InvocationExpression(
				new Ast.MemberReferenceExpression(new Ast.TypeReferenceExpression(new Ast.TypeReference("System.String", true)), "Format"),
				param.Concat(fields.Select(f => new Ast.IdentifierExpression(f))).ToList()
			));
			
			insertedCode = language.CodeGenerator.GenerateCode(ret, "").Trim();
			
			return insertedCode;
		}
		
		string GenerateFormatString(IClass currentClass, CodeGenerator generator, string[] fields)
		{
			string fieldsString = "";
			
			if (fields.Any()) {
				StringBuilder formatString = new StringBuilder();
				
				for (int i = 0; i < fields.Length; i++) {
					if (i != 0)
						formatString.Append(", ");
					formatString.AppendFormat("{0}={{{1}}}", generator.GetPropertyName(fields[i]), i);
				}
				
				fieldsString = " " + formatString.ToString();
			}
			
			return "[" + currentClass.Name + fieldsString + "]";
		}
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			listBox.SelectAll();
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			listBox.UnselectAll();
		}
		
		bool AllSelected {
			get { return listBox.SelectedItems.Count == listBox.Items.Count; }
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Key? allAccessKey = GetAccessKeyFromButton(selectAll);
			
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && allAccessKey == e.SystemKey) {
				if (AllSelected)
					listBox.UnselectAll();
				else
					listBox.SelectAll();
				e.Handled = true;
			}
			
			base.OnKeyDown(e);
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
			editor.Document.Insert(anchor.Offset, baseCall);
			editor.Select(anchor.Offset, baseCall.Length);
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
			
			editor.Caret.Offset = insertionEndAnchor.Offset + insertedCode.Length;
		}
	}
}
