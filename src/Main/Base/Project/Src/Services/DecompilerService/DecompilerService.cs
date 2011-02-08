// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Navigation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop
{
	public static class DecompilerService
	{
		public static string GetParameters(MethodDefinition method)
		{
			StringBuilder sb = new StringBuilder();

			if (!method.HasParameters)
				sb.Append("()");
			else {
				sb.Append("(");
				for (int i = 0 ; i < method.Parameters.Count; ++i) {
					var p = method.Parameters[i];

					if (p.IsOut)
						sb.Append("out ");
					else
						if (p.ParameterType.IsByReference)
							sb.Append("ref ");

					sb.Append(p.ParameterType.Name.Replace("&", string.Empty));
					sb.Append(" ");

					sb.Append(p.Name);

					if (i < method.Parameters.Count - 1)
						sb.Append(", ");
				}
				sb.Append(")");
			}

			return sb.ToString();
		}
		
		public static void ReadMetadata(IClass c, out string filePath)
		{
			if (c == null) {
				filePath = null;
				return;
			}
			
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			
			// add namespace
			CodeNamespace generatedNamespace = new CodeNamespace(c.Namespace);
			generatedNamespace.Imports.Add(new CodeNamespaceImport("System"));
			compileUnit.Namespaces.Add(generatedNamespace);
			
			// add type
			var targetClass = new CodeTypeDeclaration(c.Name);
			
			// write attributes
			AddAttributes(c, targetClass);
			
			// write class definition
			if (c.IsPublic) targetClass.TypeAttributes |= System.Reflection.TypeAttributes.Public;
			if (c.IsSealed) targetClass.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;
			// static class limitation - c.IsStatic: https://connect.microsoft.com/VisualStudio/feedback/details/93653/codedom-unable-to-generate-static-events-and-classes
			targetClass.IsPartial = c.IsPartial;

			switch (c.ClassType) {
				case ClassType.Class:
					targetClass.IsClass = true;
					break;
				case ClassType.Enum:
					targetClass.IsEnum = true;
					break;
				case ClassType.Interface:
					targetClass.IsInterface = true;
					break;
				case ClassType.Struct:
					targetClass.IsStruct = true;
					break;
			}
			
			// generics
			foreach (var typeParameter in c.TypeParameters) {
				var tp = new CodeTypeParameter(typeParameter.Name);
				foreach (var con in typeParameter.Constraints) {
					tp.Constraints.Add(con.Name);
				}
				targetClass.TypeParameters.Add(tp);
			}

			// base types
			foreach (var basetype in c.BaseTypes) {
				if (basetype.FullyQualifiedName.Equals("System.Object", StringComparison.OrdinalIgnoreCase)) 
					continue;
				var baseType = AddGenericBaseTypes(basetype);
				targetClass.BaseTypes.Add(baseType);
			}
			
			// field members
			foreach (var f in c.Fields) {
				if (!f.IsPublic && !f.IsProtected) continue;
				
				CodeMemberField field = new CodeMemberField() { Name = f.Name,
					Attributes = MemberAttributes.Public };
				AddDefinition(f, field);
				AddAttributes(f, field);
				AddDocumentation(f, field);
				
				field.Type = new CodeTypeReference(f.ReturnType.FullyQualifiedName);
				targetClass.Members.Add(field);
			}
			
			// event members
			foreach (var e in c.Events) {
				if (!e.IsPublic && !e.IsProtected) continue;
				CodeMemberEvent ev = new CodeMemberEvent() { Name = e.Name,
					Attributes = MemberAttributes.Public,
					Type = new CodeTypeReference(e.ReturnType.FullyQualifiedName) };
				AddDefinition(e, ev);
				AddDocumentation(e, ev);
				
				targetClass.Members.Add(ev);
			}
			
			// properties
			foreach (var property in c.Properties) {
				if (!property.IsPublic && !property.IsProtected) continue;
				CodeMemberProperty p = new CodeMemberProperty() {
					Name = property.Name,
					Attributes = MemberAttributes.Public,
					Type = new CodeTypeReference(property.ReturnType.FullyQualifiedName),
					HasGet = property.CanGet,
					HasSet = property.CanSet
				};
				
				AddAttributes(property, p);
				AddDefinition(property, p);
				AddDocumentation(property, p);
				
				if (property.IsIndexer) {
					p.Parameters.Add(new CodeParameterDeclarationExpression("System.Int32", "index"));
				}
				targetClass.Members.Add(p);
			}
			
			// methods and constructors
			foreach (var method in c.Methods) {
				if (!method.IsPublic && !method.IsProtected) continue;
				
				if (method.IsConstructor) {
					CodeConstructor constructor = new CodeConstructor() { Name = c.Name,
						Attributes = MemberAttributes.Public };
					AddAttributes(method, constructor);
					AddDefinition(method, constructor);
					AddParameters(method, constructor);
					AddDocumentation(method, constructor);

					targetClass.Members.Add(constructor);
				} else {
					CodeMemberMethod m = new CodeMemberMethod() {
						Name = method.Name,
						Attributes = MemberAttributes.Public,
						ReturnType = new CodeTypeReference(method.ReturnType.FullyQualifiedName),
					};
					
					AddAttributes(method, m);
					AddDefinition(method, m);
					AddParameters(method, m);
					AddDocumentation(method, m);
					
					targetClass.Members.Add(m);
				}
			}
			
			// delegates
			foreach (var inner in c.InnerClasses) {
				if (inner.ClassType == ClassType.Delegate) {
					IMethod invoker = inner.Methods.Where(method => method.Name == "Invoke").FirstOrDefault();
					CodeTypeDelegate del = new CodeTypeDelegate(inner.Name) {
						Attributes = MemberAttributes.Public,
						ReturnType = new CodeTypeReference(invoker.ReturnType.FullyQualifiedName)
					};
					
					AddDocumentation(invoker, del);
					
					foreach (var p in invoker.Parameters)
						del.Parameters.Add(new CodeParameterDeclarationExpression(p.ReturnType.FullyQualifiedName, p.Name));
					
					targetClass.Members.Add(del);
				}
			}
			
			// add class
			generatedNamespace.Types.Add(targetClass);
			filePath = WriteTempFile(c.Name, compileUnit);
		}
		
		static CodeTypeReference AddGenericBaseTypes(IReturnType basetype)
		{
			string type = basetype.FullyQualifiedName;
			CodeTypeReference baseType = new CodeTypeReference(type);
			if (basetype.IsConstructedReturnType)
			{
				var constructed = basetype as ConstructedReturnType;
				int i = 0;
				foreach (var typearg in constructed.TypeArguments) {
					baseType.TypeArguments.Add(new CodeTypeReference(typearg.Name, CodeTypeReferenceOptions.GenericTypeParameter));
					
					if (typearg is ConstructedReturnType) {
						CodeTypeReference baseType1 = new CodeTypeReference(typearg.FullyQualifiedName);
						baseType1.TypeArguments.Add(AddGenericBaseTypes(typearg));
						baseType.TypeArguments[i].TypeArguments.Add(baseType1);
					}
					++i;
				}
			}
			
			return baseType;
		}
		
		static void AddParameters(IMethod method, CodeMemberMethod m)
		{
			foreach (var p in method.Parameters) {
				string returnType = p.ReturnType.FullyQualifiedName;
				
				var par = new CodeParameterDeclarationExpression(returnType, p.Name);
				
				if (p.ReturnType.IsConstructedReturnType)
				{
					CodeTypeReference baseType = new CodeTypeReference(returnType);
					var c = p.ReturnType as ConstructedReturnType;
					foreach (var typearg in c.TypeArguments) {
						baseType.TypeArguments.Add(new CodeTypeReference(typearg.Name, CodeTypeReferenceOptions.GenericTypeParameter));
					}
					
					par.Type = baseType;
				}
				
				if (p.IsRef)
					par.Direction = FieldDirection.Ref;
				if (p.IsOut)
					par.Direction = FieldDirection.Out;
				if (p.IsParams)
					par.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.ParamArrayAttribute))));
				// TODO: if (p.IsOptional)
				
				m.Parameters.Add(par);
			}
		}
		
		static void AddDocumentation(IEntity entity, CodeTypeMember member)
		{
			if (string.IsNullOrEmpty(entity.Documentation) || string.IsNullOrEmpty(entity.Documentation.Trim()))
				return;
			
			member.Comments.Add(new CodeCommentStatement(entity.Documentation.Replace("     ", string.Empty), true));
		}
		
		static void AddDefinition(IEntity entity, CodeTypeMember member)
		{
			if (entity.IsProtected)
				member.Attributes = MemberAttributes.Family;
			
			if (entity.IsStatic)
				member.Attributes |= MemberAttributes.Static;
			if (entity.IsNew)
				member.Attributes |= MemberAttributes.New;
			if (entity.IsOverride)
				member.Attributes |= MemberAttributes.Override;
			if (entity.IsAbstract)
				member.Attributes |= MemberAttributes.Abstract;
			if (entity.IsVirtual)
				member.Attributes |= MemberAttributes.Final;
			if (entity.IsConst)
				member.Attributes |= MemberAttributes.Const;
		}
		
		static void AddAttributes(IEntity entity, CodeTypeMember member)
		{
			// write attributes
			foreach (var attr in entity.Attributes) {
				
				List<CodeAttributeArgument> list = new List<CodeAttributeArgument>();
				for (int i = 0; i < attr.PositionalArguments.Count; i++) {
					if (!(attr.PositionalArguments[i] is IReturnType))
						list.Add(new CodeAttributeArgument(new CodePrimitiveExpression(attr.PositionalArguments[i])));
				}
				
				if (list.Count == 0) {
					member.CustomAttributes.Add(
						new CodeAttributeDeclaration(attr.AttributeType.FullyQualifiedName));
				} else {
					member.CustomAttributes.Add(
						new CodeAttributeDeclaration(attr.AttributeType.FullyQualifiedName, list.ToArray()));
				}
			}
		}
		
		static string WriteTempFile(string fileName, CodeCompileUnit compileUnit)
		{
			// temp file
			string tempFolder = Path.GetTempPath();
			string file = fileName + ".temp." + 
				ProjectService.CurrentProject.LanguageProperties.CodeDomProvider.FileExtension;
						
			string filePath = Path.Combine(tempFolder, file);

			if (File.Exists(filePath))
				File.SetAttributes(filePath, FileAttributes.Temporary);	
			
			// write file
			using (var sw = new StreamWriter(filePath, false))
			{
				ProjectService
					.CurrentProject
					.LanguageProperties
					.CodeDomProvider.GenerateCodeFromCompileUnit(
						compileUnit,
						sw,
						new CodeGeneratorOptions());
			}
			
			File.SetAttributes(filePath, FileAttributes.ReadOnly);			
			return filePath;
		}
	}
}