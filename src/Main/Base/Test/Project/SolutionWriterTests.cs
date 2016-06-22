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
