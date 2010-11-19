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
		readonly string fileName;
		UsingScope usingScope;
		DefaultTypeDefinition currentTypeDefinition;
		
		public TypeSystemConvertVisitor(IProjectContent pc, string fileName)
		{
			this.usingScope = new UsingScope(pc);
			this.fileName = fileName;
		}
		
		DomRegion MakeRegion(DomLocation start, DomLocation end)
		{
			return new DomRegion(fileName, start.Line, start.Column, end.Line, end.Column);
		}
		
		// TODO: extern aliases
		
		public override IEntity VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public override IEntity VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration, object data)
		{
			throw new NotImplementedException();
		}
		
		public override IEntity VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			UsingScope previousUsingScope = usingScope;
			foreach (Identifier ident in namespaceDeclaration.NameIdentifier.NameParts) {
				usingScope = new UsingScope(usingScope, NamespaceDeclaration.BuildQualifiedName(usingScope.NamespaceName, ident.Name));
			}
			base.VisitNamespaceDeclaration(namespaceDeclaration, data);
			usingScope = previousUsingScope;
			return null;
		}
		
		// TODO: assembly attributes
		
		DefaultTypeDefinition CreateTypeDefinition(string name)
		{
			if (currentTypeDefinition != null)
				return new DefaultTypeDefinition(currentTypeDefinition, name);
			else
				return new DefaultTypeDefinition(usingScope.ProjectContent, usingScope.NamespaceName, name);
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
			throw new NotImplementedException();
		}
	}
}
