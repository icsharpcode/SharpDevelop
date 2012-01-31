// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Parser
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
		
		public ParseInformationEventArgs(IParsedFile oldParsedFile, ParseInformation newParseInformation)
		{
			if (oldParsedFile == null && newParseInformation == null)
				throw new ArgumentNullException();
			if (oldParsedFile != null && newParseInformation != null) {
				Debug.Assert(FileUtility.IsEqualFileName(oldParsedFile.FileName, newParseInformation.FileName));
			}
			this.oldParsedFile = oldParsedFile;
			this.newParseInformation = newParseInformation;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		FileName fileName;
		ITextSource content;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(FileName fileName, ITextSource content, ParseInformation parsedFile)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (content == null)
				throw new ArgumentNullException("content");
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			this.fileName = fileName;
			this.content = content;
			this.parseInformation = parsedFile;
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
		
		public IParsedFile ParsedFile {
			get {
				return parseInformation.ParsedFile;
			}
		}
	}
}
