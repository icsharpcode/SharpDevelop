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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class ClassHelper
//	{
//		public IClass Class;
//		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
//		public CompilationUnitHelper CompilationUnitHelper = new CompilationUnitHelper();
//		
//		List<IMethod> methods = new List<IMethod>();
//		List<IProperty> properties = new List<IProperty>();
//		List<IField> fields = new List<IField>();
//		
//		public void CreateClass(string name)
//		{
//			Class = ProjectContentHelper.AddClassToProjectContent(name);
//			InitializeClassCommon();
//		}
//		
//		void InitializeClassCommon()
//		{
//			Class.Stub(c => c.Methods).Return(methods);
//			Class.Stub(c => c.Properties).Return(properties);
//			Class.Stub(c => c.Fields).Return(fields);
//			Class.Stub(c => c.CompilationUnit).Return(CompilationUnitHelper.CompilationUnit);
//		}
//		
//		public void AddAttributeToClass(string attributeTypeName)
//		{
//			var attributeHelper = new AttributeHelper();
//			attributeHelper.CreateAttribute(attributeTypeName);
//			attributeHelper.AddAttributeToClass(Class);
//		}
//		
//		public void CreatePublicClass(string name)
//		{
//			Class = ProjectContentHelper.AddPublicClassToProjectContent(name);
//			InitializeClassCommon();
//		}
//		
//		public void CreatePrivateClass(string name)
//		{
//			Class = ProjectContentHelper.AddPrivateClassToProjectContent(name);
//			InitializeClassCommon();
//		}
//		
//		public void AddInterfaceToClassBaseTypes(string interfaceFullName, string dotNetName)
//		{
//			IClass interfaceClass = ProjectContentHelper.AddInterfaceToProjectContent(interfaceFullName);
//			AddClassToClassBaseTypes(interfaceClass, interfaceFullName, dotNetName);
//		}
//		
//		public void AddClassToClassBaseTypes(IClass baseTypeClass, string baseTypeFullName, string baseTypeDotNetName)
//		{
//			IReturnType baseType = CreateBaseType(baseTypeClass, baseTypeFullName, baseTypeDotNetName);
//			var baseTypes = new List<IReturnType>();
//			baseTypes.Add(baseType);
//			
//			Class.Stub(c => c.BaseTypes).Return(baseTypes);
//		}
//		
//		ReturnTypeHelper CreateBaseTypeHelper(IClass baseTypeClass, string baseTypeFullName, string baseTypeDotNetName)
//		{
//			var returnTypeHelper = new ReturnTypeHelper();
//			returnTypeHelper.CreateReturnType(baseTypeFullName);
//			returnTypeHelper.AddUnderlyingClass(baseTypeClass);
//			returnTypeHelper.AddDotNetName(baseTypeDotNetName);
//			return returnTypeHelper;
//		}
//		
//		IReturnType CreateBaseType(IClass baseTypeClass, string baseTypeFullName, string baseTypeDotNetName)
//		{
//			return CreateBaseTypeHelper(baseTypeClass, baseTypeFullName, baseTypeDotNetName).ReturnType;
//		}
//		
//		public void AddClassToClassBaseTypes(string fullName)
//		{
//			IClass baseTypeClass = ProjectContentHelper.AddClassToProjectContent(fullName);
//			AddClassToClassBaseTypes(baseTypeClass, fullName, fullName);
//		}
//		
//		public void AddBaseTypeToClass(string fullName)
//		{
//			IClass baseTypeClass = ProjectContentHelper.AddClassToProjectContent(fullName);
//			IReturnType baseType = CreateBaseType(baseTypeClass, fullName, fullName);
//			
//			Class.Stub(c => c.BaseType).Return(baseType);
//		}
//		
//		/// <summary>
//		/// Name should include the class prefix (e.g. "Class1.MyMethod");
//		/// </summary>
//		public void AddMethodToClass(string fullyQualifiedName)
//		{
//			AddMethodToClass(fullyQualifiedName, DomRegion.Empty, DomRegion.Empty);
//		}
//		
//		public void AddMethodToClass(string fullyQualifiedName, DomRegion region, DomRegion bodyRegion)
//		{
//			var helper = new MethodHelper();
//			helper.ProjectContentHelper = ProjectContentHelper;
//			helper.CreateMethod(fullyQualifiedName);
//			helper.SetRegion(region);
//			helper.SetBodyRegion(bodyRegion);
//			helper.SetDeclaringType(Class);
//			methods.Add(helper.Method);
//		}
//		
//		/// <summary>
//		/// Name should include the class prefix (e.g. "Class1.MyProperty");
//		/// </summary>
//		public void AddPropertyToClass(string fullyQualifiedName)
//		{
//			var helper = new PropertyHelper();
//			helper.ProjectContentHelper = ProjectContentHelper;
//			helper.CreateProperty(fullyQualifiedName);
//			
//			properties.Add(helper.Property);
//		}
//		
//		/// <summary>
//		/// Name should include the class prefix (e.g. "Class1.MyField");
//		/// </summary>
//		public void AddFieldToClass(string fullyQualifiedName)
//		{
//			AddFieldToClass(fullyQualifiedName, DomRegion.Empty);
//		}
//		
//		public void AddFieldToClass(string fullyQualifiedName, DomRegion region)
//		{
//			var helper = new FieldHelper();
//			helper.ProjectContentHelper = ProjectContentHelper;
//			helper.CreateField(fullyQualifiedName);
//			helper.SetRegion(region);
//			
//			fields.Add(helper.Field);
//		}
//		
//		public void AddClassNamespace(string name)
//		{
//			Class.Stub(c => c.Namespace).Return(name);
//		}
//		
//		public void SetClassRegion(DomRegion classRegion)
//		{
//			Class.Stub(c => c.Region).Return(classRegion);
//		}
//		
//		public void SetClassFileName(string fileName)
//		{
//			CompilationUnitHelper.SetFileName(fileName);
//		}
//		
//		public void SetClassType(ClassType type)
//		{
//			Class.Stub(c => c.ClassType).Return(type);
//		}
//		
//		public void SetProjectForProjectContent(object project)
//		{
//			ProjectContentHelper.SetProjectForProjectContent(project);
//		}
//		
//		public void MakeClassAbstract()
//		{
//			Class.Stub(c => c.IsAbstract).Return(true);
//		}
//		
//		public void MakeClassPartial()
//		{
//			Class.Stub(c => c.IsPartial).Return(true);
//		}
//		
//		public void AddNamespaceUsingScopeToClass(string namespaceName)
//		{
//			var usingScopeHelper = new UsingScopeHelper();
//			usingScopeHelper.SetNamespaceName(namespaceName);
//			
//			Class.Stub(c => c.UsingScope).Return(usingScopeHelper.UsingScope);
//		}
//		
//		public void SetDotNetName(string className)
//		{
//			Class.Stub(c => c.DotNetName).Return(className);
//		}
//		
//		/// <summary>
//		/// Classes at the end of the array are at the top of the inheritance tree.
//		/// </summary>
//		public void AddClassInheritanceTreeClassesOnly(params string[] classNames)
//		{
//			List<IClass> classes = CreateClassInheritanceTree(classNames);
//			Class.Stub(c => c.ClassInheritanceTreeClassesOnly).Return(classes);
//		}
//		
//		List<IClass> CreateClassInheritanceTree(string[] classNames)
//		{
//			return classNames
//				.Select(name => CreateClassHelperWithPublicClass(name).Class)
//				.ToList();
//		}
//		
//		ClassHelper CreateClassHelperWithPublicClass(string name)
//		{
//			var classHelper = new ClassHelper();
//			classHelper.CreatePublicClass(name);
//			return classHelper;
//		}
//		
//		public void SetRegionBeginLine(int line)
//		{
//			SetRegion(new DomRegion(line, 1));
//		}
//		
//		public void SetRegion(DomRegion region)
//		{
//			Class.Stub(c => c.Region).Return(region);
//		}
//	}
//}
