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
		
		CodeElementsList<CodeElement> members;
		
		public static CodeType Create(CodeModelContext context, IType type)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef != null) {
				ITypeDefinitionModel typeModel = typeDef.GetModel();
				if (typeModel != null) {
					return Create(context.WithFilteredFileName(null), typeModel);
				}
			}
			return null;
		}
		
		public static CodeType Create(CodeModelContext context, ITypeDefinitionModel typeModel)
		{
			switch (typeModel.TypeKind) {
				case TypeKind.Class:
				case TypeKind.Module:
					goto default;
				case TypeKind.Interface:
					goto default;
				case TypeKind.Struct:
				case TypeKind.Void:
					goto default;
				case TypeKind.Delegate:
					goto default;
				case TypeKind.Enum:
					goto default;
				default:
					return new CodeType(context, typeModel);
			}
		}
		
		public CodeType(CodeModelContext context, ITypeDefinitionModel typeModel)
			: base(context, typeModel)
		{
			this.typeModel = typeModel;
			this.InfoLocation = GetInfoLocation();
		}
		
		global::EnvDTE.vsCMInfoLocation GetInfoLocation()
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
			get { return typeModel.Accessibility.ToAccess(); }
			set {
				ITypeDefinition typeDefinition = typeModel.Resolve();
				if (typeDefinition != null) {
					context.CodeGenerator.ChangeAccessibility(typeDefinition, value.ToAccessibility());
				}
			}
		}
		
		public virtual string FullName {
			get {
				FullTypeName fullTypeName = typeModel.FullTypeName;
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
				return fullName.ToString();
			}
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				if (members == null) {
					members = typeModel.Members
						.Where(m => IsInFilter(m.Region))
						.Select(m => CreateMember(context, m))
						.AsCodeElements();
				}
				return members;
			}
		}
		
		public virtual global::EnvDTE.CodeElements Bases {
			get {
				var list = new CodeElementsList<CodeType>();
				ITypeDefinition typeDefinition = typeModel.Resolve();
				if (typeDefinition != null) {
					IEnumerable<IType> baseTypes;
					if (typeDefinition.Kind == TypeKind.Interface) {
						baseTypes = typeDefinition.DirectBaseTypes;
					} else {
						baseTypes = typeDefinition.DirectBaseTypes.Where(type => type.Kind != TypeKind.Interface);
					}
					foreach (IType baseType in baseTypes) {
						CodeType element = Create(context, baseType);
						if (element != null) {
							list.Add(element);
						}
					}
				}
				return list;
			}
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get {
				return GetAttributes(typeModel);
			}
		}
		
		public virtual global::EnvDTE.CodeNamespace Namespace {
			get {
				if (context.FilteredFileName != null) {
					return new FileCodeModel2(context, null).GetNamespace(typeModel.Namespace);
				} else {
					throw new NotImplementedException();
				//    return new CodeNamespace(context, typeModel.Namespace);
				}
			}
		}
		
		public virtual global::EnvDTE.ProjectItem ProjectItem {
			get {
				if (context.CurrentProject != null) {
					return EnvDTE.ProjectItem.FindByEntity(context.CurrentProject, typeModel);
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
			ITypeDefinition typeDefinition = typeModel.Resolve();
			return typeDefinition != null && typeDefinition.GetAllBaseTypeDefinitions().Any(baseType => baseType.FullName == fullName);
		}
	}
}
