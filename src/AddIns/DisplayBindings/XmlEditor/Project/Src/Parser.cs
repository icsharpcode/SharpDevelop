// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Parser that does nothing except return empty compilation unit
	/// classes so the XmlFoldingStrategy is executed.
	/// </summary>
	public class Parser : IParser
	{
		public Parser()
		{
		}
		
		#region IParser interface
		public string[] LexerTags {
			get {
				return null;
			}
			set {
			}
		}
		
		public LanguageProperties Language {
			get {
				return null;
			}
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return null;
		}
		
		public IResolver CreateResolver()
		{
			return null;
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName)
		{
			return Parse(projectContent, fileName, null);
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			DefaultCompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			return compilationUnit;
		}
		
		public bool CanParse(IDomProject project)
		{
			return false;
		}
		
		public bool CanParse(string fileName)
		{
			return XmlView.IsFileNameHandled(fileName);
		}
		#endregion
	}
}
