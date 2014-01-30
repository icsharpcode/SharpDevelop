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
