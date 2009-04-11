// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3539 $</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlResolver.
	/// </summary>
	public class XamlResolver : IResolver
	{
		IClass callingClass;
		string resolveExpression;
		XamlExpressionContext context;
		ParseInformation parseInfo;
		int caretLineNumber, caretColumn;

		internal bool IsReaderAtTarget(XmlTextReader r)
		{
			if (r.LineNumber > caretLineNumber)
				return true;
			else if (r.LineNumber == caretLineNumber)
				return r.LinePosition >= caretColumn;
			else
				return false;
		}

		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
		{
			this.resolveExpression = expressionResult.Expression;
			this.parseInfo = parseInfo;
			this.caretLineNumber = expressionResult.Region.BeginLine;
			this.caretColumn = expressionResult.Region.BeginColumn;
			this.callingClass = parseInfo.BestCompilationUnit.GetInnermostClass(caretLineNumber, caretColumn);
			this.context = expressionResult.Context as XamlExpressionContext;
			if (context == null)
				return null;
			try {
				using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
					r.WhitespaceHandling = WhitespaceHandling.Significant;
					// move reader to correct position
					while (r.Read() && !IsReaderAtTarget(r)) { }

					if (string.IsNullOrEmpty(context.AttributeName)) {
						return ResolveElementName(r, expressionResult.Expression);
					}
					else if (context.InAttributeValue) {
						MemberResolveResult mrr = ResolveAttribute(r, context.AttributeName);
						if (mrr != null) {
							return ResolveAttributeValue(mrr.ResolvedMember, resolveExpression);
						}
					}
					else {
						// in attribute name
						return ResolveAttribute(r, resolveExpression);
					}
				}
				return null;
			}
			catch (XmlException) {
				return null;
			}
		}

		ResolveResult ResolveElementName(XmlReader r, string exp)
		{
			string xmlNamespace;
			string name;
			this.resolveExpression = exp;
			if (resolveExpression.Contains(":")) {
				string prefix = resolveExpression.Substring(0, resolveExpression.IndexOf(':'));
				name = resolveExpression.Substring(resolveExpression.IndexOf(':') + 1);
				xmlNamespace = r.LookupNamespace(prefix);
			}
			else {
				xmlNamespace = r.LookupNamespace("");
				name = resolveExpression;
			}
			if (name.Contains(".")) {
				string propertyName = name.Substring(name.IndexOf('.') + 1);
				name = name.Substring(0, name.IndexOf('.'));
				return ResolveProperty(xmlNamespace, name, propertyName, true);
			}
			else {
				IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;
				IReturnType resolvedType = XamlCompilationUnit.FindType(pc, xmlNamespace, name);
				IClass resolvedClass = resolvedType != null ? resolvedType.GetUnderlyingClass() : null;
				if (resolvedClass != null) {
					return new TypeResolveResult(callingClass, null, resolvedClass);
				}
				else {
					return null;
				}
			}
		}

		MemberResolveResult ResolveProperty(string xmlNamespace, string className, string propertyName, bool allowAttached)
		{
			IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;
			IReturnType resolvedType = XamlCompilationUnit.FindType(pc, xmlNamespace, className);
			if (resolvedType != null && resolvedType.GetUnderlyingClass() != null) {
				IMember member = resolvedType.GetProperties().Find(delegate(IProperty p) { return p.Name == propertyName; });
				if (member == null) {
					member = resolvedType.GetEvents().Find(delegate(IEvent p) { return p.Name == propertyName; });
				}
				if (member == null && allowAttached) {
					member = resolvedType.GetMethods().Find(
						delegate(IMethod p) {
							return p.IsStatic && p.Parameters.Count == 1 && p.Name == "Get" + propertyName;
						});
					if (member != null) {
						member = new DefaultProperty(resolvedType.GetUnderlyingClass(), propertyName);
					} else {
						IMethod m = resolvedType.GetMethods().Find(
							delegate(IMethod p) {
								return p.IsPublic && p.IsStatic && p.Parameters.Count == 2 && (p.Name == "Add" + propertyName + "Handler" || p.Name == "Remove" + propertyName + "Handler");
							});
						member = m;
						if (member != null)
							member = new DefaultEvent(resolvedType.GetUnderlyingClass(), propertyName) { ReturnType = m.Parameters[1].ReturnType };
					}
				}
				if (member != null)
					return new MemberResolveResult(callingClass, null, member);
			}
			return null;
		}

		MemberResolveResult ResolveAttribute(XmlReader r, string attributeName)
		{
			if (context.ElementPath.Elements.Count == 0) {
				return null;
			}
			string attributeXmlNamespace;
			if (attributeName.Contains(":")) {
				attributeXmlNamespace = r.LookupNamespace(attributeName.Substring(0, attributeName.IndexOf(':')));
				attributeName = attributeName.Substring(attributeName.IndexOf(':') + 1);
			}
			else {
				attributeXmlNamespace = r.LookupNamespace("");
			}
			if (attributeName.Contains(".")) {
				string className = attributeName.Substring(0, attributeName.IndexOf('.'));
				attributeName = attributeName.Substring(attributeName.IndexOf('.') + 1);
				return ResolveProperty(attributeXmlNamespace, className, attributeName, true);
			}
			else {
				ICSharpCode.XmlEditor.QualifiedName lastElement = context.ElementPath.Elements[context.ElementPath.Elements.Count - 1];
				return ResolveProperty(lastElement.Namespace, lastElement.Name, attributeName, false);
			}
		}

		ResolveResult ResolveAttributeValue(IMember propertyOrEvent, string expression)
		{
			if (propertyOrEvent == null)
				return null;
			if (propertyOrEvent is IEvent) {
				return new MethodGroupResolveResult(callingClass, null, callingClass.DefaultReturnType, expression);
			}

			if (propertyOrEvent.Name == "Name" && callingClass != null) {
				foreach (IField f in callingClass.Fields) {
					if (f.Name == expression)
						return new MemberResolveResult(callingClass, null, f);
				}
			}

			IReturnType type = propertyOrEvent.ReturnType;
			if (type == null) return null;
			IClass c = type.GetUnderlyingClass();
			if (c == null) return null;

			if (c.ClassType == ClassType.Enum) {
				foreach (IField f in c.Fields) {
					if (f.Name == expression)
						return new MemberResolveResult(callingClass, null, f);
				}
			}
			return null;
		}
		
		public ArrayList CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			return new ArrayList();
		}
	}
}
