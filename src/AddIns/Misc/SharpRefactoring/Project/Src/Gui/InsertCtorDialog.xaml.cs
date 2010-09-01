// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for InsertCtorDialog.xaml
	/// </summary>
	public partial class InsertCtorDialog : AbstractInlineRefactorDialog
	{
		protected IList<CtorParamWrapper> paramList;
		
		public InsertCtorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IClass current)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.varList.ItemsSource = paramList = CreateCtorParams(current.Fields, current.Properties)
				// "Add check for null" is checked for every item by default
				//Select(w => { if(w.IsNullable) w.AddCheckForNull = true; return w; }).
				.ToList();
		}
		
		IEnumerable<CtorParamWrapper> CreateCtorParams(IEnumerable<IField> fields, IEnumerable<IProperty> properties)
		{
			int i = 0;
			
			foreach (var f in fields) {
				yield return new CtorParamWrapper(f) { Index = i, IsSelected = !f.IsReadonly };
				i++;
			}
			
			foreach (var p in properties.Where(prop => prop.CanSet && !prop.IsIndexer)) {
				yield return new CtorParamWrapper(p) { Index = i, IsSelected = !p.IsReadonly };
				i++;
			}
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			var line = editor.Document.GetLineForOffset(editor.Caret.Offset);
			
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset) + "\t";
			
			var filtered = paramList.Where(p => p.IsSelected).OrderBy(p => p.Index).ToList();
			
			BlockStatement block = new BlockStatement();
			
			foreach (CtorParamWrapper w in filtered) {
				if (w.AddCheckForNull) {
					if (w.Type.IsReferenceType == true)
						block.AddChild(
							new IfElseStatement(
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.Equality, new PrimitiveExpression(null)),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							)
						);
					else
						block.AddChild(
							new IfElseStatement(
								new UnaryOperatorExpression(new MemberReferenceExpression(new IdentifierExpression(w.MemberName), "HasValue"), UnaryOperatorType.Not),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							)
						);
				}
				if (w.AddRangeCheck) {
					block.AddChild(
						new IfElseStatement(
							new BinaryOperatorExpression(
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.LessThan, new IdentifierExpression("lower")),
								BinaryOperatorType.LogicalOr,
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.GreaterThan, new IdentifierExpression("upper"))
							),
							new ThrowStatement(
								new ObjectCreateExpression(
									new TypeReference("ArgumentOutOfRangeException"),
									new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"'), new IdentifierExpression(w.ParameterName), new BinaryOperatorExpression(new PrimitiveExpression("Value must be between "), BinaryOperatorType.Add, new BinaryOperatorExpression(new IdentifierExpression("lower"), BinaryOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(" and "), BinaryOperatorType.Add, new IdentifierExpression("upper")))) }
								)
							)
						)
					);
				}
			}
			
			foreach (CtorParamWrapper w in filtered)
				block.AddChild(new ExpressionStatement(new AssignmentExpression(new MemberReferenceExpression(new ThisReferenceExpression(), w.MemberName), AssignmentOperatorType.Assign, new IdentifierExpression(w.ParameterName))));
			
			AnchorElement parameterList = context.ActiveElements
				.FirstOrDefault(
					item => item is AnchorElement &&
					(item as AnchorElement).Name.Equals("parameterList", StringComparison.OrdinalIgnoreCase)
				) as AnchorElement;
			
			if (parameterList != null) {
				StringBuilder pList = new StringBuilder();
				
				var parameters = filtered
					.Select(p => new ParameterDeclarationExpression(ConvertType(p.Type), p.ParameterName))
					.ToList();
				
				for (int i = 0; i < parameters.Count; i++) {
					if (i > 0)
						pList.Append(", ");
					pList.Append(language.CodeGenerator.GenerateCode(parameters[i], ""));
				}
				
				parameterList.Text = pList.ToString();
			}
			
			StringBuilder builder = new StringBuilder();
			
			foreach (var element in block.Children.OfType<AbstractNode>()) {
				builder.Append(language.CodeGenerator.GenerateCode(element, indent));
			}
			
			return builder.ToString().Trim();
		}
		
		void UpClick(object sender, System.Windows.RoutedEventArgs e)
		{
			int selection = varList.SelectedIndex;
			
			if (selection <= 0)
				return;
			
			var curItem = paramList.First(p => p.Index == selection);
			var exchangeItem = paramList.First(p => p.Index == selection - 1);
			
			curItem.Index = selection - 1;
			exchangeItem.Index = selection;
			
			varList.ItemsSource = paramList.OrderBy(p => p.Index);
			varList.SelectedIndex = selection - 1;
		}
		
		void DownClick(object sender, System.Windows.RoutedEventArgs e)
		{
			int selection = varList.SelectedIndex;
			
			if (selection < 0 || selection >= paramList.Count - 1)
				return;
			
			var curItem = paramList.First(p => p.Index == selection);
			var exchangeItem = paramList.First(p => p.Index == selection + 1);
			
			curItem.Index = selection + 1;
			exchangeItem.Index = selection;
			
			varList.ItemsSource = paramList.OrderBy(p => p.Index);
			varList.SelectedIndex = selection + 1;
		}
	}
	
	[ValueConversion(typeof(int), typeof(bool))]
	public class IntToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value) != -1;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? 0 : -1;
		}
	}
}
