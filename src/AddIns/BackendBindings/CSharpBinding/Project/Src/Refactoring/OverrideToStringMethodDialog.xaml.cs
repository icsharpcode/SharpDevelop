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
using System.Linq;
using System.Text;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for OverrideToStringMethodDialog.xaml
	/// </summary>
	public partial class OverrideToStringMethodDialog : AbstractInlineRefactorDialog
	{
		AstNode baseCallNode;
		IList<PropertyOrFieldWrapper> parameterList;
		
		public OverrideToStringMethodDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IList<PropertyOrFieldWrapper> fields, AstNode baseCallNode)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.baseCallNode = baseCallNode;
			parameterList = fields;
			this.listBox.ItemsSource = fields;
			
			SelectAllChecked();
		}
		
		protected override string GenerateCode(ITypeDefinition currentClass)
		{
//			string[] fields = listBox.SelectedItems.OfType<PropertyOrFieldWrapper>().Select(f2 => f2.MemberName).ToArray();
			string[] fields = parameterList.Where(f => f.IsIncluded).Select(f2 => f2.MemberName).ToArray();
			
			PrimitiveExpression formatString = new PrimitiveExpression(GenerateFormatString(currentClass, editor.Language.CodeGenerator, fields));
			List<Expression> param = new List<Expression>() { formatString };
			ReturnStatement ret = new ReturnStatement(new InvocationExpression(
				new MemberReferenceExpression(new TypeReferenceExpression(ConvertType(KnownTypeCode.String)), "Format"),
				param.Concat(fields.Select(f => new IdentifierExpression(f))).ToList()
			));
			
			if (baseCallNode != null) {
				MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
				if (insertedOverrideMethod == null) {
					// We are not inside of a method declaration
					return null;
				}
				
				using (Script script = refactoringContext.StartScript()) {
					NewLineNode nextNewLineNode = insertedOverrideMethod.NextSibling as NewLineNode;
					
					// Find base method call and replace it by return statement
					script.AddTo(insertedOverrideMethod.Body, ret);
					AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
				}
			}
			
			return null;
		}
		
		string GenerateFormatString(ITypeDefinition currentClass, CodeGenerator generator, string[] fields)
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
		
		void SelectAllChecked()
		{
			foreach (var param in parameterList) {
				param.IsIncluded = true;
			}
		}
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllChecked();
		}
		
		void SelectAllUnchecked()
		{
			foreach (var param in parameterList) {
				param.IsIncluded = false;
			}
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllUnchecked();
		}
		
		bool AllSelected {
			get { return parameterList.Count(p => p.IsIncluded) == parameterList.Count; }
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Key? allAccessKey = GetAccessKeyFromButton(selectAll);
			
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && allAccessKey == e.SystemKey) {
				if (AllSelected)
					SelectAllUnchecked();
				else
					SelectAllChecked();
				e.Handled = true;
			}
			
			base.OnKeyDown(e);
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
			if (baseCallNode != null) {
				// Insert at least the base call
				MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
				if (insertedOverrideMethod == null)
				{
					// We are not inside of a method declaration
					return;
				}
				using (Script script = refactoringContext.StartScript()) {
					script.AddTo(insertedOverrideMethod.Body, baseCallNode);
				}
			}
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
		}
	}
}
