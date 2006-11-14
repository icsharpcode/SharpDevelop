// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using System;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// Provides a way to return dummy data for IProjectContents
	/// without having to use the ParserService.
	/// </summary>
	public class DummyParserServiceTestTreeView : TestTreeView
	{
		IProjectContent projectContent;
		
		public void AddProjectContentForProject(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public override IProjectContent GetProjectContent(IProject project)
		{
			return projectContent;
		}
	}
}
