//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

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
