#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using System.Data.Entity.Design;
using System.CodeDom;
using System.Data.Metadata.Edm;
using System.Xml.Linq;
using System.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class EntityModelCodeGenerator : IO, ICustomTool
    {
        #region ICustomTool Member

        public void GenerateCode(FileProjectItem item, CustomToolContext context)
        {
            LanguageOption languageToGenerateCode = LanguageOption.GenerateCSharpCode;

            if (item.Project.Language != "C#")
                languageToGenerateCode = LanguageOption.GenerateVBCode;

            XDocument edmxDocument = XDocument.Load(item.FileName);
            XElement conceptualModelsElement = EDMXIO.ReadSection(edmxDocument, EDMXIO.EDMXSection.CSDL);

            if (conceptualModelsElement == null)
                throw new ArgumentException("Input file is not a valid EDMX file.");

            XDocument csdlDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), conceptualModelsElement.Element(XName.Get("Schema", csdlNamespace.NamespaceName)));
            
            string tempFileName = IO.GetTempFilenameWithExtension("csdl");
            csdlDocument.Save(tempFileName);

            string outputFileName = context.GetOutputFileName(item, "Designer");

            EntityCodeGenerator entityCodeGenerator = new EntityCodeGenerator(languageToGenerateCode);
            IList<EdmSchemaError> edmSchemaErrors = entityCodeGenerator.GenerateCode(tempFileName, outputFileName);
            File.Delete(tempFileName);
            
            context.EnsureOutputFileIsInProject(item, outputFileName);
        }

        #endregion
    }
}
