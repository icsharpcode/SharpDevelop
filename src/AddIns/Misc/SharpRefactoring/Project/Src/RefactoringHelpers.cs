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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
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
		
		/// <summary>
		/// Determines if the property is a so-called auto-implemented property.
		/// </summary>
		public static bool IsAutoImplemented(this IProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			if (property.IsAbstract || property.DeclaringType.ClassType == ICSharpCode.SharpDevelop.Dom.ClassType.Interface)
				return false;
			
			string fileName = property.CompilationUnit.FileName;
			
			if (fileName == null)
				return false;
			
			IDocument document = DocumentUtilitites.LoadDocumentFromBuffer(ParserService.GetParseableFileContent(fileName));
			bool isAutomatic = false;
			
			if (property.CanGet) {
				if (property.GetterRegion.IsEmpty)
					isAutomatic = true;
				else {
					int getterStartOffset = document.PositionToOffset(property.GetterRegion.BeginLine, property.GetterRegion.BeginColumn);
					int getterEndOffset = document.PositionToOffset(property.GetterRegion.EndLine, property.GetterRegion.EndColumn);
					
					string text = document.GetText(getterStartOffset, getterEndOffset - getterStartOffset)
						.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
					
					isAutomatic = text == "get;";
				}
			}
			
			if (property.CanSet) {
				if (property.SetterRegion.IsEmpty)
					isAutomatic |= true;
				else {
					int setterStartOffset = document.PositionToOffset(property.SetterRegion.BeginLine, property.SetterRegion.BeginColumn);
					int setterEndOffset = document.PositionToOffset(property.SetterRegion.EndLine, property.SetterRegion.EndColumn);
					
					string text = document.GetText(setterStartOffset, setterEndOffset - setterStartOffset)
						.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
					
					isAutomatic |= text == "set;";
				}
			}
			
			return isAutomatic;
		}
	}
}
