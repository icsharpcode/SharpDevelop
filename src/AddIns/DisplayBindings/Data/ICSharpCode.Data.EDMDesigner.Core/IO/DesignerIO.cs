#region Usings

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class DesignerIO
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
            var designerView = new DesignerView { Name = designerViewXElement.Attribute("Name").Value, Zoom = int.Parse(designerViewXElement.Attribute("Zoom").Value) };
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
    }
}
