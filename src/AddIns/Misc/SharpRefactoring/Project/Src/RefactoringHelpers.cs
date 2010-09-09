// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace SharpRefactoring
{
	public static class RefactoringHelpers
	{
		/// <summary>
		/// Renames file as well as files it is dependent upon.
		/// </summary>
		public static void RenameFile(IProject p, string oldFileName, string newFileName)
		{
			FileService.RenameFile(oldFileName, newFileName, false);
			if (p != null) {
				string oldPrefix = Path.GetFileNameWithoutExtension(oldFileName) + ".";
				string newPrefix = Path.GetFileNameWithoutExtension(newFileName) + ".";
				foreach (ProjectItem item in p.Items) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem == null)
						continue;
					string dependentUpon = fileItem.DependentUpon;
					if (string.IsNullOrEmpty(dependentUpon))
						continue;
					string directory = Path.GetDirectoryName(fileItem.FileName);
					dependentUpon = Path.Combine(directory, dependentUpon);
					if (FileUtility.IsEqualFileName(dependentUpon, oldFileName)) {
						fileItem.DependentUpon = FileUtility.GetRelativePath(directory, newFileName);
						string fileName = Path.GetFileName(fileItem.FileName);
						if (fileName.StartsWith(oldPrefix)) {
							RenameFile(p, fileItem.FileName, Path.Combine(directory, newPrefix + fileName.Substring(oldPrefix.Length)));
						}
					}
				}
			}
		}
		
		public static IClass GetCurrentClassPart(this IClass c, string fileName)
		{
			if (c is CompoundClass) {
				foreach (IClass part in ((CompoundClass)c).Parts) {
					if (fileName.Equals(part.CompilationUnit.FileName, StringComparison.OrdinalIgnoreCase))
						return part;
				}
			}
			
			return c;
		}
	}
}
