// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace ICSharpCode.Core
{
	public class Breakpoint
	{
		string fileName;
		int    lineNumber;
		
		bool   isEnabled = true;
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public int LineNumber {
			get {
				return lineNumber;
			}
		}
		
		public bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public Breakpoint(string fileName, int lineNumber)
		{
			this.fileName = fileName;
			this.lineNumber = lineNumber;
		}
	}
	
	public class MethodCall
	{
		public static MethodCall NoDebugInformation = new MethodCall("<no debug information>", String.Empty);
		public static MethodCall Unknown            = new MethodCall("<unknown>", String.Empty);
		
		string methodName;
		string methodLanguage;
		
		public string Name {
			get {
				return methodName;
			}
		}
		
		public string Language {
			get {
				return methodLanguage;
			}
		}
		
		
		public MethodCall(string methodName, string methodLanguage)
		{
			this.methodName = methodName;
			this.methodLanguage = methodLanguage;
		}
	}
}

