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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;
using IronPython.Modules;
using IronPython.Runtime;
using Microsoft.Scripting.Runtime;

namespace ICSharpCode.PythonBinding
{
	public class PythonModuleCompletionItems : List<ICompletionEntry>
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass moduleClass;
		DefaultProjectContent projectContent;
		
		static readonly BindingFlags PublicAndStaticBindingFlags = BindingFlags.Public | BindingFlags.Static;
		
		public PythonModuleCompletionItems(PythonStandardModuleType moduleType)
		{
			projectContent = new DefaultProjectContent();
			compilationUnit = new DefaultCompilationUnit(projectContent);
			moduleClass = new DefaultClass(compilationUnit, moduleType.Name);
			
			AddCompletionItemsForType(moduleType.Type);
			AddStandardCompletionItems();
		}
		
		void AddCompletionItemsForType(Type type)
		{
			foreach (MemberInfo member in type.GetMembers(PublicAndStaticBindingFlags)) {
				if (!HasPythonHiddenAttribute(member)) {
					ICompletionEntry item = CreateCompletionItem(member, moduleClass);
					if (item != null) {
						Add(item);
					}
				}
			}
		}
		
		void AddStandardCompletionItems()
		{
			AddField("__name__");
			AddField("__package__");
		}

		protected void AddField(string name)
		{
			DefaultField field = new DefaultField(moduleClass, name);
			Add(field);
		}
		
		bool HasPythonHiddenAttribute(MemberInfo memberInfo)
		{
			foreach (Attribute attribute in memberInfo.GetCustomAttributes(false)) {
				Type type = attribute.GetType();
				if (type.Name == "PythonHiddenAttribute") {
					return true;
				}
			}
			return false;
		}
		
		ICompletionEntry CreateCompletionItem(MemberInfo memberInfo, IClass c)
		{
			if (memberInfo is MethodInfo) {
				return CreateMethodFromMethodInfo((MethodInfo)memberInfo, c);
			} else if (memberInfo is FieldInfo) {
				return CreateFieldFromFieldInfo((FieldInfo)memberInfo, c);
			} else if (memberInfo is Type) {
				return CreateClassFromType((Type)memberInfo);
			}
			return null;
		}
		
		IMethod CreateMethodFromMethodInfo(MethodInfo methodInfo, IClass c)
		{
			DefaultMethod method = new DefaultMethod(c, methodInfo.Name);
			method.Documentation = GetDocumentation(methodInfo);
			method.ReturnType = CreateMethodReturnType(methodInfo);
			method.Modifiers = ModifierEnum.Public;
			
			foreach (ParameterInfo paramInfo in methodInfo.GetParameters()) {
				if (!IsCodeContextParameter(paramInfo)) {
					IParameter parameter = ConvertParameter(paramInfo, method);
					method.Parameters.Add(parameter);
				}
			}
			
			c.Methods.Add(method);
			
			return method;
		}
		
		string GetDocumentation(MemberInfo memberInfo)
		{
			foreach (DocumentationAttribute documentation in GetDocumentationAttributes(memberInfo)) {
				return documentation.Documentation;
			}
			return null;
		}
		
		object[] GetDocumentationAttributes(MemberInfo memberInfo)
		{
			return memberInfo.GetCustomAttributes(typeof(DocumentationAttribute), false);
		}
		
		IReturnType CreateMethodReturnType(MethodInfo methodInfo)
		{
			DefaultClass declaringType = new DefaultClass(compilationUnit, methodInfo.ReturnType.FullName);
			return new DefaultReturnType(declaringType);
		}
		
		bool IsCodeContextParameter(ParameterInfo paramInfo)
		{
			return paramInfo.ParameterType == typeof(CodeContext);
		}
		
		IParameter ConvertParameter(ParameterInfo paramInfo, IMethod method)
		{
			DefaultClass c = new DefaultClass(compilationUnit, paramInfo.ParameterType.FullName);
			DefaultReturnType returnType = new DefaultReturnType(c);
			return new DefaultParameter(paramInfo.Name, returnType, DomRegion.Empty);
		}
		
		IField CreateFieldFromFieldInfo(FieldInfo fieldInfo, IClass c)
		{
			return new DefaultField(c, fieldInfo.Name);
		}
		
		IClass CreateClassFromType(Type type)
		{
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			return new DefaultClass(unit, type.Name);
		}
		
		public MethodGroup GetMethods(string name)
		{
			List<IMethod> methods = new List<IMethod>();
			foreach (object member in this) {
				IMethod method = member as IMethod;
				if (method != null) {
					if (method.Name == name) {
						methods.Add(method);
					}
				}
			}
			return new MethodGroup(methods);
		}
	}
}
