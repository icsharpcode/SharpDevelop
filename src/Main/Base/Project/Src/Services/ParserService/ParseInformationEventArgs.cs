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
		IParsedFile oldParseInformation;
		IParsedFile newParseInformation;
		
		public FileName FileName {
			get { return FileName.Create((oldParseInformation ?? newParseInformation).FileName); }
		}
		
		public IProjectContent ProjectContent {
			get { return (oldParseInformation ?? newParseInformation).ProjectContent; }
		}
		
		public IParsedFile OldCompilationUnit {
			get { return oldParseInformation; }
		}
		
		/// <summary>
		/// The old parse information.
		/// </summary>
		[Obsolete]
		public IParsedFile OldParseInformation {
			get { return oldCompilationUnit; }
		}
		
		/// <summary>
		/// The new parse information.
		/// </summary>
		public IParsedFile NewParseInformation {
			get { return newParseInformation; }
		}
		
		[Obsolete]
		public IParsedFile NewCompilationUnit {
			get { return newParseInformation; }
		}
		
		public bool IsPrimaryParseInfoForFile {
			get; private set;
		}
		
		public ParseInformationEventArgs(IParsedFile oldParseInformation, IParsedFile newParseInformation, bool isPrimaryParseInfoForFile)
		{
			if (oldParseInformation == null && newParseInformation == null)
				throw new ArgumentNullException();
			if (oldParseInformation != null && newParseInformation != null) {
				Debug.Assert(oldParseInformation.ProjectContent == newParseInformation.ProjectContent);
				Debug.Assert(FileUtility.IsEqualFileName(oldParseInformation.FileName, newParseInformation.FileName));
			}
			this.oldParseInformation = oldParseInformation;
			this.newParseInformation = newParseInformation;
			this.IsPrimaryParseInfoForFile = isPrimaryParseInfoForFile;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		FileName fileName;
		ITextSource content;
		IParsedFile parseInformation;
		
		public ParserUpdateStepEventArgs(FileName fileName, ITextSource content, IParsedFile parseInformation)
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
		
		public ITextSource Content {
			get {
				return content;
			}
		}
		
		public IParsedFile ParseInformation {
			get {
				return parseInformation;
			}
		}
	}
}
