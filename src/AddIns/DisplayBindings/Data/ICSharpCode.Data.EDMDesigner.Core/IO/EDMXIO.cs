// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.Core.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;
using System.IO;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class EDMXIO : IO
    {
        public static EDM Read(string edmxPath, Action<XElement> readMoreAction)
        {
            XElement edmx = XElement.Load(edmxPath);
            return Read(edmx, readMoreAction);
        }

        public static EDM Read(Stream stream, Action<XElement> readMoreAction)
        {
            XElement edmx = XElement.Load(stream);
            return Read(edmx, readMoreAction);
        }
        
        public static EDM Read(XDocument edmxDocument, Action<XElement> readMoreAction)
        {
        	return Read(edmxDocument.Root, readMoreAction);
        }

        private static EDM Read(XElement edmx, Action<XElement> readMoreAction)
        {
            XElement edmxRuntime = edmx.Element(XName.Get("Runtime", edmxNamespace.NamespaceName));

            SSDLContainer ssdlContainer = SSDLIO.ReadXElement(edmxRuntime);
            CSDLContainer csdlContainer = CSDLIO.ReadXElement(edmxRuntime);
            XElement edmxDesigner = edmx.Element(XName.Get("Designer", edmxNamespace.NamespaceName));
            
            if (ssdlContainer == null && csdlContainer == null)
            	return new EDM();

            EDM edm = new EDM()
            {
                SSDLContainer = ssdlContainer,
                CSDLContainer = MSLIO.IntegrateMSLInCSDLContainer(csdlContainer, ssdlContainer, edmxRuntime),
            };

            if (edmxDesigner != null)
            {
                if (edmxDesigner.Element(XName.Get("Connection", edmxNamespace.NamespaceName)) != null)
                    edm.DesignerProperties = edmxDesigner.Element(XName.Get("Connection", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value });

                if (edmxDesigner.Element(XName.Get("Options", edmxNamespace.NamespaceName)) != null)
                    edm.EDMXDesignerDesignerProperties = edmxDesigner.Element(XName.Get("Options", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value });

                if (edmxDesigner.Element(XName.Get("Diagrams", edmxNamespace.NamespaceName)) != null)
                    edm.EDMXDesignerDiagrams = edmxDesigner.Element(XName.Get("Diagrams", edmxNamespace.NamespaceName)).Elements(XName.Get("Diagram", edmxNamespace.NamespaceName));
            }

            readMoreAction(edmx);
            return edm;
        }

        public enum EDMXSection
        { 
            EDMX,
            Runtime,
            SSDL,
            CSDL,
            MSL,
            Designer,
            DesignerViews
        }

        public static XElement ReadSection(XDocument edmxDocument, EDMXSection section)
        {
            return ReadSection(edmxDocument.Root, section);
        }

        public static XElement ReadSection(XElement edmxElement, EDMXSection section)
        {
            if (section == EDMXSection.EDMX)
                return edmxElement;
            
            if (edmxElement == null)
                throw new ArgumentException("Input file is not a valid EDMX file.");

            if (section == EDMXSection.Designer || section == EDMXSection.DesignerViews)
            {
                XElement designerElement = edmxElement.Element(XName.Get("Designer", edmxNamespace.NamespaceName));

                if (designerElement == null)
                    return null;

                if (section == EDMXSection.Designer)
                    return designerElement;
                else
                {
                    XElement diagramsElement = designerElement.Element(XName.Get("Diagrams", edmxNamespace.NamespaceName));

                    if (diagramsElement == null)
                        throw new ArgumentException("Input file is not a valid EDMX file.");

                    return diagramsElement.Element(XName.Get("DesignerViews"));
                }
            }

            XElement runtimeElement = edmxElement.Element(XName.Get("Runtime", edmxNamespace.NamespaceName));

            if (runtimeElement == null)
                throw new ArgumentException("Input file is not a valid EDMX file.");

            if (section == EDMXSection.Runtime)
                return runtimeElement;

            switch (section)
            { 
                case EDMXSection.SSDL:
                    XElement storageModelsElement = runtimeElement.Element(XName.Get("StorageModels", edmxNamespace.NamespaceName));

                    if (storageModelsElement == null)
                        throw new ArgumentException("Input file is not a valid EDMX file.");

                    return storageModelsElement;
                case EDMXSection.CSDL:
                    XElement conceptualModelsElement = runtimeElement.Element(XName.Get("ConceptualModels", edmxNamespace.NamespaceName));

                    if (conceptualModelsElement == null)
                        throw new ArgumentException("Input file is not a valid EDMX file.");

                    return conceptualModelsElement;
                case EDMXSection.MSL:
                    XElement mappingsElement = runtimeElement.Element(XName.Get("Mappings", edmxNamespace.NamespaceName));

                    if (mappingsElement == null)
                        throw new ArgumentException("Input file is not a valid EDMX file.");

                    return mappingsElement;
            }

            return null;
        }

        public static XDocument WriteXDocument(XDocument ssdlDocument, XDocument csdlDocument, XDocument mslDocument)
        {
            return WriteXDocument(ssdlDocument.Root, csdlDocument.Root, mslDocument.Root, null);
        }

        public static XDocument WriteXDocument(XElement ssdlElement, XElement csdlElement, XElement mslElement, XElement designerElement)
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(edmxNamespace + "Edmx", new XAttribute("Version", "1.0"), new XAttribute(XNamespace.Xmlns + "edmx", edmxNamespace.NamespaceName),
                    new XElement(edmxNamespace + "Runtime",
                        new XElement(edmxNamespace + "StorageModels",
                            ssdlElement),
                        new XElement(edmxNamespace + "ConceptualModels",
                            csdlElement),
                        new XElement(edmxNamespace + "Mappings",
                            mslElement))).AddElement(designerElement));
        }

        public static XDocument WriteXDocument(EDMView edmView)
        {
            if (edmView.EDM.IsEmpty) {
                return WriteXDocument(null, null, null, null);
            }
            XElement ssdlElement = SSDLIO.WriteXElement(edmView.EDM.SSDLContainer);
            XElement csdlElement = CSDLIO.Write(edmView.EDM.CSDLContainer);
            XElement mslElement = MSLIO.Write(edmView.EDM);
            XElement designerElement = DesignerIO.Write(edmView);

            return WriteXDocument(ssdlElement, csdlElement, mslElement, designerElement);
        }
    }
}
