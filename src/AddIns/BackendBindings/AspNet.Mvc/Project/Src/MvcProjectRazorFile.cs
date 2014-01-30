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

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectRazorFile
	{
		public static MvcProjectFile CreateMvcProjectRazorFile(ProjectItem projectItem)
		{
			FileProjectItem fileProjectItem = projectItem as FileProjectItem;
			return CreateMvcProjectRazorFile(fileProjectItem);
		}
		
		public static MvcProjectFile CreateMvcProjectRazorFile(FileProjectItem fileProjectItem)
		{
			if (IsRazorFile(fileProjectItem)) {
				return new MvcProjectFile(fileProjectItem);
			}
			return null;
		}
		
		public static bool IsRazorFile(FileProjectItem fileProjectItem)
		{
			if (fileProjectItem != null) {
				return IsRazorFileName(fileProjectItem.FileName);
			}
			return false;
		}
		
		public static bool IsRazorFileName(string fileName)
		{
			string extension = MvcFileName.GetLowerCaseFileExtension(fileName);
			return extension == ".cshtml";
		}
	}
}
