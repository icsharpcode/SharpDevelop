// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
