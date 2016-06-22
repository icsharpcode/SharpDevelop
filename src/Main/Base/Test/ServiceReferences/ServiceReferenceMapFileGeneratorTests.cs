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
