// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Allows arranging the rows/column on a grid.
	/// </summary>
	[ExtensionFor(typeof(Grid))]
	[ExtensionServer(typeof(LogicalOrExtensionServer<PrimarySelectionExtensionServer, PrimarySelectionParentExtensionServer>))]
	public class GridAdornerProvider : AdornerProvider
	{
		sealed class RowSplitterPlacement : AdornerPlacement
		{
			readonly RowDefinition row;
			public RowSplitterPlacement(RowDefinition row) { this.row = row; }
			
			public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
			{
				adorner.Arrange(new Rect(-(GridRailAdorner.RailSize + GridRailAdorner.RailDistance),
				                         row.Offset - GridRailAdorner.SplitterWidth / 2,
				                         GridRailAdorner.RailSize + GridRailAdorner.RailDistance + adornedElementSize.Width,
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
				                         -(GridRailAdorner.RailSize + GridRailAdorner.RailDistance),
				                         GridRailAdorner.SplitterWidth,
				                         GridRailAdorner.RailSize + GridRailAdorner.RailDistance + adornedElementSize.Height));
			}
		}
		
		AdornerPanel adornerPanel = new AdornerPanel();
		GridRailAdorner topBar, leftBar;
		
		protected override void OnInitialized()
		{
			leftBar = new GridRailAdorner(this.ExtendedItem, adornerPanel, Orientation.Vertical);
			topBar = new GridRailAdorner(this.ExtendedItem, adornerPanel, Orientation.Horizontal);
			
			RelativePlacement rp = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Stretch);
			rp.XOffset -= GridRailAdorner.RailDistance;
			AdornerPanel.SetPlacement(leftBar, rp);
			rp = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Top);
			rp.YOffset -= GridRailAdorner.RailDistance;
			AdornerPanel.SetPlacement(topBar, rp);
			
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
		/// flag used to ensure that the asynchronus splitter creation is only enqueued once
		/// </summary>
		bool requireSplitterRecreation;
		
		void CreateSplitter()
		{
			if (requireSplitterRecreation) return;
			requireSplitterRecreation = true;
			
			// splitter creation is delayed to prevent unnecessary splitter re-creation when multiple
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
					IList<DesignItem> col = this.ExtendedItem.Properties["RowDefinitions"].CollectionElements;
					for (int i = 1; i < grid.RowDefinitions.Count; i++) {
						RowDefinition row = grid.RowDefinitions[i];
						GridRowSplitterAdorner splitter = new GridRowSplitterAdorner(leftBar, this.ExtendedItem, col[i-1], col[i]);
						AdornerPanel.SetPlacement(splitter, new RowSplitterPlacement(row));
						adornerPanel.Children.Add(splitter);
						splitterList.Add(splitter);
					}
					col = this.ExtendedItem.Properties["ColumnDefinitions"].CollectionElements;
					for (int i = 1; i < grid.ColumnDefinitions.Count; i++) {
						ColumnDefinition column = grid.ColumnDefinitions[i];
						GridColumnSplitterAdorner splitter = new GridColumnSplitterAdorner(topBar, this.ExtendedItem, col[i-1], col[i]);
						AdornerPanel.SetPlacement(splitter, new ColumnSplitterPlacement(column));
						adornerPanel.Children.Add(splitter);
						splitterList.Add(splitter);
					}
				});
		}
	}
}
