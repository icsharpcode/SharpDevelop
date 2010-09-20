// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher;
using System.Collections.Generic;

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
            EDMDesignerChangeWatcher.Init = true;

            var designerView = new DesignerView()
                { 
                    Name = designerViewXElement.Attribute("Name").Value, 
                    Zoom = int.Parse(designerViewXElement.Attribute("Zoom").Value)
                };

            var arrange = designerViewXElement.Attribute("Arrange");

            if (arrange != null)
                designerView.ArrangeTypeDesigners =  bool.Parse(arrange.Value);

            IEnumerable<XElement> designerTypeXElements = designerViewXElement.Elements("DesignerType");

            foreach (var designerTypeXElement in designerTypeXElements)
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
                        EDMDesignerChangeWatcher.Init = true;
                        typeDesigner.IsExpanded = bool.Parse(isExpandedAttribute.Value);
                        typeDesigner.Loaded -= loaded;
                        EDMDesignerChangeWatcher.Init = false;
                    };
                    typeDesigner.Loaded += loaded;
                }

                designerView.TypeDesignersLocations.Add(typeDesigner);
            }

            EDMDesignerChangeWatcher.Init = false;

            return designerView;
        }

        public static XElement GenerateNewDesignerViewsFromCSDLView(EDMView edmView)
        {
            XElement designerView = new XElement("DesignerView", new XAttribute("Name", "View"), new XAttribute("Zoom", 100), new XAttribute("Arrange", true));

            foreach (UIEntityType entityType in edmView.CSDL.EntityTypes)
            {
                designerView.Add(new XElement("DesignerType", new XAttribute("Name", entityType.Name), new XAttribute("Top", 0), new XAttribute("Left", 0), new XAttribute("IsExpanded", true)));
            }

            edmView.EDM.EDMXDesignerDiagrams = designerView.Elements();

            return new XElement("DesignerViews", designerView);
        }

        public static XElement Write(EDMView edmView)
        {
            XElement connectionElement = null;
            XElement optionsElement = null;

            if (edmView.EDM.DesignerProperties == null || edmView.EDM.DesignerProperties.FirstOrDefault(dp => dp.Name == "MetadataArtifactProcessing") == null)
            {
                List<DesignerProperty> standardDesignerProperties = null;

                if (edmView.EDM.DesignerProperties == null)
                    standardDesignerProperties = new List<DesignerProperty>();
                else
                    standardDesignerProperties = edmView.EDM.DesignerProperties.ToList();

                standardDesignerProperties.Add(new DesignerProperty() { Name = "MetadataArtifactProcessing", Value = "EmbedInOutputAssembly" });

                edmView.EDM.DesignerProperties = standardDesignerProperties;
            }

            connectionElement = new XElement(edmxNamespace + "Connection");
            XElement designerInfoPropertyElement1 = new XElement(edmxNamespace + "DesignerInfoPropertySet");
            connectionElement.Add(designerInfoPropertyElement1);

            foreach (DesignerProperty designerProperty in edmView.EDM.DesignerProperties)
            {
                designerInfoPropertyElement1.Add(new XElement(edmxNamespace + "DesignerProperty",
                    new XAttribute("Name", designerProperty.Name),
                    new XAttribute("Value", designerProperty.Value)));
            }

            optionsElement = new XElement(edmxNamespace + "Options");
            XElement designerInfoPropertyElement2 = new XElement(edmxNamespace + "DesignerInfoPropertySet");
            optionsElement.Add(designerInfoPropertyElement2);

            foreach (DesignerProperty designerProperty in edmView.EDM.DesignerProperties)
            {
                designerInfoPropertyElement2.Add(new XElement(edmxNamespace + "DesignerProperty",
                    new XAttribute("Name", designerProperty.Name),
                    new XAttribute("Value", designerProperty.Value)));
            }

            XElement designerElement = new XElement(edmxNamespace + "Designer")
                .AddElement(connectionElement)
                .AddElement(optionsElement);

            //if (edmView.EDM.EDMXDesignerDiagrams != null)
            //    designerElement.AddElement(new XElement(edmxNamespace + "Diagrams", edmView.EDM.EDMXDesignerDiagrams));
            //else
                designerElement.AddElement(new XElement(edmxNamespace + "Diagrams", Write(edmView.DesignerViews)));

            return designerElement;
        }

        private static XElement Write(EventedObservableCollection<DesignerView> designerViews)
        {
            XElement designerViewsElement = new XElement("DesignerViews");

            foreach (DesignerView designerView in designerViews)
                designerViewsElement.Add(Write(designerView));

            return designerViewsElement;
        }

        private static XElement Write(DesignerView designerView)
        {
            XElement designerViewElement = new XElement("DesignerView",
                new XAttribute("Name", designerView.Name),
                new XAttribute("Zoom", designerView.Zoom));

            foreach (ITypeDesigner uiType in designerView)
            { 
                designerViewElement.Add(new XElement("DesignerType",
                    new XAttribute("Name", uiType.UIType.Name),
                    new XAttribute("Left", uiType.Left),
                    new XAttribute("Top", uiType.Top),
                    new XAttribute("IsExpanded", uiType.IsExpanded)));
            }

            return designerViewElement;
        }
    }
}
