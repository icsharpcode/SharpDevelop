// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class ParseInformationEventArgs : EventArgs
	{
		FileName fileName;
		IProjectContent projectContent;
		ICompilationUnit oldCompilationUnit;
		ParseInformation newParseInformation;
		
		public FileName FileName {
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
		/// The new parse information.
		/// </summary>
		public ParseInformation NewParseInformation {
			get { return newParseInformation; }
		}
		
		/// <summary>
		/// The new compilation unit.
		/// </summary>
		public ICompilationUnit NewCompilationUnit {
			get {
				if (newParseInformation != null)
					return newParseInformation.CompilationUnit;
				else
					return null;
			}
		}
		
		public bool IsPrimaryParseInfoForFile {
			get; private set;
		}
		
		public ParseInformationEventArgs(FileName fileName, IProjectContent projectContent, ICompilationUnit oldCompilationUnit, ParseInformation newParseInformation, bool isPrimaryParseInfoForFile)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			this.fileName = fileName;
			this.projectContent = projectContent;
			this.oldCompilationUnit = oldCompilationUnit;
			this.newParseInformation = newParseInformation;
			this.IsPrimaryParseInfoForFile = isPrimaryParseInfoForFile;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		FileName fileName;
		ITextBuffer content;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(FileName fileName, ITextBuffer content, ParseInformation parseInformation)
		{
			this.fileName = fileName;
			this.content = content;
			this.parseInformation = parseInformation;
		}
		
		public FileName FileName {
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
