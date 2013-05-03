// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlResolver
	{
		ITypeResolveContext resolveContext;
		
		public XamlResolver(ICompilation compilation)
		{
			this.resolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);
		}
		
		public IType ResolveType(string @namespace, string type)
		{
			return XamlUnresolvedFile.CreateTypeReference(@namespace, type).Resolve(resolveContext);
		}
		
		public ResolveResult ResolveExpression(string expression, XamlContext context)
		{
			string prefix, memberName;
			string name = ParseName(expression, out prefix, out memberName);
			string namespaceUrl;
			if (prefix == "")
				namespaceUrl = context.ActiveElement.Namespace;
			else
				namespaceUrl = context.ActiveElement.ResolvePrefix(prefix);
			if (string.IsNullOrEmpty(memberName)) {
				IType type = ResolveType(namespaceUrl, context.ActiveElement.LocalName);
				IMember member = type.GetMembers(m => m.Name == name).FirstOrDefault();
				if (member == null) {
					type = ResolveType(namespaceUrl, name);
					return new TypeResolveResult(type);
				} else {
					return new MemberResolveResult(new TypeResolveResult(type), member);
				}
			} else {
				IType type = ResolveType(namespaceUrl, name);
				IMember member = type.GetMembers(m => m.Name == memberName).FirstOrDefault();
				if (member == null)
					return new UnknownMemberResolveResult(type, memberName, EmptyList<IType>.Instance);
				return new MemberResolveResult(new TypeResolveResult(type), member);
			}
		}
		
		public ResolveResult ResolveAttributeValue(XamlContext context, AttributeValue value)
		{
			string typeNameString = value.IsString
				? value.StringValue
				: GetTypeNameFromTypeExtension(value.ExtensionValue, context);
			return ResolveExpression(typeNameString, context);
		}
		
		public ResolveResult ResolveAttributeValue(XamlContext context, AttributeValue value, out string typeNameString)
		{
			typeNameString = value.IsString
				? value.StringValue
				: GetTypeNameFromTypeExtension(value.ExtensionValue, context);
			return ResolveExpression(typeNameString, context);
		}
		
		string GetTypeNameFromTypeExtension(MarkupExtensionInfo info, XamlContext context)
		{
			ResolveResult rr = ResolveExpression(info.ExtensionType, context)
				?? ResolveExpression(info.ExtensionType + "Extension", context);
			
			IType type = rr.Type;
			
			if (type == null || type.FullName != "System.Windows.Markup.TypeExtension")
				return string.Empty;
			
			var item = info.PositionalArguments.FirstOrDefault();
			if (item != null && item.IsString)
				return item.StringValue;
			if (info.NamedArguments.TryGetValue("typename", out item)) {
				if (item.IsString)
					return item.StringValue;
			}
			
			return string.Empty;
		}
		
		internal static string ParseName(string expression, out string prefix, out string member)
		{
			int colonPos = expression.IndexOf(':');
			if (colonPos > 0)
				prefix = expression.Substring(0, colonPos);
			else
				prefix = "";
			expression = expression.Remove(0, colonPos + 1);
			int dotPos = expression.IndexOf('.');
			if (dotPos >= 0) {
				member = expression.Substring(dotPos + 1);
				return expression.Remove(dotPos);
			} else {
				member = "";
				return expression;
			}
		}
	}
}
