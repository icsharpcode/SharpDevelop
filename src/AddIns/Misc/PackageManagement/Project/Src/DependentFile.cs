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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class DependentFile
	{
		IProject project;
		
		public DependentFile(IProject project)
		{
			this.project = project;
		}
		
		public FileProjectItem GetParentFileProjectItem(string fileName)
		{
			string parentFileName = GetParentFileName(fileName);
			if (parentFileName != null) {
				return project.FindFile(FileName.Create(parentFileName));
			}
			return null;
		}
		
		string GetParentFileName(string fileName)
		{
			if (IsResxFile(fileName)) {
				return GetParentFileNameForResxFile(fileName);
			} else if (IsDesignerFile(fileName)) {
				return GetParentFileNameForDesignerFile(fileName);
			}
			return null;
		}
		
		bool IsResxFile(string fileName)
		{
			return HasMatchingFileExtension(".resx", fileName);
		}
		
		string GetParentFileNameForResxFile(string fileName)
		{
			string fileExtension = GetFileExtensionForProjectSourceCodeFiles();
			return Path.ChangeExtension(fileName, fileExtension);
		}
		
		bool IsDesignerFile(string fileName)
		{
			string fileNameWithoutPath = Path.GetFileName(fileName);
			return GetIndexOfDesignerNameInFileName(fileNameWithoutPath) > 0;
		}
		
		int GetIndexOfDesignerNameInFileName(string fileName)
		{
			return fileName.LastIndexOf(".designer", StringComparison.OrdinalIgnoreCase);
		}
		
		string GetParentFileNameForDesignerFile(string fileName)
		{
			int index = GetIndexOfDesignerNameInFileName(fileName);
			return fileName.Substring(0, index) + Path.GetExtension(fileName);
		}
		
		string GetFileExtensionForProjectSourceCodeFiles()
		{
			if (IsVisualBasicProjectFile(project.FileName)) {
				return ".vb";
			}
			return ".cs";
		}
		
		bool IsVisualBasicProjectFile(string fileName)
		{
			return HasMatchingFileExtension(".vbproj", fileName);
		}
		
		bool HasMatchingFileExtension(string extension, string fileName)
		{
			string otherExtension = Path.GetExtension(fileName);
			return IsMatchIgnoringCase(extension, otherExtension);
		}
		
		bool IsMatchIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
