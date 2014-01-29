// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	[TestFixture]
	public class SolutionLoaderTests
	{
		[Test]
		public void GlobalSection()
		{
			string input = "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution" + Environment.NewLine +
				"\t\t{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Debug|Any CPU.ActiveCfg = Debug|Any CPU" + Environment.NewLine +
				"\t\t{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Release|Any CPU.ActiveCfg = Release|Any CPU" + Environment.NewLine +
				"\tEndGlobalSection" + Environment.NewLine;
			
			SolutionSection section = new SolutionLoader(new StringReader(input)).ReadSection(isGlobal: true);
			Assert.AreEqual("ProjectConfigurationPlatforms", section.SectionName);
			Assert.AreEqual("postSolution", section.SectionType);
			Assert.AreEqual(2, section.Count);
			var entries = section.ToArray();
			Assert.AreEqual("{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Debug|Any CPU.ActiveCfg", entries[0].Key);
			Assert.AreEqual("Debug|Any CPU", entries[0].Value);
			
			Assert.AreEqual("{3B2A5653-EC97-4001-BB9B-D90F1AF2C371}.Release|Any CPU.ActiveCfg", entries[1].Key);
			Assert.AreEqual("Release|Any CPU", entries[1].Value);
		}
		
		[Test]
		public void DescriptionInSolutionConfigurationPlatforms()
		{
			// http://community.sharpdevelop.net/forums/t/19948.aspx
			string input = "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution" + Environment.NewLine +
				"\t\tDebug|Any CPU = Debug|Any CPU" + Environment.NewLine +
				"\t\tRelease|Any CPU = Release|Any CPU" + Environment.NewLine +
				"\t\tDescription = Some fancy description of the application." + Environment.NewLine +
				"\tEndGlobalSection" + Environment.NewLine;
			
			var loader = new SolutionLoader(new StringReader(input));
			SolutionSection section = loader.ReadSection(isGlobal: true);
			
			var configs = loader.LoadSolutionConfigurations(section).ToArray();
			Assert.AreEqual(new[] {
			                	new ConfigurationAndPlatform("Debug", "Any CPU"),
			                	new ConfigurationAndPlatform("Release", "Any CPU")
			                }, configs);
		}
	}
}
