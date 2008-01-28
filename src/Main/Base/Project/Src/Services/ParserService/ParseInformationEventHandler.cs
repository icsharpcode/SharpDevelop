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
	public delegate void ParseInformationEventHandler(object sender, ParseInformationEventArgs e);
	
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
	
	public delegate void ParserUpdateStepEventHandler(object sender, ParserUpdateStepEventArgs e);
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		string fileName;
		string content;
		bool updated;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(string fileName, string content, bool updated, ParseInformation parseInformation)
		{
			this.fileName = fileName;
			this.content = content;
			this.updated = updated;
			this.parseInformation = parseInformation;
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		public string Content {
			get {
				return content;
			}
		}
		public bool Updated {
			get {
				return updated;
			}
		}
		
		public ParseInformation ParseInformation {
			get {
				return parseInformation;
			}
		}
	}
}
