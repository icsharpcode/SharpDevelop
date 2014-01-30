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
