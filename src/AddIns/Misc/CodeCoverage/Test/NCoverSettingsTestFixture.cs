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

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class NCoverSettingsTestFixture
	{
		NCoverSettings settings;
		NCoverSettings savedSettings;

		[SetUp]
		public void Init()
		{
			settings = new NCoverSettings();
			settings.AssemblyList = "MyNamespace.Foo; MyNamespace.Bar";
			settings.ExcludedAttributesList = "NUnit.Framework.TestFixtureAttribute; NUnit.Framework.TestAttribute";
			StringBuilder savedSettingsXml = new StringBuilder();
			settings.Save(new StringWriter(savedSettingsXml));
			savedSettings = new NCoverSettings(new StringReader(savedSettingsXml.ToString()));
		}
		
		[Test]
		public void IsAssemblyListSaved()
		{
			Assert.AreEqual(settings.AssemblyList, savedSettings.AssemblyList);
		}
		
		[Test]
		public void IsExcludedAttributeListListSaved()
		{
			Assert.AreEqual(settings.ExcludedAttributesList, savedSettings.ExcludedAttributesList);
		}
		
		[Test]
		public void NCoverSettingsFileName()
		{
			MSBuildBasedProject project = new MSBuildBasedProject(MSBuildInternals.CreateEngine());
			project.FileName = @"C:\temp\test.csproj";
			
			Assert.AreEqual(@"C:\temp\test.NCover.Settings", NCoverSettings.GetFileName(project));
		}
	}
}
