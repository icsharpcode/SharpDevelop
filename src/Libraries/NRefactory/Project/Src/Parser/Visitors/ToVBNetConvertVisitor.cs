/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 02.01.2006
 * Time: 18:18
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Converts elements not supported by VB to their VB representation.
	/// Not all elements are converted here, most simple elements (e.g. ConditionalExpression)
	/// are converted in the output visitor.
	/// </summary>
	public class ToVBNetConvertVisitor : AbstractASTVisitor
	{
		// The following conversions are implemented:
		//   Conflicting field/property names -> m_field
		//   Anonymous methods are put into new methods
		//   Simple event handler creation is replaced with AddressOfExpression
		
		TypeDeclaration currentType;
		readonly List<KeyValuePair<Statement, Statement>> replacements = new List<KeyValuePair<Statement, Statement>>();
		readonly List<MethodDeclaration> newMethods = new List<MethodDeclaration>();
		
		public override object Visit(TypeDeclaration td, object data)
		{
			TypeDeclaration outerType = currentType;
			currentType = td;
			
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
			base.Visit(td, data);
			currentType = outerType;
			
			foreach (MethodDeclaration md in newMethods) {
				td.AddChild(md);
			}
			newMethods.Clear();
			foreach (KeyValuePair<Statement, Statement> pair in replacements) {
				Statement.Replace(pair.Key, pair.Value);
			}
			replacements.Clear();
			
			return null;
		}
		
		string GetAnonymousMethodName()
		{
			for (int i = 1;; i++) {
				string name = "ConvertedAnonymousMethod" + i;
				bool ok = true;
				foreach (object c in currentType.Children) {
					MethodDeclaration method = c as MethodDeclaration;
					if (method != null && method.Name == name) {
						ok = false;
						break;
					}
				}
				foreach (MethodDeclaration method in newMethods) {
					if (method != null && method.Name == name) {
						ok = false;
						break;
					}
				}
				if (ok)
					return name;
			}
		}
		
		public override object Visit(StatementExpression statementExpression, object data)
		{
			base.Visit(statementExpression, data);
			AssignmentExpression ass = statementExpression.Expression as AssignmentExpression;
			if (ass != null && ass.Right is AddressOfExpression) {
				if (ass.Op == AssignmentOperatorType.Add) {
					Statement st = new AddHandlerStatement(ass.Left, ass.Right);
					replacements.Add(new KeyValuePair<Statement, Statement>(statementExpression, st));
				} else if (ass.Op == AssignmentOperatorType.Subtract) {
					Statement st = new RemoveHandlerStatement(ass.Left, ass.Right);
					replacements.Add(new KeyValuePair<Statement, Statement>(statementExpression, st));
				}
			}
			return null;
		}
		
		string GetMemberNameOnThisReference(Expression expr)
		{
			IdentifierExpression ident = expr as IdentifierExpression;
			if (ident != null)
				return ident.Identifier;
			FieldReferenceExpression fre = expr as FieldReferenceExpression;
			if (fre != null && fre.TargetObject is ThisReferenceExpression)
				return fre.FieldName;
			return null;
		}
		
		string GetMethodNameOfDelegateCreation(Expression expr)
		{
			string name = GetMemberNameOnThisReference(expr);
			if (name != null)
				return name;
			ObjectCreateExpression oce = expr as ObjectCreateExpression;
			if (oce != null && oce.Parameters.Count == 1) {
				return GetMemberNameOnThisReference(oce.Parameters[0]);
			}
			return null;
		}
		
		public override object Visit(AssignmentExpression assignmentExpression, object data)
		{
			AnonymousMethodExpression ame = assignmentExpression.Right as AnonymousMethodExpression;
			if (currentType != null && ame != null) {
				MethodDeclaration method = new MethodDeclaration(GetAnonymousMethodName(), Modifier.Private, new TypeReference("System.Void"), ame.Parameters, null);
				method.Body = ame.Body;
				newMethods.Add(method);
				assignmentExpression.Right = new AddressOfExpression(new IdentifierExpression(method.Name));
			}
			if (assignmentExpression.Op == AssignmentOperatorType.Add
			    || assignmentExpression.Op == AssignmentOperatorType.Subtract)
			{
				string methodName = GetMethodNameOfDelegateCreation(assignmentExpression.Right);
				if (methodName != null) {
					foreach (object c in currentType.Children) {
						MethodDeclaration method = c as MethodDeclaration;
						if (method != null && method.Name == methodName) {
							// this statement is registering an event
							assignmentExpression.Right = new AddressOfExpression(new IdentifierExpression(methodName));
							break;
						}
					}
				}
			}
			return base.Visit(assignmentExpression, data);
		}
	}
}
