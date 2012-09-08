// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	public class ParseInformationEventArgs : EventArgs
	{
		IProject parentProject;
		IUnresolvedFile oldUnresolvedFile;
		ParseInformation newParseInformation;
		
		public FileName FileName {
			get {
				if (newParseInformation != null)
					return newParseInformation.FileName;
				else
					return FileName.Create(oldUnresolvedFile.FileName);
			}
		}
		
		/// <summary>
		/// Gets the parent project for this parse information.
		/// Returns null if the parse information is not associated with any project.
		/// </summary>
		public IProject ParentProject {
			get { return parentProject; }
		}
		
		/// <summary>
		/// The old parsed file.
		/// Returns null if no old parse information exists (first parse run).
		/// </summary>
		public IUnresolvedFile OldUnresolvedFile {
			get { return oldUnresolvedFile; }
		}
		
		/// <summary>
		/// The new parsed file.
		/// Returns null if no new parse information exists (file was removed from project).
		/// </summary>
		public IUnresolvedFile NewUnresolvedFile {
			get { return newParseInformation != null ? newParseInformation.UnresolvedFile : null; }
		}
		
		/// <summary>
		/// The new parse information.
		/// Returns null if no new parse information exists (file was removed from project).
		/// </summary>
		public ParseInformation NewParseInformation {
			get { return newParseInformation; }
		}
		
		[Obsolete]
		public IUnresolvedFile OldCompilationUnit {
			get { return this.OldUnresolvedFile; }
		}
		
		[Obsolete]
		public IUnresolvedFile NewCompilationUnit {
			get { return this.NewUnresolvedFile; }
		}
		
		public ParseInformationEventArgs(IProject parentProject, IUnresolvedFile oldUnresolvedFile, ParseInformation newParseInformation)
		{
			if (oldUnresolvedFile == null && newParseInformation == null)
				throw new ArgumentNullException();
			if (oldUnresolvedFile != null && newParseInformation != null) {
				Debug.Assert(FileUtility.IsEqualFileName(oldUnresolvedFile.FileName, newParseInformation.FileName));
			}
			this.parentProject = parentProject;
			this.oldUnresolvedFile = oldUnresolvedFile;
			this.newParseInformation = newParseInformation;
		}
	}
	
	public class ParserUpdateStepEventArgs : EventArgs
	{
		FileName fileName;
		ITextSource content;
		ParseInformation parseInformation;
		
		public ParserUpdateStepEventArgs(FileName fileName, ITextSource content, ParseInformation parseInformation)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (content == null)
				throw new ArgumentNullException("content");
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
		
		public IUnresolvedFile UnresolvedFile {
			get {
				return parseInformation.UnresolvedFile;
			}
		}
	}
}
