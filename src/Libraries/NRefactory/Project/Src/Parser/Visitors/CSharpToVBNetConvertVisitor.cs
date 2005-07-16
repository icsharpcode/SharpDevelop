/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 16.07.2005
 * Time: 16:32
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// This class converts C# constructs to their VB.NET equivalents.
	/// </summary>
	public class CSharpToVBNetConvertVisitor : AbstractASTVisitor
	{
		// The following conversions are implemented:
		//   Conflicting field/property names -> m_field
		//   a == null -> a Is Nothing
		//   a != null -> a Is Not Nothing
		
		// The following conversions should be implemented in the future:
		//   ForStatement -> ForNextStatement when for-loop is simple
		//   if (Event != null) Event(this, bla); -> RaiseEvent Event(this, bla)
		
		public override object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			if (binaryOperatorExpression.Op == BinaryOperatorType.Equality || binaryOperatorExpression.Op == BinaryOperatorType.InEquality) {
				if (IsNullLiteralExpression(binaryOperatorExpression.Left)) {
					Expression tmp = binaryOperatorExpression.Left;
					binaryOperatorExpression.Left = binaryOperatorExpression.Right;
					binaryOperatorExpression.Right = tmp;
				}
				if (IsNullLiteralExpression(binaryOperatorExpression.Right)) {
					if (binaryOperatorExpression.Op == BinaryOperatorType.Equality) {
						binaryOperatorExpression.Op = BinaryOperatorType.ReferenceEquality;
					} else {
						binaryOperatorExpression.Op = BinaryOperatorType.ReferenceInequality;
					}
				}
			}
			return base.Visit(binaryOperatorExpression, data);
		}
		
		bool IsNullLiteralExpression(Expression expr)
		{
			PrimitiveExpression pe = expr as PrimitiveExpression;
			if (pe == null) return false;
			return pe.Value == null;
		}
		
		public override object Visit(TypeDeclaration td, object data)
		{
			//   Conflicting field/property names -> m_field
			List<string> properties = new List<string>();
			foreach (object o in td.Children) {
				PropertyDeclaration pd = o as PropertyDeclaration;
				if (pd != null) {
					properties.Add(pd.Name);
				}
			}
			List<VariableDeclaration> conflicts = new List<VariableDeclaration>();
			foreach (object o in td.Children) {
				FieldDeclaration fd = o as FieldDeclaration;
				if (fd != null) {
					foreach (VariableDeclaration var in fd.Fields) {
						string name = var.Name;
						foreach (string propertyName in properties) {
							if (name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)) {
								conflicts.Add(var);
							}
						}
					}
				}
			}
			new PrefixFieldsVisitor(conflicts, "m_").Run(td);
			return base.Visit(td, data);
		}
	}
}
