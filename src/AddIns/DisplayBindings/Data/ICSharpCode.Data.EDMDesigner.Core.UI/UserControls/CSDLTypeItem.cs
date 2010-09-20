// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class CSDLTypeItem : TextBlock
    {
        static CSDLTypeItem()
        {
            ResourceDictionaryLoader.LoadResourceDictionary("/UserControls/CSDLTypeResourceDictionary.xaml");
        }

        public IUIType UIType
        {
            get { return (IUIType)GetValue(UITypeProperty); }
            set { SetValue(UITypeProperty, value); }
        }

        public static readonly DependencyProperty UITypeProperty =
            DependencyProperty.Register("UIType", typeof(IUIType), typeof(CSDLTypeItem), new UIPropertyMetadata(null, 
                (sender, e) =>
                {
                    var entityType = e.NewValue as UIEntityType;
                    if (entityType != null)
                        ((CSDLTypeItem)sender).SetBinding(OpacityProperty, new Binding("IsCompletlyMapped") { Source = (entityType.BusinessInstance).Mapping, Converter = new BoolToOpacityConverter(), ConverterParameter = 0.5 });
                })); 

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(this, UIType, DragDropEffects.Copy);
        }
    }
}
