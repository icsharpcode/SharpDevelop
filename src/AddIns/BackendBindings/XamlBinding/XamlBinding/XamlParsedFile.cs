// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	public sealed class XamlParsedFile : IParsedFile
	{
		FileName fileName;
		AXmlDocument document;
		List<Error> errors;
		IUnresolvedTypeDefinition[] topLevel;
		
		XamlParsedFile(FileName fileName, AXmlDocument document)
		{
			this.fileName = fileName;
			this.document = document;
			this.errors = new List<Error>();
		}
		
		public static XamlParsedFile Create(FileName fileName, ITextSource fileContent, AXmlDocument document)
		{
			XamlParsedFile file = new XamlParsedFile(fileName, document);
			
			file.errors.AddRange(document.SyntaxErrors.Select(err => new Error(ErrorType.Error, err.Description)));
			var visitor = new XamlDocumentVisitor(file, fileContent);
			visitor.VisitDocument(document);
			file.topLevel = new[] { visitor.TypeDefinition };
			
			file.lastWriteTime = DateTime.UtcNow;
			return file;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		DateTime lastWriteTime = DateTime.UtcNow;
		
		public DateTime LastWriteTime {
			get { return lastWriteTime; }
		}
		
		public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return topLevel; }
		}
		
		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		public IList<Error> Errors {
			get { return errors; }
		}
		
		public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(TextLocation location)
		{
			foreach (var td in topLevel) {
				if (td.Region.IsInside(location))
					return td;
			}
			return null;
		}
		
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(TextLocation location)
		{
			return GetTopLevelTypeDefinition(location);
		}
		
		public IUnresolvedMember GetMember(TextLocation location)
		{
			var td = GetInnermostTypeDefinition(location);
			if (td != null) {
				foreach (var md in td.Members) {
					if (md.Region.IsInside(location))
						return md;
				}
			}
			return null;
		}
		
		public ITypeResolveContext GetTypeResolveContext(ICompilation compilation, TextLocation loc)
		{
			throw new NotImplementedException();
		}
		
		class XamlDocumentVisitor : AXmlVisitor
		{
			public DefaultUnresolvedTypeDefinition TypeDefinition { get; private set; }
			
			IParsedFile file;
			AXmlDocument currentDocument;
			ReadOnlyDocument textDocument;
			
			public XamlDocumentVisitor(IParsedFile file, ITextSource fileContent)
			{
				this.file = file;
				textDocument = new ReadOnlyDocument(fileContent);
			}
			
			public override void VisitDocument(AXmlDocument document)
			{
				currentDocument = document;
				AXmlElement rootElement = currentDocument.Children.OfType<AXmlElement>().FirstOrDefault();
				if (rootElement != null) {
					string className = rootElement.GetAttributeValue(XamlBehavior.XamlNamespace, "Class");
					if (className != null) {
						TypeDefinition = new DefaultUnresolvedTypeDefinition(className) {
							Kind = TypeKind.Class,
							ParsedFile = file,
							Region = new DomRegion(file.FileName, textDocument.GetLocation(rootElement.StartOffset), textDocument.GetLocation(rootElement.EndOffset))
						};
						TypeDefinition.Members.Add(
							new DefaultUnresolvedMethod(TypeDefinition, "InitializeComponent") {
								Accessibility = Accessibility.Public,
								ReturnType = KnownTypeReference.Void
							});
					}
				}
				base.VisitDocument(document);
			}
			
			public override void VisitElement(AXmlElement element)
			{
				string name = element.GetAttributeValue(XamlBehavior.XamlNamespace, "Name") ??
					element.GetAttributeValue("Name");
				string modifier = element.GetAttributeValue(XamlBehavior.XamlNamespace, "FieldModifier");
				
				if (name != null) {
					var field = new DefaultUnresolvedField(TypeDefinition, name);
					field.Accessibility = Accessibility.Internal;
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
