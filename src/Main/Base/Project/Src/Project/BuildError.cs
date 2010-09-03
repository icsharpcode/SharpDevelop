// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
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
		bool warning;
		[NonSerialized]
		object tag;
		string contextMenuAddInTreeEntry;
		string subcategory;
		string helpKeyword;
		
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
			get {
				return warning;
			}
			set {
				warning = value;
			}
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
	}
}
