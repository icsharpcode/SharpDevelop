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
			var typeDef = type.GetDefinition();
			if (typeDef != null) {
				var typeModel = typeDef.GetModel();
				if (typeModel != null)
					return Create(context.WithFilteredFileName(null), typeModel);
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
		
		/// <summary>
		/// Note that projectContent may be different to the IClass.ProjectContent since the class
		/// is retrieved from the namespace contents and could belong to a separate project or
		/// referenced assembly.
		/// </summary>
		public CodeType(CodeModelContext context, ITypeDefinitionModel typeModel)
			: base(context, typeModel)
		{
			this.typeModel = typeModel;
		}
		
		public CodeType()
		{
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return typeModel.Accessibility.ToAccess(); }
			set {
				var td = typeModel.Resolve();
				if (td != null) {
					context.CodeGenerator.ChangeAccessibility(td, value.ToAccessibility());
				}
			}
		}
		
		public virtual string FullName {
			get {
				var fullTypeName = typeModel.FullTypeName;
				StringBuilder b = new StringBuilder();
				if (!string.IsNullOrEmpty(fullTypeName.TopLevelTypeName.Namespace)) {
					b.Append(fullTypeName.TopLevelTypeName.Namespace);
					b.Append('.');
				}
				b.Append(fullTypeName.TopLevelTypeName.Name);
				for (int i = 0; i < fullTypeName.NestingLevel; i++) {
					b.Append('.');
					b.Append(fullTypeName.GetNestedTypeName(i));
				}
				return b.ToString();
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
				var td = typeModel.Resolve();
				if (td != null) {
					IEnumerable<IType> baseTypes;
					if (td.Kind == TypeKind.Interface)
						baseTypes = td.DirectBaseTypes;
					else
						baseTypes = td.DirectBaseTypes.Where(t => t.Kind != TypeKind.Interface);
					foreach (var baseType in baseTypes) {
						CodeType element = Create(context, baseType);
						if (element != null)
							list.Add(element);
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
				if (context.FilteredFileName != null)
					return new FileCodeModel2(context).GetNamespace(typeModel.Namespace);
				else
					throw new NotImplementedException();
				//    return new CodeNamespace(context, typeModel.Namespace);
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
			var td = typeModel.Resolve();
			return td != null && td.GetAllBaseTypeDefinitions().Any(b => b.FullName == fullName);
		}
	}
}
