// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;
namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Display Height/Width on the primary selection
	/// </summary>
    [ExtensionFor(typeof(UIElement))]
    class SizeDisplayExtension : PrimarySelectionAdornerProvider
    {
        HeightDisplay heightDisplay;
        WidthDisplay widthDisplay;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this.ExtendedItem != null)
            {
                RelativePlacement placementHeight = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Stretch);
                placementHeight.XOffset = 10;
                heightDisplay = new HeightDisplay();
                heightDisplay.DataContext = this.ExtendedItem.Component;

                RelativePlacement placementWidth = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Bottom);
                placementWidth.YOffset = 10;
                widthDisplay = new WidthDisplay();
                widthDisplay.DataContext = this.ExtendedItem.Component;

                this.AddAdorners(placementHeight, heightDisplay);
                this.AddAdorners(placementWidth, widthDisplay);         
            }
        }   
    }
}
