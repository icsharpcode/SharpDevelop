// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement, global::EnvDTE.CodeType
	{
		protected readonly ITypeDefinitionModel typeModel;
		protected readonly ITypeDefinition typeDefinition;
		IType[] typeArguments;
		
		CodeElementsList<CodeElement> members;
		
		internal static CodeType Create(CodeModelContext context, IType type)
		{
			ITypeDefinition typeDefinition = type.GetDefinition();
			if (typeDefinition != null) {
				return Create(context.WithFilteredFileName(null), typeDefinition, type.TypeArguments.ToArray());
			}
			return null;
		}
		
		internal static CodeType Create(
			CodeModelContext context,
			ITypeDefinition typeDefinition,
			params IType[] typeArguments)
		{
			switch (typeDefinition.Kind) {
				case TypeKind.Class:
					return new CodeClass2(context, typeDefinition);
				case TypeKind.Interface:
					return new CodeInterface(context, typeDefinition, typeArguments);
				case TypeKind.Module:
				case TypeKind.Struct:
				case TypeKind.Void:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				default:
					return new CodeType(context, typeDefinition, typeArguments);
			}
		}
		
		public CodeType(CodeModelContext context, ITypeDefinitionModel typeModel)
			: base(context, typeModel)
		{
			this.typeModel = typeModel;
			this.InfoLocation = GetInfoLocationOld();
		}
		
		public CodeType(CodeModelContext context, ITypeDefinition typeDefinition, params IType[] typeArguments)
			: base(context, typeDefinition)
		{
			this.typeDefinition = typeDefinition;
			this.typeArguments = typeArguments;
			this.InfoLocation = GetInfoLocation();
		}
		
		global::EnvDTE.vsCMInfoLocation GetInfoLocation()
		{
			if (typeDefinition.ParentAssembly.IsMainAssembly) {
				return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject;
			}
			return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
		}
		
		global::EnvDTE.vsCMInfoLocation GetInfoLocationOld()
		{
			if (typeModel != null) {
				return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject;
			}
			return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
		}
		
		public CodeType()
		{
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return typeDefinition.Accessibility.ToAccess(); }
			set {
				if (typeDefinition != null) {
					context.CodeGenerator.ChangeAccessibility(typeDefinition, value.ToAccessibility());
				}
			}
		}
		
		public virtual string FullName {
			get {
				FullTypeName fullTypeName = GetFullTypeName();
				var fullName = new StringBuilder();
				if (!string.IsNullOrEmpty(fullTypeName.TopLevelTypeName.Namespace)) {
					fullName.Append(fullTypeName.TopLevelTypeName.Namespace);
					fullName.Append('.');
				}
				fullName.Append(fullTypeName.TopLevelTypeName.Name);
				for (int i = 0; i < fullTypeName.NestingLevel; i++) {
					fullName.Append('.');
					fullName.Append(fullTypeName.GetNestedTypeName(i));
				}
				return fullName.ToString() + GetTypeArguments();
			}
		}
		
		FullTypeName GetFullTypeName()
		{
			if (typeModel != null) {
				return typeModel.FullTypeName;
			}
			return typeDefinition.FullTypeName;
		}
		
		string GetTypeArguments()
		{
			if (typeArguments.Length == 0) {
				return String.Empty;
			}
			
			return String.Format(
				"<{0}>",
				String.Join(", ", typeArguments.Select(type => type.FullName)));
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				if (members == null) {
					members = new CodeElementsList<CodeElement>();
					members.AddRange(typeDefinition.Members
						.Where(member => IsInFilter(member.Region))
						.Where(member => !member.Region.End.IsEmpty)
						.Select(member => CreateMember(context, member)));
				}
				return members;
			}
		}
		
		public virtual global::EnvDTE.CodeElements Bases {
			get {
				var types = new CodeElementsList<CodeType>();
				foreach (IType baseType in GetBaseTypes()) {
					CodeType element = Create(context, baseType);
					if (element != null) {
						types.Add(element);
					}
				}
				return types;
			}
		}
		
		IEnumerable<IType> GetBaseTypes()
		{
			if (typeDefinition.Kind == TypeKind.Interface) {
				return typeDefinition.DirectBaseTypes;
			}
			return typeDefinition.DirectBaseTypes.Where(type => type.Kind != TypeKind.Interface);
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get {
				return GetAttributes(typeDefinition);
			}
		}
		
		public virtual global::EnvDTE.CodeNamespace Namespace {
			get {
				if (context.FilteredFileName != null) {
					return new FileCodeModel2(context, null).GetNamespace(typeModel.Namespace);
				} else {
					throw new NotImplementedException();
				//	return new CodeNamespace(context, typeDefinition.Namespace);
				}
			}
		}
		
		public virtual global::EnvDTE.ProjectItem ProjectItem {
			get {
				if (context.CurrentProject != null) {
					return EnvDTE.ProjectItem.FindByEntity(context.CurrentProject, typeDefinition);
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns true if the current type matches the fully qualified name or any of its
		/// base types are a match.
		/// </summary>
		protected override bool GetIsDerivedFrom(string fullName)
		{
			return typeDefinition
				.GetAllBaseTypeDefinitions()
				.Any(baseType => baseType.FullName == fullName);
		}
	}
}
