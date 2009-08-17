// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class ParseInformationEventArgs : EventArgs
	{
		string fileName;
		IProjectContent projectContent;
		ICompilationUnit oldCompilationUnit;
		ICompilationUnit newCompilationUnit;
		
		public string FileName {
			get { return fileName; }
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		/// <summary>
		/// Gets the old compilation unit.
		/// </summary>
		public ICompilationUnit OldCompilationUnit {
			get { return oldCompilationUnit; }
		}
		
		/// <summary>
		/// The new compilation unit.
		/// </summary>
		public ICompilationUnit NewCompilationUnit {
			get { return newCompilationUnit; }
		}
		
		public ParseInformationEventArgs(string fileName, IProjectContent projectContent, ICompilationUnit oldCompilationUnit, ICompilationUnit newCompilationUnit)
		{
			this.fileName = fileName;
			this.projectContent = projectContent;
			this.oldCompilationUnit = oldCompilationUnit;
			this.newCompilationUnit = newCompilationUnit;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		string fileName;
		ITextBuffer content;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(string fileName, ITextBuffer content, ParseInformation parseInformation)
		{
			this.fileName = fileName;
			this.content = content;
			this.parseInformation = parseInformation;
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public ITextBuffer Content {
			get {
				return content;
			}
		}
		
		public ParseInformation ParseInformation {
			get {
				return parseInformation;
			}
		}
	}
}
