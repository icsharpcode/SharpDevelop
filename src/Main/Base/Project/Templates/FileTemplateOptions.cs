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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Argument class for file templates.
	/// </summary>
	public class FileTemplateOptions
	{
		/// <summary>
		/// Gets/Sets whether the file being created will be untitled.
		/// </summary>
		public bool IsUntitled { get; set; }
		
		/// <summary>
		/// The parent project to which this file is added.
		/// Can be null when creating a file outside of a project.
		/// </summary>
		public IProject Project { get; set; }
		
		/// <summary>
		/// The name of the file
		/// </summary>
		public FileName FileName { get; set; }
		
		/// <summary>
		/// The default namespace to use for the newly created file.
		/// </summary>
		public string Namespace { get; set; }
		
		/// <summary>
		/// The class name (generated from the file name).
		/// </summary>
		public string ClassName { get; set; }
		
		/// <summary>
		/// The object that was created by <see cref="FileTemplate.CreateCustomizationObject"/>
		/// and subsequently customized by the user in the dialog's property grid.
		/// </summary>
		public object CustomizationObject { get; set; }
	}
}
