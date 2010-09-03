// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public DummyParserServiceTestTreeView(IRegisteredTestFrameworks testFrameworks)
			: base(testFrameworks)
		{
		}
		
		public DummyParserServiceTestTreeView()
			: this(new MockTestFrameworksWithNUnitFrameworkSupport())
		{
		}
		
		/// <summary>
		/// Gets or sets the project content that will be returned from the
		/// GetProjectContent method.
		/// </summary>
		public IProjectContent ProjectContentForProject {
			get { return projectContent; }
			set { projectContent = value; }
		}
		
		public override IProjectContent GetProjectContent(IProject project)
		{
			return projectContent;
		}
	}
}
