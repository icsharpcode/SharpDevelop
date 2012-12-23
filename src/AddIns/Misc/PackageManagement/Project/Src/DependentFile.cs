// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

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
				return project.FindFile(parentFileName);
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
