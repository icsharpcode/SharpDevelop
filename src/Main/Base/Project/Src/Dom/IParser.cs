// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IParser {
		
		string[] LexerTags {
			get;
			set;
		}
		
		IExpressionFinder ExpressionFinder {
			get;
		}
		
		/// <summary>
		/// Gets if the parser can parse the specified file.
		/// This method is used to get the correct parser for a specific file and normally decides based on the file
		/// extension.
		/// </summary>
		bool CanParse(string fileName);
		
		/// <summary>
		/// Gets if the parser can parse the specified project.
		/// Only when no parser for a project is found, the assembly is loaded.
		/// </summary>
		bool CanParse(IProject project);
		
		ICompilationUnit Parse(IProjectContent projectContent, string fileName);
		ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent);
		
		IResolver CreateResolver();
	}
}
