// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class LanguageBindingService
	{
		static LanguageBindingDescriptor[] bindings = null;
		
		public static string GetProjectFileExtension(string languageName)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerLanguageName(languageName);
			return descriptor == null ? null : descriptor.ProjectFileExtension;
		}
		
		public static ILanguageBinding GetBindingPerLanguageName(string languagename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerLanguageName(languagename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static ILanguageBinding GetBindingPerFileName(string filename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerFileName(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static ILanguageBinding GetBindingPerProjectFile(string filename)
		{
			LanguageBindingDescriptor descriptor = GetCodonPerProjectFile(filename);
			return descriptor == null ? null : descriptor.Binding;
		}
		
		public static LanguageBindingDescriptor GetCodonPerLanguageName(string languagename)
		{
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (binding.Binding.Language == languagename) {
					return binding;
				}
			}
			return null;
		}
		
		public static LanguageBindingDescriptor GetCodonPerFileName(string filename)
		{
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (binding.Binding.CanCompile(filename)) {
					return binding;
				}
			}
			return null;
		}
		
		public static LanguageBindingDescriptor GetCodonPerProjectFile(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToUpper();
			foreach (LanguageBindingDescriptor binding in bindings) {
				if (binding.ProjectFileExtension.ToUpper() == ext) {
					return binding;
				}
			}
			return null;
		}
		
		static LanguageBindingService()
		{
			try {
				AddInTreeNode treeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/LanguageBindings");
				bindings = (LanguageBindingDescriptor[])(treeNode.BuildChildItems(null)).ToArray(typeof(LanguageBindingDescriptor));
			} catch (TreePathNotFoundException) {
				bindings = new LanguageBindingDescriptor[] {};
			}
		}
	}
}
