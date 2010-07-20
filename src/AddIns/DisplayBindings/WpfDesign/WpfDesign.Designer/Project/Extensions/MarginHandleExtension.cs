// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    public class MarginHandleExtension : AdornerProvider
    {
        private MarginHandle _leftHandle, _topHandle, _rightHandle, _bottomHandle;
        
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this.ExtendedItem.Parent != null)
            {
                if (this.ExtendedItem.Parent.ComponentType == typeof(Grid)){
                    FrameworkElement extendedControl = (FrameworkElement)this.ExtendedItem.Component;
                    AdornerPanel adornerPanel = new AdornerPanel();
                    
                    // If the Element is rotated/skewed in the grid, then margin handles do not appear
                    if (extendedControl.LayoutTransform.Value == Matrix.Identity && extendedControl.RenderTransform.Value == Matrix.Identity)
                    {
                        _leftHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Left);
                        _topHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Top);
                        _rightHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Right);
                        _bottomHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Bottom);
                    }
                    
                    if (adornerPanel != null)
                        this.Adorners.Add(adornerPanel);
                }
            }
        }
        
        public void HideHandles()
        {
        	if (_leftHandle != null && _topHandle != null && _rightHandle != null && _bottomHandle != null) {
        	_leftHandle.Visibility=Visibility.Hidden;
        	_leftHandle.ShouldBeVisible=false;
        	_topHandle.Visibility=Visibility.Hidden;
        	_topHandle.ShouldBeVisible=false;
        	_rightHandle.Visibility=Visibility.Hidden;
        	_rightHandle.ShouldBeVisible=false;
        	_bottomHandle.Visibility=Visibility.Hidden;
        	_bottomHandle.ShouldBeVisible=false;
        	}        	
        }
        
        public void ShowHandles()
        {
        	if (_leftHandle != null && _topHandle != null && _rightHandle != null && _bottomHandle != null){
        	_leftHandle.Visibility=Visibility.Visible;
        	_leftHandle.ShouldBeVisible=true;
        	_leftHandle.DecideVisiblity(_leftHandle.HandleLength);
        	_topHandle.Visibility=Visibility.Visible;
        	_topHandle.ShouldBeVisible=true;
        	_rightHandle.DecideVisiblity(_rightHandle.HandleLength);
        	_rightHandle.Visibility=Visibility.Visible;
        	_rightHandle.ShouldBeVisible=true;
        	_rightHandle.DecideVisiblity(_rightHandle.HandleLength);
        	_bottomHandle.Visibility=Visibility.Visible; 
        	_bottomHandle.ShouldBeVisible=true;
        	_bottomHandle.DecideVisiblity(_bottomHandle.HandleLength);
        	}
        }
    }
}
   
