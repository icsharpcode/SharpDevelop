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
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;

namespace ICSharpCode.XamlBinding
{
	public class XamlAstResolver
	{
		ReadOnlyDocument textDocument;
		XamlFullParseInformation parseInfo;
		ICompilation compilation;
		
		public XamlAstResolver(ICompilation compilation, XamlFullParseInformation parseInfo)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			if (parseInfo == null)
				throw new ArgumentNullException("parseInfo");
			this.compilation = compilation;
			this.parseInfo = parseInfo;
			this.textDocument = new ReadOnlyDocument(parseInfo.Text, parseInfo.FileName);
		}
		
		public ResolveResult ResolveAtLocation(TextLocation location, CancellationToken cancellationToken = default(CancellationToken))
		{
			int offset = textDocument.GetOffset(location);
			var line = textDocument.GetLineByNumber(location.Line);
			
			if (offset == line.EndOffset)
				return ErrorResolveResult.UnknownError;
			if (char.IsWhiteSpace(textDocument.GetCharAt(offset)))
				return ErrorResolveResult.UnknownError;
			
			AXmlObject innermost = parseInfo.Document.GetChildAtOffset(offset);
			if (innermost is AXmlText)
				return ResolveText((AXmlText)innermost, cancellationToken);
			if (innermost is AXmlTag)
				return ResolveTag((AXmlTag)innermost, cancellationToken);
			if (innermost is AXmlElement)
				return ResolveElement((AXmlElement)innermost, cancellationToken);
			if (innermost is AXmlAttribute)
				return ResolveAttribute((AXmlAttribute)innermost, offset, cancellationToken);
			return ErrorResolveResult.UnknownError;
		}
		
		ResolveResult ResolveText(AXmlText text, CancellationToken cancellationToken)
		{
			return ErrorResolveResult.UnknownError;
		}
		
		internal ResolveResult ResolveAttribute(AXmlAttribute attribute, int offset = -1, CancellationToken cancellationToken = default(CancellationToken))
		{
			IMember member = null, underlying = null;
			IType type = null;
			if (attribute.Namespace == XamlConst.XamlNamespace) {
				switch (attribute.LocalName) {
					case "Name":
						IUnresolvedTypeDefinition typeDefinition = parseInfo.UnresolvedFile.TypeDefinition;
						if (typeDefinition != null) {
							type = typeDefinition.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
							IField field = type.GetFields(f => f.Name == attribute.Value).FirstOrDefault();
							if (field != null)
								return new MemberResolveResult(null, field);
						}
						break;
					case "Class":
						type = compilation.FindType(new FullTypeName(attribute.Value));
						return new TypeResolveResult(type);
				}
			}
			if (attribute.ParentElement == null)
				return ErrorResolveResult.UnknownError;
			
			string propertyName = attribute.LocalName;
			if (propertyName.Contains(".")) {
				string namespaceName = string.IsNullOrEmpty(attribute.Namespace) ? attribute.ParentElement.LookupNamespace("") : attribute.Namespace;
				string name = propertyName.Substring(0, propertyName.IndexOf('.'));
				propertyName = propertyName.Substring(propertyName.IndexOf('.') + 1);
				ITypeReference reference = XamlUnresolvedFile.CreateTypeReference(namespaceName, name);
				type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				member = FindMember(type, propertyName);
				if (member == null)
					member = FindAttachedMember(type, propertyName, out underlying);
			} else {
				ITypeReference reference = XamlUnresolvedFile.CreateTypeReference(attribute.ParentElement.Namespace, attribute.ParentElement.LocalName);
				type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				member = FindMember(type, propertyName);
			}
			if (member == null)
				return new UnknownMemberResolveResult(type, propertyName, EmptyList<IType>.Instance);
			if (offset > -1 && attribute.ValueSegment.Contains(offset, 1))
				return ResolveAttributeValue(member, attribute, cancellationToken);
			if (underlying != null)
				return new AttachedMemberResolveResult(new TypeResolveResult(underlying.DeclaringType), underlying, member);
			else
				return new MemberResolveResult(new TypeResolveResult(member.DeclaringType), member);
		}
		
		ResolveResult ResolveTag(AXmlTag tag, CancellationToken cancellationToken)
		{
			if (!tag.IsStartOrEmptyTag && !tag.IsEndTag)
				return ErrorResolveResult.UnknownError;
			return ResolveElement(tag.Ancestors.OfType<AXmlElement>().First(), cancellationToken);
		}
		
		internal ResolveResult ResolveElement(AXmlElement element, CancellationToken cancellationToken = default(CancellationToken))
		{
			string namespaceName = element.Namespace;
			string name = element.LocalName;
			if (name.Contains(".")) {
				string propertyName = name.Substring(name.IndexOf('.') + 1);
				name = name.Substring(0, name.IndexOf('.'));
				ITypeReference reference = XamlUnresolvedFile.CreateTypeReference(namespaceName, name);
				IType type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				IMember member = FindMember(type, propertyName);
				IMember underlying = null;
				if (member == null)
					member = FindAttachedMember(type, propertyName, out underlying);
				if (member == null)
					return new UnknownMemberResolveResult(type, propertyName, EmptyList<IType>.Instance);
				if (underlying != null)
					return new AttachedMemberResolveResult(new TypeResolveResult(type), underlying, member);
				return new MemberResolveResult(new TypeResolveResult(type), member);
			} else {
				ITypeReference reference = XamlUnresolvedFile.CreateTypeReference(namespaceName, name);
				IType type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				return new TypeResolveResult(type);
			}
		}
		
		IMember FindMember(IType type, string propertyName)
		{
			IMember member = type.GetProperties(p => p.Name == propertyName).FirstOrDefault();
			if (member != null)
				return member;
			return type.GetEvents(e => e.Name == propertyName).FirstOrDefault();
		}
		
		IMember FindAttachedMember(IType type, string propertyName, out IMember underlyingMember)
		{
			underlyingMember = type
				.GetMethods(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 1
				            && m.Name == "Get" + propertyName)
				.FirstOrDefault();
			var definition = type.GetDefinition();
			if (definition != null) {
				ITypeResolveContext localContext = new SimpleTypeResolveContext(definition);
				if (underlyingMember != null)
					return new DefaultUnresolvedProperty { Name = propertyName }
				.CreateResolved(localContext);
				
				underlyingMember = type
					.GetMethods(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 2
					            && m.Name == "Add" + propertyName + "Handler")
					.FirstOrDefault();
				if (underlyingMember != null)
					return new DefaultUnresolvedEvent { Name = propertyName }
				.CreateResolved(localContext);
			}
			return null;
		}
		
		ResolveResult ResolveAttributeValue(IMember propertyOrEvent, AXmlAttribute attribute, CancellationToken cancellationToken)
		{
			IUnresolvedTypeDefinition typeDefinition = parseInfo.UnresolvedFile.TypeDefinition;
			if (typeDefinition != null) {
				IType type = typeDefinition.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				if (propertyOrEvent is IEvent) {
					var memberLookup = new MemberLookup(type.GetDefinition(), compilation.MainAssembly);
					var rr = memberLookup.Lookup(new ThisResolveResult(type), attribute.Value, EmptyList<IType>.Instance, false);
					if (rr is MethodGroupResolveResult) {
						Conversion conversion = CSharpConversions.Get(compilation).ImplicitConversion(rr, propertyOrEvent.ReturnType);
						IMethod method = conversion.Method;
						if (method == null)
							method = ((MethodGroupResolveResult)rr).Methods.FirstOrDefault();
						if (method != null)
							return new MemberResolveResult(null, method);
					}
					return rr;
				}
				if (propertyOrEvent.Name == "Name") {
					IField field = type.GetFields(f => f.Name == attribute.Value).FirstOrDefault();
					if (field != null)
						return new MemberResolveResult(null, field);
				}
				if (propertyOrEvent.ReturnType.Kind == TypeKind.Enum) {
					IField field = propertyOrEvent.ReturnType.GetFields(f => f.Name == attribute.Value).FirstOrDefault();
					if (field != null)
						return new MemberResolveResult(null, field);
				}
				// TODO might be a markup extension
				return new UnknownMemberResolveResult(type, attribute.Value, EmptyList<IType>.Instance);
			}
			return ErrorResolveResult.UnknownError;
		}
	}
}
