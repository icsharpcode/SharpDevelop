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
using System.Globalization;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	[Serializable]
	public class BuildError
	{
		public BuildError()
		{
			this.line = -1;
			this.column = -1;
			this.errorCode = string.Empty;
			this.errorText = string.Empty;
			this.fileName = string.Empty;
		}
		
		public BuildError(string fileName, string errorText)
		{
			this.line = -1;
			this.column = -1;
			this.errorCode = string.Empty;
			this.errorText = errorText;
			this.fileName = fileName;
		}
		
		public BuildError(string fileName, int line, int column, string errorCode, string errorText)
		{
			this.line = line;
			this.column = column;
			this.errorCode = errorCode;
			this.errorText = errorText;
			this.fileName = fileName;
		}
		
		int column;
		string errorCode;
		string errorText;
		string fileName;
		int line;
		bool isWarning;
		bool isMessage;
		[NonSerialized]
		object tag;
		string contextMenuAddInTreeEntry;
		string subcategory;
		string helpKeyword;
		IProject parentProject;
		
		public string HelpKeyword {
			get { return helpKeyword; }
			set { helpKeyword = value; }
		}
		
		public string Subcategory {
			get { return subcategory; }
			set { subcategory = value; }
		}
		
		public int Column {
			get {
				return column;
			}
			set {
				column = value;
			}
		}
		
		public string ErrorCode {
			get {
				return errorCode;
			}
			set {
				errorCode = value;
			}
		}
		
		public string ErrorText {
			get {
				return errorText;
			}
			set {
				errorText = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public int Line {
			get {
				return line;
			}
			set {
				line = value;
			}
		}
		
		public bool IsWarning {
			get { return isWarning; }
			set { isWarning = value; }
		}
		
		public bool IsMessage {
			get { return isMessage; }
			set { isMessage = value; }
		}
		
		/// <summary>
		/// The project that contains this error. This property can be null.
		/// </summary>
		public IProject ParentProject {
			get { return parentProject; }
			set { parentProject = value; }
		}
		
		/// <summary>
		/// Allows to store any object with this error. An object might be attached by a custom
		/// MSBuild logger and later read by the context menu command.
		/// </summary>
		/// <remarks>The Tag property is [NonSerialized], which shouldn't be a problem
		/// because both custom loggers and context menu commands are guaranteed to run
		/// in the main AppDomain.</remarks>
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public string ContextMenuAddInTreeEntry {
			get {
				return contextMenuAddInTreeEntry;
			}
			set {
				contextMenuAddInTreeEntry = value;
			}
		}
		
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.FileName)) {
				return string.Format(CultureInfo.CurrentCulture,
				                     "{0} {1}: {2}",
				                     StringParser.Parse(this.IsWarning ? "${res:Global.WarningText}" : "${res:Global.ErrorText}"),
				                     this.ErrorCode, this.ErrorText);
			} else {
				return string.Format(CultureInfo.CurrentCulture,
				                     "{0}({1},{2}) : {3} {4}: {5}",
				                     this.FileName, this.Line, this.Column,
				                     StringParser.Parse(this.IsWarning ? "${res:Global.WarningText}" : "${res:Global.ErrorText}"),
				                     this.ErrorCode, this.ErrorText);
			}
		}
		
		public RichText ToRichText()
		{
			// TODO: add some color
			return new RichText(ToString());
		}
	}
}
