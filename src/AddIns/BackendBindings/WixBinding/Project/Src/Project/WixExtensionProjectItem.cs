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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public static class WixItemType
	{		
		public const string LibraryName = "WixLibrary";
		public static readonly ItemType Library = new ItemType(LibraryName);

		public const string ExtensionName = "WixExtension";
		public static readonly ItemType Extension = new ItemType(ExtensionName);
		
	}
	
	/// <summary>
	/// Base class for all Wix compiler extension project items.
	/// </summary>
	public class WixExtensionProjectItem : ProjectItem
	{
		public WixExtensionProjectItem(IProject project)
			: base(project, WixItemType.Extension)
		{
		}
		
		public WixExtensionProjectItem(IProject project, IProjectItemBackendStore item)
			: base(project, item)
		{
		}
	}
}
