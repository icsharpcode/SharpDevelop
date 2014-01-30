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
using System.Text;
using System.Xml;

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Tests the saving and loading of the OpenCover settings file. This
	/// file is used to stores the includes and excludes regular expressions
	/// that OpenCover uses.
	/// </summary>
	[TestFixture]
	public class OpenCoverSettingsTestFixture : SDTestFixtureBase
	{
		OpenCoverSettings settings;
		OpenCoverSettings savedSettings;
		StringBuilder savedSettingsXml;
		XmlDocument doc;
		
		[SetUp]
		public void Init()
		{
			settings = new OpenCoverSettings();
			settings.Include.Add("[a]*");
			settings.Include.Add("[b]*");
			settings.Exclude.Add("[c]*");
			settings.Exclude.Add("[d]*");
			savedSettingsXml = new StringBuilder();
			settings.Save(new StringWriter(savedSettingsXml));
			savedSettings = new OpenCoverSettings(new StringReader(savedSettingsXml.ToString()));

			doc = new XmlDocument();
			doc.LoadXml(savedSettingsXml.ToString());
		}
		
		[Test]
		public void IsIncludeListSaved()
		{
			Assert.AreEqual(settings.Include, savedSettings.Include);
		}
		
		[Test]
		public void IsExcludedListSaved()
		{
			Assert.AreEqual(settings.Exclude, savedSettings.Exclude);
		}
		
		[Test]
		public void OpenCoverSettingsFileName()
		{
			MSBuildBasedProject project = new MSBuildBasedProject(
				new ProjectCreateInformation(MockSolution.Create(), new FileName(@"C:\temp\test.csproj")));
			
			Assert.AreEqual(@"C:\temp\test.OpenCover.Settings", OpenCoverSettings.GetFileName(project));
		}
		
		[Test]
		public void FourRuleElements()
		{			
			Assert.AreEqual(4, doc.SelectNodes("/OpenCoverSettings/Rule").Count);
		}
		
		[Test]
		public void FirstRuleElement()
		{
			Assert.IsNotNull(doc.SelectSingleNode("/OpenCoverSettings/Rule[text()='+[a]*']"));
		}
	}
}
