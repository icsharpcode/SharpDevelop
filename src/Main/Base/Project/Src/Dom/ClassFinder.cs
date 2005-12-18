// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		
		public ClassFinder(IMember classMember)
			: this(classMember.DeclaringType, classMember.Region.BeginLine, classMember.Region.BeginColumn)
		{
		}
		
		public ClassFinder(IClass callingClass, int caretLine, int caretColumn)
		{
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.callingClass = callingClass;
			this.cu = callingClass.CompilationUnit;
			this.projectContent = cu.ProjectContent;
		}
		
		// currently callingMember is not required
		public ClassFinder(IClass callingClass, IMember callingMember, int caretLine, int caretColumn)
			: this(callingClass, caretLine, caretColumn)
		{
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
