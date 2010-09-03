// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.AddIn.ImageSourceEditor
{
    /// <summary>
    /// Editor to edit properties of type of ImageSource such as Windows.Icon or Image.Source
    /// </summary>
    [TypeEditor(typeof (ImageSource))]
    public partial class ImageSourceEditor 
    {
        public ImageSourceEditor()
        {
            InitializeComponent();
        }
        
		 private void ChooseImageClick(object sender, RoutedEventArgs e)
        {
            ChooseImageDialog cid = new ChooseImageDialog(PropertyNode);
            cid.ShowActivated = true;
            cid.Show();
        }
		 
		 /// <summary>
		 /// Gets the property node that the editor is editing. 
		 /// </summary>
        public PropertyNode PropertyNode{
            get { return DataContext as PropertyNode; }
        }
    }
}
