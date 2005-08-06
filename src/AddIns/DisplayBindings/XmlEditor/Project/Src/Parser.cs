// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			return new DefaultCompilationUnit(projectContent);
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			return new DefaultCompilationUnit(projectContent);
		}
		
		public bool CanParse(IProject project)
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
