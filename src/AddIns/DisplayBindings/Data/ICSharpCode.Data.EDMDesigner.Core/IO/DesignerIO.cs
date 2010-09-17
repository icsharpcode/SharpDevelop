#region Usings

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class DesignerIO : IO
    {
        public static void Read(EDMView edmView, XElement designerViewsXElement, Func<UIEntityType, ITypeDesigner> createEntityDesignerFromUIType, Func<UIComplexType, ITypeDesigner> createComplexDesignerFromUIType)
        {
            var designerViewXElements = designerViewsXElement == null ? Enumerable.Empty<XElement>() : designerViewsXElement.Elements("DesignerView");
            if (designerViewXElements.Any())
                foreach (var designerViewXElement in designerViewXElements)
                    edmView.DesignerViews.Add(Read(edmView, createEntityDesignerFromUIType, createComplexDesignerFromUIType, designerViewXElement));
            else
                edmView.DesignerViews.Add(DesignerView.NewView());
        }

        public static DesignerView Read(EDMView edmView, Func<UIEntityType, ITypeDesigner> createEntityDesignerFromUIType, Func<UIComplexType, ITypeDesigner> createComplexDesignerFromUIType, XElement designerViewXElement)
        {
            var designerView = new DesignerView()
                { 
                    Name = designerViewXElement.Attribute("Name").Value, 
                    Zoom = int.Parse(designerViewXElement.Attribute("Zoom").Value)
                };

            var arrange = designerViewXElement.Attribute("Arrange");

            if (arrange != null)
                designerView.ArrangeTypeDesigners =  bool.Parse(arrange.Value);

            foreach (var designerTypeXElement in designerViewXElement.Elements("DesignerType"))
            {
                var name = designerTypeXElement.Attribute("Name").Value;
                
                ITypeDesigner typeDesigner;
                
                var entityType = edmView.CSDL.EntityTypes.FirstOrDefault(et => et.Name == name);
                
                if (entityType != null)
                    typeDesigner = createEntityDesignerFromUIType(entityType);
                else
                {
                    var complexType = edmView.CSDL.ComplexTypes.FirstOrDefault(ct => ct.Name == name);
                    if (complexType == null)
                        continue;
                    typeDesigner = createComplexDesignerFromUIType(complexType);
                }

                var leftAttribute = designerTypeXElement.Attribute("Left").Value;

                if (leftAttribute != null)
                    typeDesigner.Left = double.Parse(leftAttribute, CultureInfo.InvariantCulture);

                var topAttribute = designerTypeXElement.Attribute("Top").Value;

                if (topAttribute != null)
                    typeDesigner.Top = double.Parse(topAttribute, CultureInfo.InvariantCulture);

                var isExpandedAttribute = designerTypeXElement.Attribute("IsExpanded");

                if (isExpandedAttribute != null)
                {
                    RoutedEventHandler loaded = null;
                    loaded = delegate
                    {
                        typeDesigner.IsExpanded = bool.Parse(isExpandedAttribute.Value);
                        typeDesigner.Loaded -= loaded;
                    };
                    typeDesigner.Loaded += loaded;
                }

                designerView.TypeDesignersLocations.Add(typeDesigner);
            }
            return designerView;
        }

        public static XElement GenerateNewDesignerViewsFromCSDLView(CSDLView csdl)
        {
            XElement designerView = new XElement("DesignerView", new XAttribute("Name", "View"), new XAttribute("Zoom", 100), new XAttribute("Arrange", true));

            foreach (UIEntityType entityType in csdl.EntityTypes)
            {
                designerView.Add(new XElement("DesignerType", new XAttribute("Name", entityType.Name), new XAttribute("Top", 0), new XAttribute("Left", 0), new XAttribute("IsExpanded", true)));
            }

            return new XElement("DesignerViews", designerView);
        }

        public static XElement Write(EDM edm)
        {
            if (edm.DesignerProperties == null)
                return null;
            
            XElement connectionElement = new XElement(edmxNamespace + "Connection");
            XElement designerInfoPropertyElement1 = new XElement(edmxNamespace + "DesignerInfoPropertyElement");
            connectionElement.Add(designerInfoPropertyElement1);

            foreach (DesignerProperty designerProperty in edm.DesignerProperties)
            {
                connectionElement.Add(new XElement(edmxNamespace + "DesignerProperty",
                    new XAttribute("Name", designerProperty.Name),
                    new XAttribute("Value", designerProperty.Value)));
            }

            XElement optionsElement = new XElement(edmxNamespace + "Options");
            XElement designerInfoPropertyElement2 = new XElement(edmxNamespace + "DesignerInfoPropertyElement");
            optionsElement.Add(designerInfoPropertyElement2);

            foreach (DesignerProperty designerProperty in edm.DesignerProperties)
            {
                optionsElement.Add(new XElement(edmxNamespace + "DesignerProperty",
                   new XAttribute("Name", designerProperty.Name),
                   new XAttribute("Value", designerProperty.Value)));
            }

            return new XElement(edmxNamespace + "Designer")
                .AddElement(connectionElement)
                .AddElement(optionsElement)
                .AddElement(new XElement(edmxNamespace + "Diagrams", edm.EDMXDesignerDiagrams));
        }
    }
}
