// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
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
		
		public ParseInformation ParseInformation {
			get {
				return parseInformation;
			}
		}
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
}
