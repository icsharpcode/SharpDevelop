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
		
		/// <summary>
		/// Gets or sets the project content that will be returned from the
		/// GetProjectContent method.
		/// </summary>
		public IProjectContent ProjectContentForProject {
			get {
				return projectContent;
			}
			set {
				projectContent = value;
			}
		}
		
		public override IProjectContent GetProjectContent(IProject project)
		{
			return projectContent;
		}
	}
}
