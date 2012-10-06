// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Manages the list of display bindings, and the user's default settings (for Open With dialog)
	/// </summary>
	[SDService]
	public interface IDisplayBindingService
	{
		/// <summary>
		/// Attach secondary view contents to the view content.
		/// </summary>
		/// <param name="viewContent">The view content to attach to</param>
		/// <param name="isReattaching">This is a reattaching pass</param>
		void AttachSubWindows(IViewContent viewContent, bool isReattaching);
		
		/// <summary>
		/// Gets the primary display binding for the specified file name.
		/// </summary>
		IDisplayBinding GetBindingPerFileName(FileName filename);
		
		/// <summary>
		/// Gets the default primary display binding for the specified file name.
		/// </summary>
		DisplayBindingDescriptor GetDefaultCodonPerFileName(FileName filename);
		
		/// <summary>
		/// Sets the default display binding for the specified file extension.
		/// </summary>
		void SetDefaultCodon(string extension, DisplayBindingDescriptor bindingDescriptor);
		
		/// <summary>
		/// Gets list of possible primary display bindings for the specified file name.
		/// </summary>
		IReadOnlyList<DisplayBindingDescriptor> GetCodonsPerFileName(FileName filename);
		
		DisplayBindingDescriptor AddExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding);
		void RemoveExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding);
	}
}
