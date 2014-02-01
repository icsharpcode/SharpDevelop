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
using System.Collections.Generic;
using System.Data.Entity.Design;
using System.Data.Metadata.Edm;
using System.IO;
using System.Xml.Linq;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
	public class EntityModelCodeGenerator : IO, ICustomTool
	{
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			XElement schema = GetModelSchema(item);
			XDocument csdlDocument = CreateCsdlDocument(schema);
			
			string tempFileName = IO.GetTempFilenameWithExtension("csdl");
			csdlDocument.Save(tempFileName);

			LanguageOption languageToGenerateCode = GetLanguageOption(item);
			string outputFileName = context.GetOutputFileName(item, "Designer");

			EntityCodeGenerator entityCodeGenerator = new EntityCodeGenerator(languageToGenerateCode);
			AddNamespaceMapping(entityCodeGenerator, schema, context.OutputNamespace);
			IList<EdmSchemaError> edmSchemaErrors = entityCodeGenerator.GenerateCode(tempFileName, outputFileName);
			File.Delete(tempFileName);
			
			context.EnsureOutputFileIsInProject(item, outputFileName);
		}
		
		XElement GetModelSchema(FileProjectItem item)
		{
			XDocument edmxDocument = XDocument.Load(item.FileName);
			XElement conceptualModelsElement = EDMXIO.ReadSection(edmxDocument, EDMXIO.EDMXSection.CSDL);
			if (conceptualModelsElement == null)
				throw new ArgumentException("Input file is not a valid EDMX file.");

			return conceptualModelsElement.Element(XName.Get("Schema", csdlNamespace.NamespaceName));
		}
		
		XDocument CreateCsdlDocument(XElement schema)
		{
			return new XDocument(new XDeclaration("1.0", "utf-8", null), schema);
		}
		
		LanguageOption GetLanguageOption(FileProjectItem item)
		{
			if (item.Project.Language != "C#")
				return LanguageOption.GenerateVBCode;
			return LanguageOption.GenerateCSharpCode;
		}
		
		void AddNamespaceMapping(EntityCodeGenerator entityCodeGenerator, XElement schema, string outputNamespace)
		{
			if (!String.IsNullOrEmpty(outputNamespace))
				AddNamespaceMapping(entityCodeGenerator, schema.Attribute(XName.Get("Namespace")), outputNamespace);
		}
		
		void AddNamespaceMapping(EntityCodeGenerator entityCodeGenerator, XAttribute edmNamespace, string outputNamespace)
		{
			if (edmNamespace != null)
				entityCodeGenerator.EdmToObjectNamespaceMap.Add(edmNamespace.Value, outputNamespace);
		}
	}
}
