// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class handles the installed display bindings
	/// and provides a simple access point to these bindings.
	/// </summary>
	internal static class DisplayBindingService
	{
		readonly static string displayBindingPath = "/SharpDevelop/Workbench/DisplayBindings";
		
		static DisplayBindingDescriptor[] bindings = null;
		
		/// <summary>
		/// Gets the primary display binding for the specified file name.
		/// </summary>
		public static IDisplayBinding GetBindingPerFileName(string filename)
		{
			DisplayBindingDescriptor codon = GetCodonPerFileName(filename);
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
		
		/// <summary>
		/// Gets list of possible primary display bindings for the specified file name.
		/// </summary>
		public static IList<DisplayBindingDescriptor> GetCodonsPerFileName(string filename)
		{
			List<DisplayBindingDescriptor> list = new List<DisplayBindingDescriptor>();
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (!binding.IsSecondary && binding.CanAttachToFile(filename)) {
					if (binding.Binding != null && binding.Binding.CanCreateContentForFile(filename)) {
						list.Add(binding);
					}
				}
			}
			return list;
		}
		
		/// <summary>
		/// Attach secondary view contents to the view content.
		/// </summary>
		/// <param name="viewContent">The view content to attach to</param>
		/// <param name="isReattaching">This is a reattaching pass</param>
		public static void AttachSubWindows(IViewContent viewContent, bool isReattaching)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (binding.IsSecondary && binding.CanAttachToFile(viewContent.PrimaryFileName)) {
					ISecondaryDisplayBinding displayBinding = binding.SecondaryBinding;
					if (displayBinding != null
					    && (!isReattaching || displayBinding.ReattachWhenParserServiceIsReady)
					    && displayBinding.CanAttachTo(viewContent))
					{
						IViewContent[] subViewContents = binding.SecondaryBinding.CreateSecondaryViewContent(viewContent);
						if (subViewContents != null) {
							Array.ForEach(subViewContents, viewContent.SecondaryViewContents.Add);
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
