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
	public class PartCoverSettingsFactoryTests
	{
		PartCoverSettingsFactory factory;
		PartCoverSettings partCoverSettings;
		MockCSharpProject project;
		MockFileSystem fileSystem;
		
		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new PartCoverSettingsFactory(fileSystem);
			project = new MockCSharpProject();
		}
		
		[Test]
		public void CreatePartCoverSettingsWhenFileDoesNotExistCreatesSettingsWithNoPartCoverIncludes()
		{
			fileSystem.FileExistsReturnValue = false;
			CreatePartCoverSettingsFromFactory();
			Assert.AreEqual(0, partCoverSettings.Include.Count);
		}
		
		void CreatePartCoverSettingsFromFactory()
		{
			partCoverSettings = factory.CreatePartCoverSettings(project);
		}
		
		[Test]
		public void CreatePartCoverSettingsWhenFileExistsCreatesSettingsFromFile()
		{
			string partCoverSettingsXml =
				"<PartCoverSettings>\r\n" +
				"   <Rule>+test</Rule>\r\n" +
				"</PartCoverSettings>";
			
			StringReader reader = new StringReader(partCoverSettingsXml);
			fileSystem.CreateTextReaderReturnValue = reader;
			
			fileSystem.FileExistsReturnValue = true;
			
			CreatePartCoverSettingsFromFactory();
			Assert.AreEqual("test", partCoverSettings.Include[0]);
		}
	}
}
