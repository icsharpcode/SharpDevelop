// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Debugger.AddIn.Visualizers.Graph;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
    /// <summary>
    /// UserControl used to display ObjectNode.
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public NodeControl()
        {
            InitializeComponent();
        }

        private ObjectNode node;
        /// <summary>
        /// ObjectNode that this control displays.
        /// </summary>
        public ObjectNode GraphNode 
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
                int row = 0;
                // dynamically create TextBlocks and insert them to the 2-column propertyGrid
                foreach (var property in node.Properties)
                {
                    propertyGrid.RowDefinitions.Add(new RowDefinition());

                    TextBlock txtName = createTextBlock(property.Name);
                    propertyGrid.Children.Add(txtName);
                    Grid.SetRow(txtName, row);
                    Grid.SetColumn(txtName, 0);

                    TextBlock txtValue = createTextBlock(property.Value);
                    propertyGrid.Children.Add(txtValue);
                    Grid.SetRow(txtValue, row);
                    Grid.SetColumn(txtValue, 1);

                    row++;
                }
            }
        }
        
        /// <summary>
        /// Creates TextBlock with given text.
        /// </summary>
        private TextBlock createTextBlock(string text)
        {
        	TextBlock newTextblock = new TextBlock();
        	newTextblock.Text = text;
        	newTextblock.Padding = new Thickness(4);
        	return newTextblock; 
        }
    }
}
