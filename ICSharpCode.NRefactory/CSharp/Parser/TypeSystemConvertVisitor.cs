// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Produces type and member definitions from the DOM.
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
		
		#region Using Declarations
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
		#endregion
		
		#region Namespace Declaration
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
		#endregion
		
		// TODO: assembly attributes
		
		#region Type Definitions
		DefaultTypeDefinition CreateTypeDefinition(string name)
		{
			DefaultTypeDefinition newType;
			if (currentTypeDefinition != null) {
				newType = new DefaultTypeDefinition(currentTypeDefinition, name);
				currentTypeDefinition.InnerClasses.Add(newType);
			} else {
				newType = new DefaultTypeDefinition(usingScope.ProjectContent, usingScope.NamespaceName, name);
				parsedFile.TopLevelTypeDefinitions.Add(newType);
			}
			return newType;
		}
		
		public override IEntity VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			var td = currentTypeDefinition = CreateTypeDefinition(typeDeclaration.Name);
			td.ClassType = typeDeclaration.ClassType;
			td.Region = MakeRegion(typeDeclaration.StartLocation, typeDeclaration.EndLocation);
			td.BodyRegion = MakeRegion(typeDeclaration.LBrace.StartLocation, typeDeclaration.RBrace.EndLocation);
			td.AddDefaultConstructorIfRequired = true;
			
			ConvertAttributes(td.Attributes, typeDeclaration.Attributes);
			ApplyModifiers(td, typeDeclaration.Modifiers);
			if (td.ClassType == ClassType.Interface)
				td.IsAbstract = true; // interfaces are implicitly abstract
			else if (td.ClassType == ClassType.Enum || td.ClassType == ClassType.Struct)
				td.IsSealed = true; // enums/structs are implicitly sealed
			
			//TODO ConvertTypeParameters(td.TypeParameters, typeDeclaration.TypeArguments, typeDeclaration.Constraints, td);
			
			// TODO: base type references?
			
			foreach (AbstractMemberBase member in typeDeclaration.Members) {
				member.AcceptVisitor(this, data);
			}
			
			currentTypeDefinition = (DefaultTypeDefinition)currentTypeDefinition.DeclaringTypeDefinition;
			return td;
		}
		
		public override IEntity VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			var td = CreateTypeDefinition(delegateDeclaration.Name);
			td.ClassType = ClassType.Delegate;
			td.Region = MakeRegion(delegateDeclaration.StartLocation, delegateDeclaration.EndLocation);
			td.BaseTypes.Add(multicastDelegateReference);
			
			ConvertAttributes(td.Attributes, delegateDeclaration.Attributes);
			ApplyModifiers(td, delegateDeclaration.Modifiers);
			td.IsSealed = true; // delegates are implicitly sealed
			
			// TODO: convert return type, convert parameters
			AddDefaultMethodsToDelegate(td, SharedTypes.UnknownType, EmptyList<IParameter>.Instance);
			
			return td;
		}
		
		static readonly ITypeReference multicastDelegateReference = typeof(MulticastDelegate).ToTypeReference();
		static readonly IParameter delegateObjectParameter = MakeParameter(TypeCode.Object.ToTypeReference(), "object");
		static readonly IParameter delegateIntPtrMethodParameter = MakeParameter(typeof(IntPtr).ToTypeReference(), "method");
		static readonly IParameter delegateAsyncCallbackParameter = MakeParameter(typeof(AsyncCallback).ToTypeReference(), "callback");
		static readonly IParameter delegateResultParameter = MakeParameter(typeof(IAsyncResult).ToTypeReference(), "result");
		
		static IParameter MakeParameter(ITypeReference type, string name)
		{
			DefaultParameter p = new DefaultParameter(type, name);
			p.Freeze();
			return p;
		}
		
		/// <summary>
		/// Adds the 'Invoke', 'BeginInvoke', 'EndInvoke' methods, and a constructor, to the <see cref="delegateType"/>.
		/// </summary>
		public static void AddDefaultMethodsToDelegate(DefaultTypeDefinition delegateType, ITypeReference returnType, IEnumerable<IParameter> parameters)
		{
			if (delegateType == null)
				throw new ArgumentNullException("delegateType");
			if (returnType == null)
				throw new ArgumentNullException("returnType");
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			
			DomRegion region = new DomRegion(delegateType.Region.FileName, delegateType.Region.BeginLine, delegateType.Region.BeginColumn);
			
			DefaultMethod invoke = new DefaultMethod(delegateType, "Invoke");
			invoke.Accessibility = Accessibility.Public;
			invoke.IsSynthetic = true;
			invoke.Parameters.AddRange(parameters);
			invoke.ReturnType = returnType;
			invoke.Region = region;
			delegateType.Methods.Add(invoke);
			
			DefaultMethod beginInvoke = new DefaultMethod(delegateType, "BeginInvoke");
			beginInvoke.Accessibility = Accessibility.Public;
			beginInvoke.IsSynthetic = true;
			beginInvoke.Parameters.AddRange(invoke.Parameters);
			beginInvoke.Parameters.Add(delegateAsyncCallbackParameter);
			beginInvoke.Parameters.Add(delegateObjectParameter);
			beginInvoke.ReturnType = delegateResultParameter.Type;
			beginInvoke.Region = region;
			delegateType.Methods.Add(beginInvoke);
			
			DefaultMethod endInvoke = new DefaultMethod(delegateType, "EndInvoke");
			endInvoke.Accessibility = Accessibility.Public;
			endInvoke.IsSynthetic = true;
			endInvoke.Parameters.Add(delegateResultParameter);
			endInvoke.ReturnType = invoke.ReturnType;
			endInvoke.Region = region;
			delegateType.Methods.Add(endInvoke);
			
			DefaultMethod ctor = new DefaultMethod(delegateType, ".ctor");
			ctor.EntityType = EntityType.Constructor;
			ctor.Accessibility = Accessibility.Public;
			ctor.IsSynthetic = true;
			ctor.Parameters.Add(delegateObjectParameter);
			ctor.Parameters.Add(delegateIntPtrMethodParameter);
			ctor.ReturnType = delegateType;
			ctor.Region = region;
			delegateType.Methods.Add(ctor);
		}
		#endregion
		
		#region Modifiers
		static void ApplyModifiers(DefaultTypeDefinition td, Modifiers modifiers)
		{
			td.Accessibility = GetAccessibility(modifiers) ?? (td.DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
			td.IsAbstract = (modifiers & (Modifiers.Abstract | Modifiers.Static)) != 0;
			td.IsSealed = (modifiers & (Modifiers.Sealed | Modifiers.Static)) != 0;
			td.IsShadowing = (modifiers & Modifiers.New) != 0;
		}
		
		static Accessibility? GetAccessibility(Modifiers modifiers)
		{
			switch (modifiers & Modifiers.VisibilityMask) {
				case Modifiers.Private:
					return Accessibility.Private;
				case Modifiers.Internal:
					return Accessibility.Internal;
				case Modifiers.Protected | Modifiers.Internal:
					return Accessibility.ProtectedOrInternal;
				case Modifiers.Protected:
					return Accessibility.Protected;
				case Modifiers.Public:
					return Accessibility.Public;
				default:
					return null;
			}
		}
		#endregion
		
		#region Attributes
		void ConvertAttributes(IList<IAttribute> outputList, IEnumerable<AttributeSection> attributes)
		{
			foreach (AttributeSection section in attributes) {
				throw new NotImplementedException();
			}
		}
		#endregion
	}
}
