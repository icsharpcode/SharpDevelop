// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	[TestFixture]
	public class SolutionWriterTests
	{
		[Test]
		public void GlobalSection()
		{
			SolutionSection section = new SolutionSection("ProjectConfigurationPlatforms", "postSolution");
			section.Add("{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Debug|Any CPU.ActiveCfg", "Debug|Any CPU");
			section.Add("{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Release|Any CPU.ActiveCfg", "Release|Any CPU");
			StringWriter w = new StringWriter();
			new SolutionWriter(w).WriteGlobalSection(section);
			Assert.AreEqual("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution" + w.NewLine +
			                "\t\t{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Debug|Any CPU.ActiveCfg = Debug|Any CPU" + w.NewLine +
			                "\t\t{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Release|Any CPU.ActiveCfg = Release|Any CPU" + w.NewLine +
			                "\tEndGlobalSection" + w.NewLine
			                , w.ToString());
		}
	}
}
