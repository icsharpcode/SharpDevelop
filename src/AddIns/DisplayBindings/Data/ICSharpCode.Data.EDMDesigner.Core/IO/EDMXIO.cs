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
            XElement edmxRuntime = edmx.Element(XName.Get("Runtime", "http://schemas.microsoft.com/ado/2007/06/edmx"));

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
                edm.DesignerProperties = edmxDesigner.Element(XName.Get("Connection", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value });
                edm.EDMXDesignerDesignerProperties = edmxDesigner.Element(XName.Get("Options", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value });
                edm.EDMXDesignerDiagrams = edmxDesigner.Element(XName.Get("Diagrams", edmxNamespace.NamespaceName)).Elements(XName.Get("Diagram", edmxNamespace.NamespaceName));
            }

            readMoreAction(edmx);
            return edm;
        }
    }
}
