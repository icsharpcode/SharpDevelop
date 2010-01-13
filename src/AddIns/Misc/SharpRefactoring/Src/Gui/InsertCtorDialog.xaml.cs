// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
		IList<CtorParamWrapper> paramList;
		
		public InsertCtorDialog(ITextEditor editor, ITextAnchor anchor, IClass current)
			: base(editor, anchor)
		{
			InitializeComponent();
			
			this.varList.ItemsSource = paramList = CreateCtorParams(current.Fields).ToList();
		}
		
		IEnumerable<CtorParamWrapper> CreateCtorParams(IEnumerable<IField> fields)
		{
			int i = 0;
			
			foreach (IField f in fields) {
				yield return new CtorParamWrapper(f) { Index = i };
				i++;
			}
		}
		
		protected override string GenerateCode(CodeGenerator generator, IClass currentClass)
		{
			StringBuilder builder = new StringBuilder();
			
			var line = editor.Document.GetLineForOffset(editor.Caret.Offset);
			
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset);
			
			var filtered = paramList.Where(p => p.IsSelected).OrderBy(p => p.Index).ToList();
			
			BlockStatement block = new BlockStatement();
			
			foreach (CtorParamWrapper w in filtered) {
				if (w.AddCheckForNull) {
					if (w.Type.IsReferenceType == true)
						block.AddChild(
							new IfElseStatement(
								new BinaryOperatorExpression(new IdentifierExpression(w.Name), BinaryOperatorType.Equality, new PrimitiveExpression(null)),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.Name, '"' + w.Name + '"') }))
							)
						);
					else
						block.AddChild(
							new IfElseStatement(
								new UnaryOperatorExpression(new MemberReferenceExpression(new IdentifierExpression(w.Name), "HasValue"), UnaryOperatorType.Not),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.Name, '"' + w.Name + '"') }))
							)
						);
				}
				if (!string.IsNullOrWhiteSpace(w.UpperBound) && !string.IsNullOrWhiteSpace(w.LowerBound)) {
					double upper, lower;
					if (!double.TryParse(w.UpperBound, out upper))
						throw new Exception("Upper bound not valid for parameter " + w.Name);
					if (!double.TryParse(w.LowerBound, out lower))
						throw new Exception("Lower bound not valid for parameter " + w.Name);
					block.AddChild(
						new IfElseStatement(
							new BinaryOperatorExpression(
								new BinaryOperatorExpression(new IdentifierExpression(w.Name), BinaryOperatorType.LessThan, new PrimitiveExpression(lower)),
								BinaryOperatorType.LogicalOr,
								new BinaryOperatorExpression(new IdentifierExpression(w.Name), BinaryOperatorType.GreaterThan, new PrimitiveExpression(upper))
							),
							new ThrowStatement(
								new ObjectCreateExpression(
									new TypeReference("ArgumentOutOfRangeException"),
									new List<Expression>() { new PrimitiveExpression(w.Name, '"' + w.Name + '"'), new IdentifierExpression(w.Name), new PrimitiveExpression("Value must be between " + lower.ToString(CultureInfo.InvariantCulture) + " and " + upper.ToString(CultureInfo.InvariantCulture)) }
								)
							)
						)
					);
				}
			}
			
			foreach (CtorParamWrapper w in filtered)
				block.AddChild(new ExpressionStatement(new AssignmentExpression(new MemberReferenceExpression(new ThisReferenceExpression(), w.Name), AssignmentOperatorType.Assign, new IdentifierExpression(w.Name))));
			
			ConstructorDeclaration ctor = new ConstructorDeclaration(currentClass.Name, Modifiers.Public, filtered.Select(p => new ParameterDeclarationExpression(ConvertType(p.Type), p.Name)).ToList(), null) {
				Body = block
			};
			
			builder.Append(generator.GenerateCode(ctor, indent));
			
			return builder.ToString();
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
			
			if (selection >= paramList.Count - 1)
				return;
			
			var curItem = paramList.First(p => p.Index == selection);
			var exchangeItem = paramList.First(p => p.Index == selection + 1);
			
			curItem.Index = selection + 1;
			exchangeItem.Index = selection;
			
			varList.ItemsSource = paramList.OrderBy(p => p.Index);
			varList.SelectedIndex = selection + 1;
		}
	}
	
	public class CtorParamWrapper
	{
		IField field;
		
		public string Text {
			get { return field.ProjectContent.Language.GetAmbience().Convert(field); }
		}
		
		public bool IsNullable {
			get {
				return field.ReturnType.IsReferenceType == true ||
					field.ReturnType.IsConstructedReturnType && field.ReturnType.Name == "Nullable";
			}
		}
		
		public bool HasRange {
			get {
				return (field.ReturnType.IsConstructedReturnType &&
				        IsTypeWithRange(field.ReturnType.CastToConstructedReturnType().TypeArguments.First())
				       ) || IsTypeWithRange(field.ReturnType);
			}
		}
		
		public int Index { get; set; }
		
		public bool IsSelected { get; set; }
		
		public bool AddCheckForNull { get; set; }
		
		public string LowerBound { get; set; }
		
		public string UpperBound { get; set; }
		
		public string Name {
			get { return field.Name; }
		}
		
		public IReturnType Type {
			get { return field.ReturnType; }
		}
		
		bool IsTypeWithRange(IReturnType type)
		{
			return type.Name == "Int32" ||
				type.Name == "Int16" ||
				type.Name == "Int64" ||
				type.Name == "Single" ||
				type.Name == "Double" ||
				type.Name == "UInt16" ||
				type.Name == "UInt32" ||
				type.Name == "UInt64";
		}
		
		public CtorParamWrapper(IField field)
		{
			if (field == null || field.ReturnType == null)
				throw new ArgumentNullException("field");
			
			this.field = field;
		}
	}
}