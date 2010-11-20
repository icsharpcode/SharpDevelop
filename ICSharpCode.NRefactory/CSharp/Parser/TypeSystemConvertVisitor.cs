// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Produces type and member declarations.
	/// </summary>
	public class TypeSystemConvertVisitor : AbstractDomVisitor<object, IEntity>
	{
		readonly ParsedFile parsedFile;
		UsingScope usingScope;
		DefaultTypeDefinition currentTypeDefinition;
		
		public TypeSystemConvertVisitor(IProjectContent pc, string fileName)
		{
			this.parsedFile = new ParsedFile(fileName, new UsingScope(pc));
			this.usingScope = parsedFile.RootUsingScope;
		}
		
		public ParsedFile ParsedFile {
			get { return parsedFile; }
		}
		
		DomRegion MakeRegion(DomLocation start, DomLocation end)
		{
			return new DomRegion(parsedFile.FileName, start.Line, start.Column, end.Line, end.Column);
		}
		
		// TODO: extern aliases
		
		public override IEntity VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			ITypeOrNamespaceReference r = null;
			foreach (Identifier identifier in usingDeclaration.NameIdentifier.NameParts) {
				if (r == null) {
					// TODO: alias?
					r = new SimpleTypeOrNamespaceReference(identifier.Name, null, currentTypeDefinition, usingScope, true);
				} else {
					r = new MemberTypeOrNamespaceReference(r, identifier.Name, null, currentTypeDefinition, usingScope);
				}
			}
			if (r != null)
				usingScope.Usings.Add(r);
			return null;
		}
		
		public override IEntity VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public override IEntity VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			DomRegion region = MakeRegion(namespaceDeclaration.StartLocation, namespaceDeclaration.EndLocation);
			UsingScope previousUsingScope = usingScope;
			foreach (Identifier ident in namespaceDeclaration.NameIdentifier.NameParts) {
				usingScope = new UsingScope(usingScope, NamespaceDeclaration.BuildQualifiedName(usingScope.NamespaceName, ident.Name));
				usingScope.Region = region;
			}
			base.VisitNamespaceDeclaration(namespaceDeclaration, data);
			parsedFile.UsingScopes.Add(usingScope); // add after visiting children so that nested scopes come first
			usingScope = previousUsingScope;
			return null;
		}
		
		// TODO: assembly attributes
		
		DefaultTypeDefinition CreateTypeDefinition(string name)
		{
			if (currentTypeDefinition != null) {
				return new DefaultTypeDefinition(currentTypeDefinition, name);
			} else {
				DefaultTypeDefinition newType = new DefaultTypeDefinition(usingScope.ProjectContent, usingScope.NamespaceName, name);
				parsedFile.TopLevelTypeDefinitions.Add(newType);
				return newType;
			}
		}
		
		public override IEntity VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			var td = currentTypeDefinition = CreateTypeDefinition(typeDeclaration.Name);
			td.ClassType = typeDeclaration.ClassType;
			td.Region = MakeRegion(typeDeclaration.StartLocation, typeDeclaration.EndLocation);
			td.BodyRegion = MakeRegion(typeDeclaration.LBrace.StartLocation, typeDeclaration.RBrace.EndLocation);
			td.AddDefaultConstructorIfRequired = true;
			
			ConvertAttributes(td.Attributes, typeDeclaration.Attributes);
			// TODO: modifiers
			
			//ConvertTypeParameters(td.TypeParameters, typeDeclaration.TypeArguments, typeDeclaration.Constraints, td);
			
			// TODO: base type references?
			
			// TODO: members
			
			currentTypeDefinition = (DefaultTypeDefinition)currentTypeDefinition.DeclaringTypeDefinition;
			return td;
		}
		
		public override IEntity VisitEnumDeclaration(EnumDeclaration enumDeclaration, object data)
		{
			return VisitTypeDeclaration(enumDeclaration, data);
		}
		
		void ConvertAttributes(IList<IAttribute> outputList, IEnumerable<AttributeSection> attributes)
		{
			foreach (AttributeSection section in attributes) {
				throw new NotImplementedException();
			}
		}
	}
}
