// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlResolver.
	/// </summary>
	public class XamlResolver
	{
		ReadOnlyDocument textDocument;
		XamlFullParseInformation parseInfo;
		ICompilation compilation;
		TextLocation location;
		int offset;
		CancellationToken cancellationToken;
		
		public XamlResolver()
		{
		}
		
		public ResolveResult Resolve(XamlFullParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			this.parseInfo = parseInfo;
			this.location = location;
			this.compilation = compilation;
			textDocument = new ReadOnlyDocument(parseInfo.Text);
			offset = textDocument.GetOffset(location);
			
			AXmlObject innermost = parseInfo.Document.GetChildAtOffset(offset);
			
			if (innermost is AXmlText)
				return ResolveText((AXmlText)innermost);
			if (innermost is AXmlTag)
				return ResolveTag((AXmlTag)innermost);
			if (innermost is AXmlElement)
				return ResolveElement((AXmlElement)innermost);
			if (innermost is AXmlAttribute)
				return ResolveAttribute((AXmlAttribute)innermost);
			return ErrorResolveResult.UnknownError;
		}
		
		ResolveResult ResolveText(AXmlText text)
		{
			return ErrorResolveResult.UnknownError;
		}
		
		ResolveResult ResolveAttribute(AXmlAttribute attribute)
		{
			IMember member = null;
			IType type = null;
			string namespaceName = string.IsNullOrEmpty(attribute.Namespace) ? attribute.ParentElement.Namespace : attribute.Namespace;
			string propertyName = attribute.LocalName;
			
			if (propertyName.Contains(".")) {
				string name = propertyName.Substring(0, propertyName.IndexOf('.'));
				propertyName = propertyName.Substring(propertyName.IndexOf('.') + 1);
				ITypeReference reference = CreateTypeReference(namespaceName, name);
				type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				member = FindMember(type, propertyName, true);
			} else {
				ITypeReference reference = CreateTypeReference(namespaceName, attribute.ParentElement.LocalName);
				type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				member = FindMember(type, propertyName, false);
			}
			if (member == null)
				return new UnknownMemberResolveResult(type, propertyName, EmptyList<IType>.Instance);
			
			if (attribute.ValueSegment.Contains(offset, 1))
				return ResolveAttributeValue(member, attribute);
			return new MemberResolveResult(null, member);
		}
		
		ResolveResult ResolveTag(AXmlTag tag)
		{
			if (!tag.IsStartOrEmptyTag && !tag.IsEndTag)
				return ErrorResolveResult.UnknownError;
			return ResolveElement(tag.Ancestors.OfType<AXmlElement>().First());
		}

		ResolveResult ResolveElement(AXmlElement element)
		{
			string namespaceName = element.Namespace;
			string name = element.LocalName;

			if (name.Contains(".")) {
				string propertyName = name.Substring(name.IndexOf('.') + 1);
				name = name.Substring(0, name.IndexOf('.'));
				ITypeReference reference = CreateTypeReference(namespaceName, name);
				IType type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				IMember member = FindMember(type, propertyName, true);
				if (member == null)
					return new UnknownMemberResolveResult(type, propertyName, EmptyList<IType>.Instance);
				return new MemberResolveResult(null, member);
			} else {
				ITypeReference reference = CreateTypeReference(namespaceName, name);
				IType type = reference.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				return new TypeResolveResult(type);
			}
		}

		IMember FindMember(IType type, string propertyName, bool allowAttached)
		{
			IMember member = type.GetProperties(p => p.Name == propertyName).FirstOrDefault();
			if (member != null)
				return member;
			member = type.GetEvents(e => e.Name == propertyName).FirstOrDefault();
			if (member != null)
				return member;
			
			if (allowAttached) {
				IMethod method = type.GetMethods(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 1 && m.Name == "Get" + propertyName).FirstOrDefault();
				ITypeDefinition td = type.GetDefinition();
				if (method != null)
					return new DefaultUnresolvedProperty { Name = propertyName }.CreateResolved(new SimpleTypeResolveContext(td));
				method = type.GetMethods(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 2 && (m.Name == "Add" + propertyName + "Handler" || m.Name == "Remove" + propertyName + "Handler")).FirstOrDefault();
				if (method != null)
					return new DefaultUnresolvedEvent { Name = propertyName }.CreateResolved(new SimpleTypeResolveContext(td));
			}
			return null;
		}
		
		ResolveResult ResolveAttributeValue(IMember propertyOrEvent, AXmlAttribute attribute)
		{
			ITypeDefinition type = parseInfo.ParsedFile.GetTopLevelTypeDefinition(location).Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
			if (type == null)
				return ErrorResolveResult.UnknownError;
			if (propertyOrEvent is IEvent) {
				var rr = new MemberLookup(type, type.ParentAssembly, false).Lookup(new ThisResolveResult(type), attribute.Value, EmptyList<IType>.Instance, false);
				if (rr is MethodGroupResolveResult) {
					Conversion conversion = Conversions.Get(type.Compilation).ImplicitConversion(rr, propertyOrEvent.ReturnType);
					IMethod method = conversion.Method;
					if (method == null)
						method = ((MethodGroupResolveResult)rr).Methods.FirstOrDefault();
					if (method != null)
						return new MemberResolveResult(null, method);
				}
				return rr;
			} else {
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
			}
			return new UnknownMemberResolveResult(type, attribute.Value, EmptyList<IType>.Instance);
		}
		
		ITypeReference CreateTypeReference(string @namespace, string localName)
		{
			if (@namespace.StartsWith("clr-namespace:", StringComparison.OrdinalIgnoreCase)) {
				return CreateClrNamespaceTypeReference(@namespace.Substring("clr-namespace:".Length), localName);
			}
			return new XamlTypeReference(@namespace, localName);
		}
		
		ITypeReference CreateClrNamespaceTypeReference(string @namespace, string localName)
		{
			int assemblyNameIndex = @namespace.IndexOf(";assembly=", StringComparison.OrdinalIgnoreCase);
			IAssemblyReference asm = DefaultAssemblyReference.CurrentAssembly;
			if (assemblyNameIndex > -1) {
				asm = new DefaultAssemblyReference(@namespace.Substring(assemblyNameIndex + ";assembly=".Length));
				@namespace = @namespace.Substring(0, assemblyNameIndex);
			}
			return new GetClassTypeReference(asm, @namespace, localName, 0);
		}
	}

	public class XamlTypeReference : ITypeReference
	{
		string localName;
		string xmlNamespace;
		
		const string XmlnsDefinitionAttribute = "System.Windows.Markup.XmlnsDefinitionAttribute";
		
		public XamlTypeReference(string xmlNamespace, string localName)
		{
			this.localName = localName;
			this.xmlNamespace = xmlNamespace;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			foreach (var asm in context.Compilation.Assemblies) {
				foreach (var attr in asm.AssemblyAttributes.Where(a => a.AttributeType.FullName == XmlnsDefinitionAttribute
				                                                  && a.PositionalArguments.Count == 2)) {
					ConstantResolveResult crr = attr.PositionalArguments[0] as ConstantResolveResult;
					ConstantResolveResult crr2 = attr.PositionalArguments[1] as ConstantResolveResult;
					if (crr == null || crr2 == null)
						continue;
					string namespaceName = crr2.ConstantValue as string;
					if (xmlNamespace.Equals(crr.ConstantValue as string, StringComparison.OrdinalIgnoreCase) && namespaceName != null) {
						ITypeDefinition td = asm.GetTypeDefinition(namespaceName, localName, 0);
						if (td != null)
							return td;
					}
				}
			}
			
			return new UnknownType(null, localName, 0);
		}
	}
}
