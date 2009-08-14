#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class EDMXIO : IO
    {
        public static EDM Read(string edmxPath, Action<XElement> readMoreAction)
        {
            XElement edmx = XElement.Load(edmxPath);
            XElement edmxRuntime = edmx.Element(XName.Get("Runtime", "http://schemas.microsoft.com/ado/2007/06/edmx"));

            SSDLContainer ssdlContainer = SSDLIO.ReadXElement(edmxRuntime);
            CSDLContainer csdlContainer = CSDLIO.ReadXElement(edmxRuntime);
            XElement edmxDesigner = edmx.Element(XName.Get("Designer", edmxNamespace.NamespaceName));

            EDM edm = new EDM()
            { 
                SSDLContainer = ssdlContainer,
                CSDLContainer = MSLIO.IntegrateMSLInCSDLContainer(csdlContainer, ssdlContainer, edmxRuntime), 
                DesignerProperties = edmxDesigner.Element(XName.Get("Connection", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value }), 
                EDMXDesignerDesignerProperties = edmxDesigner.Element(XName.Get("Options", edmxNamespace.NamespaceName)).Element(XName.Get("DesignerInfoPropertySet", edmxNamespace.NamespaceName)).Elements(XName.Get("DesignerProperty", edmxNamespace.NamespaceName)).Select(e => new DesignerProperty { Name = e.Attribute("Name").Value, Value = e.Attribute("Value").Value }), 
                EDMXDesignerDiagrams = edmxDesigner.Element(XName.Get("Diagrams", edmxNamespace.NamespaceName)).Elements(XName.Get("Diagram", edmxNamespace.NamespaceName))
            };

            readMoreAction(edmx);
            return edm;
        }
    }
}
