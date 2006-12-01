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
		ParseInformation parseInformation;
		ICompilationUnit compilationUnit;
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Gets the parse information. The new compilation unit has not yet been added to the parse information
		/// (but will be immediately after the event was executed, be careful when invoking back to the main thread),
		/// you can use this property to get the previous compilation unit.
		/// </summary>
		public ParseInformation ParseInformation {
			get {
				return parseInformation;
			}
		}
		
		/// <summary>
		/// The new compilation unit.
		/// </summary>
		public ICompilationUnit CompilationUnit {
			get {
				return compilationUnit;
			}
		}
		
		public ParseInformationEventArgs(string fileName, ParseInformation parseInformation, ICompilationUnit compilationUnit)
		{
			this.fileName         = fileName;
			this.parseInformation = parseInformation;
			this.compilationUnit  = compilationUnit;
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
