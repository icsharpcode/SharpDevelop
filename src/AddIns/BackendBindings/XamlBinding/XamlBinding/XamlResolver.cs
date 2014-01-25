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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

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
			string namespaceUrl = context.ActiveElement.LookupNamespace(prefix);
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
		
		public ResolveResult ResolveAttributeValue(string expression, XamlContext context)
		{
			if (!context.InAttributeValueOrMarkupExtension)
				return ErrorResolveResult.UnknownError;
			if (context.AttributeValue.IsString)
				return ResolveExpression(expression, context);
			MarkupExtensionInfo info = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset);
			object data = Utils.GetMarkupDataAtPosition(info, context.ValueStartOffset);
			
			IType extensionType = ResolveExpression(info.ExtensionType, context).Type;
			if (extensionType.Kind == TypeKind.Unknown)
				extensionType = ResolveExpression(info.ExtensionType + "Extension", context).Type;
			
			if (data is KeyValuePair<string, AttributeValue>) {
				var kv = (KeyValuePair<string, AttributeValue>)data;
				if (kv.Value.StartOffset >= context.ValueStartOffset) {
					IProperty member = extensionType.GetProperties(p => p.Name == expression).FirstOrDefault();
					if (member != null)
						return new MemberResolveResult(new TypeResolveResult(extensionType), member);
					return new UnknownMemberResolveResult(extensionType, expression, EmptyList<IType>.Instance);
				} else {
					
				}
			}
			
			return ResolveExpression(expression, context);
		}
		
		string GetTypeNameFromTypeExtension(MarkupExtensionInfo info, XamlContext context)
		{
			IType type = ResolveExpression(info.ExtensionType, context).Type;
			if (type.Kind == TypeKind.Unknown)
				type = ResolveExpression(info.ExtensionType + "Extension", context).Type;
			
			if (type.FullName != "System.Windows.Markup.TypeExtension")
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
