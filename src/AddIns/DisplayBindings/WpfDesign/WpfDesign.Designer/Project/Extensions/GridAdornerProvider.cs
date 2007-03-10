// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Allows arranging the rows/column on a grid.
	/// </summary>
	[ExtensionFor(typeof(Grid))]
	public class GridAdornerProvider : PrimarySelectionAdornerProvider
	{
		sealed class RowSplitterPlacement : AdornerPlacement
		{
			readonly RowDefinition row;
			public RowSplitterPlacement(RowDefinition row) { this.row = row; }
			
			public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
			{
				adorner.Arrange(new Rect(-GridRailAdorner.RailSize,
				                         row.Offset - GridRailAdorner.SplitterWidth / 2,
				                         GridRailAdorner.RailSize + adornedElementSize.Width,
				                         GridRailAdorner.SplitterWidth));
			}
		}
		
		sealed class ColumnSplitterPlacement : AdornerPlacement
		{
			readonly ColumnDefinition column;
			public ColumnSplitterPlacement(ColumnDefinition column) { this.column = column; }
			
			public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
			{
				adorner.Arrange(new Rect(column.Offset - GridRailAdorner.SplitterWidth / 2,
				                         -GridRailAdorner.RailSize,
				                         GridRailAdorner.SplitterWidth,
				                         GridRailAdorner.RailSize + adornedElementSize.Height));
			}
		}
		
		AdornerPanel adornerPanel = new AdornerPanel();
		GridRailAdorner topBar, leftBar;
		
		protected override void OnInitialized()
		{
			leftBar = new GridRailAdorner(this.ExtendedItem, adornerPanel, Orientation.Vertical);
			topBar = new GridRailAdorner(this.ExtendedItem, adornerPanel, Orientation.Horizontal);
			
			AdornerPanel.SetPlacement(leftBar, new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Stretch));
			AdornerPanel.SetPlacement(topBar, new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Top));
			
			adornerPanel.Children.Add(leftBar);
			adornerPanel.Children.Add(topBar);
			this.Adorners.Add(adornerPanel);
			
			CreateSplitter();
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			
			base.OnInitialized();
		}
		
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			base.OnRemove();
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "RowDefinitions" || e.PropertyName == "ColumnDefinitions") {
				CreateSplitter();
			}
		}
		
		readonly List<GridSplitterAdorner> splitterList = new List<GridSplitterAdorner>();
		/// <summary>
		/// flag used to unsure that the asynchronus splitter creation is only enqueued once
		/// </summary>
		bool requireSplitterRecreation;
		
		void CreateSplitter()
		{
			if (requireSplitterRecreation) return;
			requireSplitterRecreation = true;
			
			// splitter creation is asynchronous to prevent unnecessary splitter re-creation when multiple
			// changes to the collection are done.
			// It also ensures that the Offset property of new rows/columns is initialized when the splitter
			// is added.
			Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Loaded, // Loaded = after layout, but before input
				(Action)delegate {
					requireSplitterRecreation = false;
					foreach (GridSplitterAdorner splitter in splitterList) {
						adornerPanel.Children.Remove(splitter);
					}
					splitterList.Clear();
					Grid grid = (Grid)this.ExtendedItem.Component;
					for (int i = 1; i < grid.RowDefinitions.Count; i++) {
						RowDefinition row = grid.RowDefinitions[i];
						GridRowSplitterAdorner splitter = new GridRowSplitterAdorner();
						AdornerPanel.SetPlacement(splitter, new RowSplitterPlacement(row));
						adornerPanel.Children.Add(splitter);
						splitterList.Add(splitter);
					}
					for (int i = 1; i < grid.ColumnDefinitions.Count; i++) {
						ColumnDefinition column = grid.ColumnDefinitions[i];
						GridColumnSplitterAdorner splitter = new GridColumnSplitterAdorner();
						AdornerPanel.SetPlacement(splitter, new ColumnSplitterPlacement(column));
						adornerPanel.Children.Add(splitter);
						splitterList.Add(splitter);
					}
				});
		}
	}
}
