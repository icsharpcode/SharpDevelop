// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceMapFileGeneratorTests
	{
		ServiceReferenceMapGenerator generator;
		
		void CreateFileGenerator()
		{
			generator = new ServiceReferenceMapGenerator();
		}
		
		string GenerateMapFile(ServiceReferenceMapFile mapFile)
		{
			var output = new StringBuilder();
			var writer = new StringWriter(output);
			
			generator.GenerateServiceReferenceMapFile(writer, mapFile);
			
			return output.ToString();
		}
		
		ServiceReferenceMapFile CreateServiceReferenceMapFileWithUrl(string url)
		{
			return ServiceReferenceMapFile.CreateMapFileWithUrl(url);
		}
		
		[Test]
		public void GenerateServiceReferenceMapFile_MapFileWrittenToStringWriter_SerialisesReferenceGroup()
		{
			CreateFileGenerator();
			ServiceReferenceMapFile mapFile = CreateServiceReferenceMapFileWithUrl("http://localhost/MyService1.svc");
			mapFile.ID = "a606bbd6-26e5-4025-a25e-b8c262422f2a";
			string output = GenerateMapFile(mapFile);
			
			string expectedOutput =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<ReferenceGroup xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" ID=""a606bbd6-26e5-4025-a25e-b8c262422f2a"" xmlns=""urn:schemas-microsoft-com:xml-wcfservicemap"">
  <ClientOptions>
    <GenerateAsynchronousMethods>false</GenerateAsynchronousMethods>
    <EnableDataBinding>true</EnableDataBinding>
    <ImportXmlTypes>false</ImportXmlTypes>
    <GenerateInternalTypes>false</GenerateInternalTypes>
    <GenerateMessageContracts>false</GenerateMessageContracts>
    <GenerateSerializableTypes>true</GenerateSerializableTypes>
    <Serializer>Auto</Serializer>
    <UseSerializerForFaults>true</UseSerializerForFaults>
    <ReferenceAllAssemblies>true</ReferenceAllAssemblies>
  </ClientOptions>
  <MetadataSources>
    <MetadataSource Address=""http://localhost/MyService1.svc"" Protocol=""http"" SourceId=""1"" />
  </MetadataSources>
  <Metadata />
</ReferenceGroup>";
				
			Assert.AreEqual(expectedOutput, output);
		}
	}
}
