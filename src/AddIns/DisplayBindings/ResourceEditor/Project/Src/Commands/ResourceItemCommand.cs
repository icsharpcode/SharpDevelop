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
using ICSharpCode.SharpDevelop;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Commands
{
	/// <summary>
	/// Represents a <see cref="ResourceEditor.ViewModels.ResourceItem"/>-related command.
	/// </summary>
	public class ResourceItemCommand : SimpleCommand
	{
		/// <summary>
		/// Defines whether this command supports multiselection of items.
		/// If not, command will be disabled by default, when more than 1 item is selected.
		/// </summary>
		public virtual bool SupportsMultiSelections {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Defines whether this command is active when nothing is selected.
		/// </summary>
		public virtual bool EmptySelectionAllowed {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Returns list of resource item types for which this command may be enabled or <c>null</c> to allow any type.
		/// </summary>
		public virtual IEnumerable<ResourceItemEditorType> AllowedTypes {
			get {
				return null;
			}
		}
		
		public override bool CanExecute(object parameter)
		{
			if (ResourceEditor == null)
				return false;
			
			var selectedResourceItems = GetSelectedItems();
			if (!EmptySelectionAllowed && !selectedResourceItems.Any())
				return false;
			if (!SupportsMultiSelections && (selectedResourceItems.Count() > 1))
				return false;
			
			return CanExecuteWithResourceItems(selectedResourceItems);
		}
		
		/// <summary>
		/// Checks whether this command can be executed for the current set of selected resource items.
		/// </summary>
		/// <param name="resourceItems">List of selected resource items. Will always contain at least one element.</param>
		/// <returns><c>True</c>, when command can be executed, <c>false</c> otherwise.</returns>
		public virtual bool CanExecuteWithResourceItems(IEnumerable<ResourceItem> resourceItems)
		{
			return true;
		}
		
		public override void Execute(object parameter)
		{
			var selectedResourceItems = GetSelectedItems();
			if (!EmptySelectionAllowed && !selectedResourceItems.Any())
				return;
			if (!SupportsMultiSelections && (selectedResourceItems.Count() > 1))
				return;
			ExecuteWithResourceItems(selectedResourceItems);
		}
		
		/// <summary>
		/// Executes command for the given set of selected resource items.
		/// </summary>
		/// <param name="resourceItems">List of selected resource items. Will always contain at least one element.</param>
		public virtual void ExecuteWithResourceItems(IEnumerable<ResourceItem> resourceItems)
		{
		}
		
		public ResourceEditorViewModel ResourceEditor {
			get {
				return ((ResourceEditViewContent) SD.Workbench.ActiveViewContent).ResourceEditor;
			}
		}
		
		protected IEnumerable<ResourceItem> GetSelectedItems()
		{
			var editor = ResourceEditor;
			if (editor != null)
				return editor.GetSelectedItems();
			return new ResourceItem[0];
		}
	}
}
