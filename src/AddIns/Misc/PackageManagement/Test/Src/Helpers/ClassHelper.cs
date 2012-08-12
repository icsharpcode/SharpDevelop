// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class ClassHelper
	{
		public IClass Class;
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		public CompilationUnitHelper CompilationUnitHelper = new CompilationUnitHelper();
		
		List<IMethod> methods = new List<IMethod>();
		List<IProperty> properties = new List<IProperty>();
		List<IField> fields = new List<IField>();
		
		public void CreateClass(string name)
		{
			Class = ProjectContentHelper.AddClassToProjectContent(name);
			InitializeClassCommon();
		}
		
		void InitializeClassCommon()
		{
			Class.Stub(c => c.Methods).Return(methods);
			Class.Stub(c => c.Properties).Return(properties);
			Class.Stub(c => c.Fields).Return(fields);
			Class.Stub(c => c.CompilationUnit).Return(CompilationUnitHelper.CompilationUnit);
		}
		
		public void AddAttributeToClass(string name)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(name);
			attributeHelper.AddAttributeToClass(Class);
		}
		
		public void CreatePublicClass(string name)
		{
			Class = ProjectContentHelper.AddPublicClassToProjectContent(name);
			InitializeClassCommon();
		}
		
		public void CreatePrivateClass(string name)
		{
			Class = ProjectContentHelper.AddPrivateClassToProjectContent(name);
			InitializeClassCommon();
		}
		
		public void AddInterfaceToClassBaseTypes(string interfaceFullName, string dotNetName)
		{
			IClass interfaceClass = ProjectContentHelper.AddInterfaceToProjectContent(interfaceFullName);
			AddClassToClassBaseTypes(interfaceClass, interfaceFullName, dotNetName);
		}
		
		public void AddClassToClassBaseTypes(IClass baseTypeClass, string baseTypeFullName, string baseTypeDotNetName)
		{
			IReturnType baseType = CreateBaseType(baseTypeClass, baseTypeFullName, baseTypeDotNetName);
			var baseTypes = new List<IReturnType>();
			baseTypes.Add(baseType);
			
			Class.Stub(c => c.BaseTypes).Return(baseTypes);
		}
		
		IReturnType CreateBaseType(IClass baseTypeClass, string baseTypeFullName, string baseTypeDotNetName)
		{
			var returnTypeHelper = new ReturnTypeHelper();
			returnTypeHelper.CreateReturnType(baseTypeFullName);
			returnTypeHelper.AddUnderlyingClass(baseTypeClass);
			returnTypeHelper.AddDotNetName(baseTypeDotNetName);
			return returnTypeHelper.ReturnType;
		}
		
		public void AddClassToClassBaseTypes(string fullName)
		{
			IClass baseTypeClass = ProjectContentHelper.AddClassToProjectContent(fullName);
			AddClassToClassBaseTypes(baseTypeClass, fullName, fullName);
		}
		
		public void AddBaseTypeToClass(string fullName)
		{
			IClass baseTypeClass = ProjectContentHelper.AddClassToProjectContent(fullName);
			IReturnType baseType = CreateBaseType(baseTypeClass, fullName, fullName);
			
			Class.Stub(c => c.BaseType).Return(baseType);
		}
		
		/// <summary>
		/// Name should include the class prefix (e.g. "Class1.MyMethod");
		/// </summary>
		public void AddMethodToClass(string fullyQualifiedName)
		{
			AddMethodToClass(fullyQualifiedName, DomRegion.Empty, DomRegion.Empty);
		}
		
		public void AddMethodToClass(string fullyQualifiedName, DomRegion region, DomRegion bodyRegion)
		{
			var helper = new MethodHelper();
			helper.ProjectContentHelper = ProjectContentHelper;
			helper.CreateMethod(fullyQualifiedName);
			helper.SetRegion(region);
			helper.SetBodyRegion(bodyRegion);
			helper.SetDeclaringType(Class);
			methods.Add(helper.Method);
		}
		
		/// <summary>
		/// Name should include the class prefix (e.g. "Class1.MyProperty");
		/// </summary>
		public void AddPropertyToClass(string fullyQualifiedName)
		{
			var helper = new PropertyHelper();
			helper.ProjectContentHelper = ProjectContentHelper;
			helper.CreateProperty(fullyQualifiedName);
			
			properties.Add(helper.Property);
		}
		
		/// <summary>
		/// Name should include the class prefix (e.g. "Class1.MyField");
		/// </summary>
		public void AddFieldToClass(string fullyQualifiedName)
		{
			AddFieldToClass(fullyQualifiedName, DomRegion.Empty);
		}
		
		public void AddFieldToClass(string fullyQualifiedName, DomRegion region)
		{
			var helper = new FieldHelper();
			helper.ProjectContentHelper = ProjectContentHelper;
			helper.CreateField(fullyQualifiedName);
			helper.SetRegion(region);
			
			fields.Add(helper.Field);
		}
		
		public void AddClassNamespace(string name)
		{
			Class.Stub(c => c.Namespace).Return(name);
		}
		
		public void SetClassRegion(DomRegion classRegion)
		{
			Class.Stub(c => c.Region).Return(classRegion);
		}
		
		public void SetClassFileName(string fileName)
		{
			CompilationUnitHelper.SetFileName(fileName);
		}
		
		public void SetClassType(ClassType type)
		{
			Class.Stub(c => c.ClassType).Return(type);
		}
		
		public void SetProjectForProjectContent(object project)
		{
			ProjectContentHelper.SetProjectForProjectContent(project);
		}
	}
}
