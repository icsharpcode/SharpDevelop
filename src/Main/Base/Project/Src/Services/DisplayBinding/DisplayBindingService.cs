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

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class handles the installed display bindings
	/// and provides a simple access point to these bindings.
	/// </summary>
	public static class DisplayBindingService
	{
		readonly static string displayBindingPath = "/SharpDevelop/Workbench/DisplayBindings";
		
		static DisplayBindingDescriptor[] bindings = null;
		
		public static IDisplayBinding GetBindingPerFileName(string filename)
		{
			DisplayBindingDescriptor codon = GetCodonPerFileName(filename);
			return codon == null ? null : codon.Binding;
		}
		
		public static IDisplayBinding GetBindingPerLanguageName(string languagename)
		{
			DisplayBindingDescriptor codon = GetCodonPerLanguageName(languagename);
			return codon == null ? null : codon.Binding;
		}
		
		static DisplayBindingDescriptor GetCodonPerFileName(string filename)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (!binding.IsSecondary && binding.Binding.CanCreateContentForFile(filename)) {
					return binding;
				}
			}
			return null;
		}
		
		static DisplayBindingDescriptor GetCodonPerLanguageName(string languagename)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (binding.Binding != null && binding.Binding.CanCreateContentForLanguage(languagename)) {
					return binding;
				}
			}
			return null;
		}
		
		public static void AttachSubWindows(IWorkbenchWindow workbenchWindow)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (binding.IsSecondary && binding.SecondaryBinding.CanAttachTo(workbenchWindow.ViewContent)) {
					ISecondaryViewContent [] viewContents = binding.SecondaryBinding.CreateSecondaryViewContent(workbenchWindow.ViewContent);
					if (viewContents != null) {
						foreach (ISecondaryViewContent viewContent in viewContents) {
							workbenchWindow.AttachSecondaryViewContent(viewContent);
						}
					} else {
						MessageService.ShowError("Can't attach secondary view content. " + workbenchWindow.ViewContent + " returned null.\n(should never happen)");
					}
				}
			}
		}
		
		static DisplayBindingService()
		{
			bindings = (DisplayBindingDescriptor[])(AddInTree.GetTreeNode(displayBindingPath).BuildChildItems(null)).ToArray(typeof(DisplayBindingDescriptor));
		}
	}
}
