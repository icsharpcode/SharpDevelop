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
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;

namespace ICSharpCode.XamlBinding
{
	// TODO: add [Serializable] support for FastSerializer
	// (SD code may require modifications so that addin assemblies
	// can be found by the deserializer)
	public sealed class XamlUnresolvedFile : IUnresolvedFile
	{
		FileName fileName;
		List<Error> errors;
		IUnresolvedTypeDefinition[] topLevel;
		
		XamlUnresolvedFile(FileName fileName)
		{
			this.fileName = fileName;
			this.errors = new List<Error>();
		}
		
		public static XamlUnresolvedFile Create(FileName fileName, ITextSource fileContent, AXmlDocument document)
		{
			XamlUnresolvedFile file = new XamlUnresolvedFile(fileName);
			ReadOnlyDocument textDocument = new ReadOnlyDocument(fileContent, fileName);
			
			file.errors.AddRange(document.SyntaxErrors.Select(err => new Error(ErrorType.Error, err.Description, textDocument.GetLocation(err.StartOffset))));
			var visitor = new XamlDocumentVisitor(file, textDocument);
			visitor.VisitDocument(document);
			if (visitor.TypeDefinition != null)
				file.topLevel = new[] { visitor.TypeDefinition };
			else
				file.topLevel = new IUnresolvedTypeDefinition[0];
			
			return file;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		DateTime? lastWriteTime;
		
		public DateTime? LastWriteTime {
			get { return lastWriteTime; }
			set { lastWriteTime = value; }
		}
		
		IList<IUnresolvedTypeDefinition> IUnresolvedFile.TopLevelTypeDefinitions {
			get { return topLevel; }
		}
		
		IList<IUnresolvedAttribute> IUnresolvedFile.AssemblyAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		IList<IUnresolvedAttribute> IUnresolvedFile.ModuleAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		public IList<Error> Errors {
			get { return errors; }
		}
		
		IUnresolvedTypeDefinition IUnresolvedFile.GetTopLevelTypeDefinition(TextLocation location)
		{
			foreach (var td in topLevel) {
				if (td.Region.IsInside(location))
					return td;
			}
			return null;
		}
		
		public IUnresolvedTypeDefinition TypeDefinition {
			get { return topLevel.FirstOrDefault(); }
		}
		
		IUnresolvedTypeDefinition IUnresolvedFile.GetInnermostTypeDefinition(TextLocation location)
		{
			return ((IUnresolvedFile)this).GetTopLevelTypeDefinition(location);
		}
		
		public IUnresolvedMember GetMember(TextLocation location)
		{
			var td = ((IUnresolvedFile)this).GetInnermostTypeDefinition(location);
			if (td != null) {
				foreach (var md in td.Members) {
					if (md.Region.IsInside(location))
						return md;
				}
			}
			return null;
		}
		
		public static ITypeReference CreateTypeReference(string @namespace, string localName)
		{
			if (@namespace == null)
				return new UnknownType(null, localName);
			if (@namespace.StartsWith("clr-namespace:", StringComparison.OrdinalIgnoreCase))
				return CreateClrNamespaceTypeReference(@namespace.Substring("clr-namespace:".Length), localName);
			return new XamlTypeReference(@namespace, localName);
		}
		
		public static ITypeReference CreateClrNamespaceTypeReference(string @namespace, string localName)
		{
			if (@namespace == null)
				return new UnknownType(null, localName);
			int assemblyNameIndex = @namespace.IndexOf(";assembly=", StringComparison.OrdinalIgnoreCase);
			IAssemblyReference asm = DefaultAssemblyReference.CurrentAssembly;
			if (assemblyNameIndex > -1) {
				asm = new DefaultAssemblyReference(@namespace.Substring(assemblyNameIndex + ";assembly=".Length));
				@namespace = @namespace.Substring(0, assemblyNameIndex);
			}
			return new GetClassTypeReference(asm, @namespace, localName, 0);
		}
		
		class XamlDocumentVisitor : AXmlVisitor
		{
			public DefaultUnresolvedTypeDefinition TypeDefinition { get; private set; }
			
			IUnresolvedFile file;
			AXmlDocument currentDocument;
			ReadOnlyDocument textDocument;
			
			public XamlDocumentVisitor(IUnresolvedFile file, ReadOnlyDocument textDocument)
			{
				this.file = file;
				this.textDocument = textDocument;
			}
			
			public override void VisitDocument(AXmlDocument document)
			{
				currentDocument = document;
				AXmlElement rootElement = currentDocument.Children.OfType<AXmlElement>().FirstOrDefault();
				if (rootElement != null) {
					string className = rootElement.GetAttributeValue(XamlConst.XamlNamespace, "Class");
					string modifier = rootElement.GetAttributeValue(XamlConst.XamlNamespace, "ClassModifier");
					if (className != null) {
						TypeDefinition = new DefaultUnresolvedTypeDefinition(className) {
							Kind = TypeKind.Class,
							UnresolvedFile = file,
							Accessibility = Accessibility.Public,
							Region = new DomRegion(file.FileName, textDocument.GetLocation(rootElement.StartOffset), textDocument.GetLocation(rootElement.EndOffset))
						};
						TypeDefinition.Members.Add(
							new DefaultUnresolvedMethod(TypeDefinition, "InitializeComponent") {
								Accessibility = Accessibility.Public,
								ReturnType = KnownTypeReference.Void
							});
						TypeDefinition.Members.Add(
							new DefaultUnresolvedField(TypeDefinition, "_contentLoaded") {
								Accessibility = Accessibility.Private,
								ReturnType = KnownTypeReference.Boolean
							});
						
						var connectMember =
							new DefaultUnresolvedMethod(TypeDefinition, "Connect") {
							Accessibility = Accessibility.Private,
							ReturnType = KnownTypeReference.Void
						};
						connectMember.Parameters.Add(new DefaultUnresolvedParameter(KnownTypeReference.Int32, "connectionId"));
						connectMember.Parameters.Add(new DefaultUnresolvedParameter(KnownTypeReference.Object, "target"));
						TypeDefinition.Members.Add(connectMember);
						connectMember.ExplicitInterfaceImplementations.Add(
							new DefaultMemberReference(SymbolKind.Method, new GetClassTypeReference(new FullTypeName(typeof(System.Windows.Markup.IComponentConnector).FullName)), "Connect"));
						
						var browsableAttribute = new DefaultUnresolvedAttribute(new GetClassTypeReference(new FullTypeName(typeof(System.ComponentModel.EditorBrowsableAttribute).FullName)));
						
						browsableAttribute.PositionalArguments.Add(
							new SimpleConstantValue(
								new GetClassTypeReference(new FullTypeName(typeof(System.ComponentModel.EditorBrowsableAttribute).FullName)), System.ComponentModel.EditorBrowsableState.Never
							));
						
						connectMember.Attributes.Add(browsableAttribute);
						TypeDefinition.BaseTypes.Add(CreateTypeReference(rootElement.Namespace, rootElement.LocalName));
						TypeDefinition.BaseTypes.Add(new GetClassTypeReference(new FullTypeName(typeof(System.Windows.Markup.IComponentConnector).FullName)));
						if (modifier != null)
							TypeDefinition.Accessibility = ParseAccessibility(modifier);
					}
				}
				base.VisitDocument(document);
			}
			
			public override void VisitElement(AXmlElement element)
			{
				string name = element.GetAttributeValue(XamlConst.XamlNamespace, "Name") ??
					element.GetAttributeValue("Name");
				string modifier = element.GetAttributeValue(XamlConst.XamlNamespace, "FieldModifier");
				
				if (name != null && TypeDefinition != null) {
					var field = new DefaultUnresolvedField(TypeDefinition, name);
					field.Accessibility = Accessibility.Internal;
					field.ReturnType = CreateTypeReference(element.Namespace, element.LocalName);
					field.Region = new DomRegion(file.FileName, textDocument.GetLocation(element.StartOffset), textDocument.GetLocation(element.EndOffset));
					if (modifier != null)
						field.Accessibility = ParseAccessibility(modifier);
					TypeDefinition.Members.Add(field);
				}
				
				base.VisitElement(element);
			}
			
			Accessibility ParseAccessibility(string value)
			{
				if ("public".Equals(value.Trim(), StringComparison.OrdinalIgnoreCase))
					return Accessibility.Public;
				return Accessibility.Internal;
			}
		}
	}
}
