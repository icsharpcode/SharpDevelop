// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using ICSharpCode.XmlBinding.Gui;
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XmlBinding.Parser
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
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			DefaultCompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			return compilationUnit;
		}
		
		public bool CanParse(IProject project)
		{
			return false;
		}
		
		public bool CanParse(string fileName)
		{
			return XmlDisplayBinding.IsFileNameHandled(fileName);
		}
		#endregion
	}
}
