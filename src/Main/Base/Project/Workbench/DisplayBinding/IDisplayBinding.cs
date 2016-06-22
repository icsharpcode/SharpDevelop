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
using System.IO;
using System.Linq;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface IDisplayBinding
	{
		bool IsPreferredBindingForFile(FileName fileName);
		
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForFile(FileName fileName);
		
		double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType);
		
		/// <remarks>
		/// Creates a new IViewContent object for the file fileName
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForFile(OpenedFile file);
	}
}
