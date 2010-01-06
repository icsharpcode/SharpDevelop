// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Modules;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	public class PythonModuleCompletionItems : ReadOnlyCollectionBase
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass moduleClass;
		DefaultProjectContent projectContent;

		static readonly BindingFlags PublicAndStaticBindingFlags = BindingFlags.Public | BindingFlags.Static;

		public PythonModuleCompletionItems(Type type)
		{
			projectContent = new DefaultProjectContent();
			compilationUnit = new DefaultCompilationUnit(projectContent);
			moduleClass = new DefaultClass(compilationUnit, String.Empty);
			
			AddCompletionItemsForType(type);
			AddStandardCompletionItems();
		}
		
		void AddCompletionItemsForType(Type type)
		{
			foreach (MemberInfo member in type.GetMembers(PublicAndStaticBindingFlags)) {
				if (!HasPythonHiddenAttribute(member)) {
					object item = CreateCompletionItem(member, moduleClass);
					if (item != null) {
						InnerList.Add(item);
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
			InnerList.Add(field);
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
		
		object CreateCompletionItem(MemberInfo memberInfo, IClass c)
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
			return new DefaultMethod(c, methodInfo.Name);
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
	}
}
