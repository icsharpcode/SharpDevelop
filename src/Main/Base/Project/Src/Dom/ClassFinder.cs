// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Interface for objects that can find classes.
	/// </summary>
	public class ClassFinder
	{
		int caretLine, caretColumn;
		ICompilationUnit cu;
		IClass callingClass;
		IProjectContent projectContent;
		
		public ClassFinder(string fileName, string fileContent, int offset)
		{
			caretLine = 0;
			caretColumn = 0;
			for (int i = 0; i < offset; i++) {
				if (fileContent[i] == '\n') {
					caretLine++;
					caretColumn = 0;
				} else {
					caretColumn++;
				}
			}
			Init(fileName);
		}
		
		public ClassFinder(string fileName, int caretLineNumber, int caretColumn)
		{
			this.caretLine   = caretLineNumber;
			this.caretColumn = caretColumn;
			
			Init(fileName);
		}
		
		void Init(string fileName)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) {
				return;
			}
			
			cu = parseInfo.MostRecentCompilationUnit;
			
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
				projectContent = cu.ProjectContent;
			} else {
				projectContent = ParserService.CurrentProjectContent;
			}
		}
		
		public IClass GetClass(string fullName, int typeParameterCount)
		{
			return projectContent.GetClass(fullName, typeParameterCount);
		}
		
		public IReturnType SearchType(string name, int typeParameterCount)
		{
			return projectContent.SearchType(name, typeParameterCount, callingClass, cu, caretLine, caretColumn);
		}
		
		public string SearchNamespace(string name)
		{
			return projectContent.SearchNamespace(name, callingClass, cu, caretLine, caretColumn);
		}
	}
}
