// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// Tests the saving and loading of the PartCover settings file. This
	/// file is used to stores the includes and excludes regular expressions
	/// that PartCover uses.
	/// </summary>
	[TestFixture]
	public class PartCoverSettingsTestFixture
	{
		PartCoverSettings settings;
		PartCoverSettings savedSettings;
		StringBuilder savedSettingsXml;
		XmlDocument doc;
		
		[SetUp]
		public void Init()
		{
			settings = new PartCoverSettings();
			settings.Include.Add("[a]*");
			settings.Include.Add("[b]*");
			settings.Exclude.Add("[c]*");
			settings.Exclude.Add("[d]*");
			savedSettingsXml = new StringBuilder();
			settings.Save(new StringWriter(savedSettingsXml));
			savedSettings = new PartCoverSettings(new StringReader(savedSettingsXml.ToString()));

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
		public void PartCoverSettingsFileName()
		{
			MSBuildBasedProject project = new MSBuildBasedProject(MSBuildInternals.CreateEngine());
			project.FileName = @"C:\temp\test.csproj";
			
			Assert.AreEqual(@"C:\temp\test.PartCover.Settings", PartCoverSettings.GetFileName(project));
		}
		
		[Test]
		public void FourRuleElements()
		{			
			Assert.AreEqual(4, doc.SelectNodes("/PartCoverSettings/Rule").Count);
		}
		
		[Test]
		public void FirstRuleElement()
		{
			Assert.IsNotNull(doc.SelectSingleNode("/PartCoverSettings/Rule[text()='+[a]*']"));
		}
	}
}

