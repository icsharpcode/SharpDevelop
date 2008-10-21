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

		bool IsReaderAtTarget(XmlTextReader r)
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
						return ResolveElementName(r);
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

		ResolveResult ResolveElementName(XmlReader r)
		{
			string xmlNamespace;
			string name;
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

		public ArrayList CtrlSpace(int caretLineNumber, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext expressionContext)
		{
			this.parseInfo = parseInfo;
			this.caretLineNumber = caretLineNumber;
			this.caretColumn = caretColumn;
			this.callingClass = parseInfo.BestCompilationUnit.GetInnermostClass(caretLineNumber, caretColumn);
			this.context = expressionContext as XamlExpressionContext;
			if (context == null) {
				return null;
			}

			if (context.AttributeName == null) {
				return CtrlSpaceForElement(fileContent);
			}
			else if (context.InAttributeValue) {
				return CtrlSpaceForAttributeValue(fileContent, context);
			}
			else {
				return CtrlSpaceForAttributeName(fileContent, context);
			}
		}

		ArrayList CtrlSpaceForAttributeName(string fileContent, XamlExpressionContext context)
		{
			if (context.ElementPath.Elements.Count == 0)
				return null;
			QualifiedName lastElement = context.ElementPath.Elements[context.ElementPath.Elements.Count - 1];
			XamlCompilationUnit cu = parseInfo.BestCompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return null;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.Name);
			if (rt == null)
				return null;
			ArrayList list = new ArrayList();
			foreach (IProperty p in rt.GetProperties()) {
				if (p.IsPublic && p.CanSet) {
					list.Add(p);
				}
			}
			return list;
		}

		ArrayList CtrlSpaceForAttributeValue(string fileContent, XamlExpressionContext context)
		{
			ArrayList attributes = CtrlSpaceForAttributeName(fileContent, context);
			if (attributes != null) {
				foreach (IProperty p in attributes.OfType<IProperty>()) {
					if (p.Name == context.AttributeName && p.ReturnType != null) {
						IClass c = p.ReturnType.GetUnderlyingClass();
						if (c != null && c.ClassType == ClassType.Enum) {
							return EnumCompletion(c);
						}
					}
				}
			}
			return null;
		}

		ArrayList EnumCompletion(IClass enumClass)
		{
			ArrayList arr = new ArrayList();
			foreach (IField f in enumClass.Fields) {
				arr.Add(f);
			}
			return arr;
		}

		ArrayList CtrlSpaceForElement(string fileContent)
		{
			using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
				try {
					r.WhitespaceHandling = WhitespaceHandling.Significant;
					// move reader to correct position
					while (r.Read() && !IsReaderAtTarget(r)) { }
				}
				catch (XmlException) {
				}
				ArrayList result = new ArrayList();
				IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;

				resolveExpression = r.Name;
				TypeResolveResult rr = ResolveElementName(r) as TypeResolveResult;
				if (rr != null) {
					AddPropertiesForType(result, r, rr);
				}

				foreach (var ns in r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)) {
					ArrayList list = XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value);
					if (list != null) {
						foreach (IClass c in list.OfType<IClass>()) {
							if (c.ClassType != ClassType.Class)
								continue;
							if (c.IsAbstract && c.IsStatic)
								continue;
							if (c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute"))
								continue;
							if (!c.Methods.Any(m => m.IsConstructor && m.IsPublic))
								continue;
							if (string.IsNullOrEmpty(ns.Key))
								result.Add(c);
							else
								result.Add(new XamlCompletionClass(c, ns.Key));
						}
					}
				}
				return result;
			}
		}

		void AddPropertiesForType(ArrayList result, XmlTextReader r, TypeResolveResult rr)
		{
			if (rr.ResolvedType != null) {
				foreach (IProperty p in rr.ResolvedType.GetProperties()) {
					if (!p.IsPublic)
						continue;
					if (!p.CanSet && !IsCollectionType(p.ReturnType))
						continue;
					string propPrefix = p.DeclaringType.Name;
					if (!string.IsNullOrEmpty(r.Prefix))
						propPrefix = r.Prefix + ":" + propPrefix;
					result.Add(new XamlCompletionProperty(p, propPrefix));
				}
			}
		}

		bool IsCollectionType(IReturnType rt)
		{
			if (rt == null)
				return false;
			return rt.GetMethods().Any(m => m.Name == "Add" && m.IsPublic);
		}

		class XamlCompletionClass : DefaultClass, IEntity
		{
			string newName;

			public XamlCompletionClass(IClass baseClass, string prefix)
				: base(baseClass.CompilationUnit, baseClass.FullyQualifiedName)
			{
				this.Modifiers = baseClass.Modifiers;
				newName = prefix + ":" + baseClass.Name;
			}

			string IEntity.Name
			{
				get { return newName; }
			}
		}

		class XamlCompletionProperty : DefaultProperty, IEntity
		{
			string newName;

			public XamlCompletionProperty(IProperty baseProperty, string prefix)
				: base(baseProperty.DeclaringType, baseProperty.Name)
			{
				this.Modifiers = baseProperty.Modifiers;
				newName = prefix + "." + baseProperty.Name;
			}

			string IEntity.Name
			{
				get { return newName; }
			}
		}
	}
}
