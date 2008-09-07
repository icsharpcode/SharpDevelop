// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace Base
{
	/// <summary>
	/// Interface for classes that are able to open a file and create a <see cref="IViewContent"/> for it.
	/// </summary>
	public interface IDisplayBinding
	{
		/// <summary>
		/// Loads the file and opens a <see cref="IViewContent"/>.
		/// When this method returns <c>null</c>, the display binding cannot handle the file type.
		/// </summary>
		IViewContent OpenFile(string fileName);
	}
	
	public static class DisplayBindingManager
	{
		static List<IDisplayBinding> items;
		
		public static IViewContent CreateViewContent(string fileName)
		{
			if (items == null) {
				items = AddInTree.BuildItems<IDisplayBinding>("/Workspace/DisplayBindings", null, true);
			}
			foreach (IDisplayBinding binding in items) {
				IViewContent content = binding.OpenFile(fileName);
				if (content != null) {
					return content;
				}
			}
			return null;
		}
	}
}
