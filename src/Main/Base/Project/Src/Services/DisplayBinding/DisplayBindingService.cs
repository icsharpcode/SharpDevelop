// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
				if (!binding.IsSecondary && binding.CanAttachToFile(filename)) {
					if (binding.Binding != null && binding.Binding.CanCreateContentForFile(filename)) {
						return binding;
					}
				}
			}
			return null;
		}
		
		static DisplayBindingDescriptor GetCodonPerLanguageName(string languagename)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (!binding.IsSecondary && binding.CanAttachToLanguage(languagename)) {
					if (binding.Binding != null && binding.Binding.CanCreateContentForLanguage(languagename)) {
						return binding;
					}
				}
			}
			return null;
		}
		
		public static void AttachSubWindows(IViewContent viewContent)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (binding.IsSecondary && binding.CanAttachToFile(viewContent.FileName ?? viewContent.UntitledName)) {
					ISecondaryDisplayBinding displayBinding = binding.SecondaryBinding;
					if (displayBinding != null && displayBinding.CanAttachTo(viewContent)) {
						ISecondaryViewContent[] subViewContents = binding.SecondaryBinding.CreateSecondaryViewContent(viewContent);
						if (subViewContents != null) {
							viewContent.SecondaryViewContents.AddRange(subViewContents);
						} else {
							MessageService.ShowError("Can't attach secondary view content. " + binding.SecondaryBinding + " returned null for " + viewContent + ".\n(should never happen)");
						}
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
