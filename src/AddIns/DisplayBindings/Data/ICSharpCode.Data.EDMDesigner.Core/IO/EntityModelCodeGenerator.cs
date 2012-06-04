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
