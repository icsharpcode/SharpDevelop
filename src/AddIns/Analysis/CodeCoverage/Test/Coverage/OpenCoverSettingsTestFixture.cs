// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Tests the saving and loading of the PartCover settings file. This
	/// file is used to stores the includes and excludes regular expressions
	/// that PartCover uses.
	/// </summary>
	[TestFixture]
	public class OpenCoverSettingsTestFixture
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
				new ProjectCreateInformation {
					Solution = new Solution(new MockProjectChangeWatcher()),
					OutputProjectFileName = @"C:\temp\test.csproj",
					ProjectName = "test"
				});
			
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
