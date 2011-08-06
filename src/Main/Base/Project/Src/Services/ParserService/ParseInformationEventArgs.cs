// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	public class ParseInformationEventArgs : EventArgs
	{
		IParsedFile oldParsedFile;
		ParseInformation newParseInformation;
		
		public FileName FileName {
			get {
				if (newParseInformation != null)
					return newParseInformation.FileName;
				else
					return FileName.Create(oldParsedFile.FileName);
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				if (newParseInformation != null)
					return newParseInformation.ProjectContent;
				else
					return oldParsedFile.ProjectContent;
			}
		}
		
		/// <summary>
		/// The old parsed file.
		/// Returns null if no old parse information exists (first parse run).
		/// </summary>
		public IParsedFile OldParsedFile {
			get { return oldParsedFile; }
		}
		
		/// <summary>
		/// The new parsed file.
		/// Returns null if no new parse information exists (file was removed from project).
		/// </summary>
		public IParsedFile NewParsedFile {
			get { return newParseInformation != null ? newParseInformation.ParsedFile : null; }
		}
		
		/// <summary>
		/// The new parse information.
		/// Returns null if no new parse information exists (file was removed from project).
		/// </summary>
		public ParseInformation NewParseInformation {
			get { return newParseInformation; }
		}
		
		[Obsolete]
		public IParsedFile OldCompilationUnit {
			get { return this.OldParsedFile; }
		}
		
		[Obsolete]
		public IParsedFile NewCompilationUnit {
			get { return this.NewParsedFile; }
		}
		
		/// <summary>
		/// Gets whether this parse information is the primary information for the given file.
		/// Secondary parse informations exist when a single file is used in multiple projects.
		/// </summary>
		public bool IsPrimaryParseInfoForFile {
			get; private set;
		}
		
		public ParseInformationEventArgs(IParsedFile oldParsedFile, ParseInformation newParseInformation, bool isPrimaryParseInfoForFile)
		{
			if (oldParsedFile == null && newParseInformation == null)
				throw new ArgumentNullException();
			if (oldParsedFile != null && newParseInformation != null) {
				Debug.Assert(oldParsedFile.ProjectContent == newParseInformation.ProjectContent);
				Debug.Assert(FileUtility.IsEqualFileName(oldParsedFile.FileName, newParseInformation.FileName));
			}
			this.oldParsedFile = oldParsedFile;
			this.newParseInformation = newParseInformation;
			this.IsPrimaryParseInfoForFile = isPrimaryParseInfoForFile;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		FileName fileName;
		ITextSource content;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(FileName fileName, ITextSource content, ParseInformation parseInformation)
		{
			if (parseInformation == null)
				throw new ArgumentNullException("parseInformation");
			this.fileName = fileName;
			this.content = content;
			this.parseInformation = parseInformation;
		}
		
		public FileName FileName {
			get {
				return fileName;
			}
		}
		
		public ITextSource Content {
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
