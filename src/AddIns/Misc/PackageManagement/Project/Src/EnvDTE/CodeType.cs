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

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement, global::EnvDTE.CodeType
	{
		protected ITypeDefinition typeDefinition;
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
		
		public CodeType()
		{
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return typeDefinition.Accessibility.ToAccess(); }
			set { }
		}
		
		public virtual string FullName {
			get {
				FullTypeName fullTypeName = typeDefinition.FullTypeName;
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
						.Where(member => !member.Region.End.IsEmpty || !typeDefinition.ParentAssembly.IsMainAssembly)
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
			get { return new FileCodeModelCodeNamespace(context, typeDefinition.Namespace); }
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
		
		protected IType FindType(string type)
		{
			var fieldTypeName = new FullTypeName(type);
			return typeDefinition.Compilation.FindType(fieldTypeName);
		}
		
		protected void ReloadTypeDefinition()
		{
			ICompilation compilation = context.DteProject.GetCompilationUnit(typeDefinition.BodyRegion.FileName);
			
			ITypeDefinition matchedTypeDefinition = compilation.MainAssembly.GetTypeDefinition(typeDefinition.FullTypeName);
			if (matchedTypeDefinition != null) {
				typeDefinition = matchedTypeDefinition;
			}
		}
	}
}
