// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface IDisplayBinding
	{
		bool IsPreferredBindingForFile(string fileName);
		
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForFile(string fileName);
		
		double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType);
		
		/// <remarks>
		/// Creates a new IViewContent object for the file fileName
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForFile(OpenedFile file);
	}
	

}
