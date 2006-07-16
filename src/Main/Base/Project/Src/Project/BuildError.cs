// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class BuildError
	{
		public BuildError()
		{
			this.line = 0;
			this.column = 0;
			this.errorCode = string.Empty;
			this.errorText = string.Empty;
			this.fileName = string.Empty;
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
		object tag;
		string contextMenuAddInTreeEntry;
		
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
