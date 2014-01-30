// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
