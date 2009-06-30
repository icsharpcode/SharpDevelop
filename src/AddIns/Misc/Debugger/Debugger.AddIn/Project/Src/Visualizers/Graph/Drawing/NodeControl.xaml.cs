// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Graph.Layout;
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
	/// UserControl used to display Positione.
	/// </summary>
	public partial class NodeControl : UserControl
	{
		/// <summary>
		/// Creates new NodeControl displaying PositionedNode.
		/// </summary>
		/*public NodeControl() : this()
		{
			//this.GraphNode = graphNode;
		}*/
		
		/// <summary>
		/// Creates new NodeControl displaying PositionedNode.
		/// </summary>
		public NodeControl()
		{
			InitializeComponent();
		}
		
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;

		private PositionedGraphNode node;
		/// <summary>
		/// ObjectNode that this control displays.
		/// </summary>
		public PositionedGraphNode GraphNode
		{
			get
			{
				return node;
			}
			private set
			{
				this.node = value;
			}
		}
		
		public void AddProperty(PositionedNodeProperty property)
		{
			int nRow = propertyGrid.RowDefinitions.Count;
			
			var row = new RowDefinition();
			propertyGrid.RowDefinitions.Add(row);
			
			if (!property.IsAtomic && !property.IsNull)
			{
				Button btnExpandCollapse = new Button();
				btnExpandCollapse.Tag = property;
				btnExpandCollapse.Content = property.IsExpanded ? "-" : "+";
				btnExpandCollapse.Width = 20;
				propertyGrid.Children.Add(btnExpandCollapse);
				Grid.SetRow(btnExpandCollapse, nRow);
				Grid.SetColumn(btnExpandCollapse, 0);
				btnExpandCollapse.Click += new RoutedEventHandler(btnExpandCollapse_Click);
			}

			TextBlock txtName = createTextBlock(property.Name);
			propertyGrid.Children.Add(txtName);
			Grid.SetRow(txtName, nRow);
			Grid.SetColumn(txtName, 1);

			TextBlock txtValue = createTextBlock(property.Value);
			propertyGrid.Children.Add(txtValue);
			Grid.SetRow(txtValue, nRow);
			Grid.SetColumn(txtValue, 2);
		}
		
		/*public void Measure()
		{
			this.Measure();
			
			int nRow = 0;
			// dynamically create TextBlocks and insert them to the 2-column propertyGrid
			foreach (var property in node.Properties)
			{
				

				nRow++;
			}
		}*/

		void btnExpandCollapse_Click(object sender, RoutedEventArgs e)
		{
			Button buttonClicked = ((Button)sender);
			var property = (PositionedNodeProperty)buttonClicked.Tag;
			
			property.IsExpanded = !property.IsExpanded;
			buttonClicked.Content = property.IsExpanded ? "-" : "+";
			if (property.IsExpanded)
			{
				OnPropertyExpanded(property);
			}
			else
			{
				OnPropertyCollapsed(property);
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
		
		#region event helpers
		protected virtual void OnPropertyExpanded(PositionedNodeProperty property)
		{
			if (this.PropertyExpanded != null)
			{
				this.PropertyExpanded(this, new PositionedPropertyEventArgs(property));
			}
		}

		protected virtual void OnPropertyCollapsed(PositionedNodeProperty property)
		{
			if (this.PropertyCollapsed != null)
			{
				this.PropertyCollapsed(this, new PositionedPropertyEventArgs(property));
			}
		}
		#endregion
	}
}
