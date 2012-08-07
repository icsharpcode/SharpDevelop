// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.CodeCoverage;
using ICSharpCode.CodeCoverage.Tests.Utils;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class OpenCoverSettingsFactoryTests
	{
		OpenCoverSettingsFactory factory;
		OpenCoverSettings openCoverSettings;
		MockCSharpProject project;
		MockFileSystem fileSystem;
		
		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new OpenCoverSettingsFactory(fileSystem);
			project = new MockCSharpProject();
		}
		
		[Test]
		public void CreateOpenCoverSettingsWhenFileDoesNotExistCreatesSettingsWithNoPartCoverIncludes()
		{
			fileSystem.FileExistsReturnValue = false;
			CreateOpenCoverSettingsFromFactory();
			Assert.AreEqual(0, openCoverSettings.Include.Count);
		}
		
		void CreateOpenCoverSettingsFromFactory()
		{
			openCoverSettings = factory.CreateOpenCoverSettings(project);
		}
		
		[Test]
		public void CreateOpenCoverSettingsWhenFileExistsCreatesSettingsFromFile()
		{
			string openCoverSettingsXml =
				"<OpenCoverSettings>\r\n" +
				"   <Rule>+test</Rule>\r\n" +
				"</OpenCoverSettings>";
			
			StringReader reader = new StringReader(openCoverSettingsXml);
			fileSystem.CreateTextReaderReturnValue = reader;
			
			fileSystem.FileExistsReturnValue = true;
			
			CreateOpenCoverSettingsFromFactory();
			Assert.AreEqual("test", openCoverSettings.Include[0]);
		}
	}
}
