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

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public partial class TreeViewOptionsDialog : Window
	{
		public TreeViewOptionsDialog(IEnumerable<IOptionPanelDescriptor> optionPanels)
		{
			if (optionPanels == null)
				throw new ArgumentNullException("optionPanels");
			InitializeComponent();
			
			ICSharpCode.SharpDevelop.Gui.FormLocationHelper.ApplyWindow(this, "TreeViewOptionsDialog.WindowBounds", true);
			
			var list = optionPanels.Select(op => new OptionPanelNode(op, this)).ToList();
			treeView.ItemsSource = list;
			if (list.Count > 0) {
				list[0].IsSelected = true;
			}
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (IOptionPanel op in optionPanels) {
				if (!op.SaveOptions())
					return;
			}
			this.DialogResult = true;
			Close();
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			foreach (IDisposable op in optionPanels.OfType<IDisposable>()) {
				op.Dispose();
			}
		}
		
		List<IOptionPanel> optionPanels = new List<IOptionPanel>();
		OptionPanelNode activeNode;
		
		void SelectNode(OptionPanelNode node)
		{
			while (!node.OptionPanelDescriptor.HasOptionPanel && node.Children.Count > 0)
				node = node.Children[0];
			
			if (node == activeNode)
				return;
			
			while (activeNode != null) {
				activeNode.IsActive = false;
				activeNode.IsExpanded = false;
				activeNode = activeNode.Parent;
			}
			
			activeNode = node;
			optionPanelTitle.Text = node.Title;
			if (node.Content is System.Windows.Forms.Control) {
				optionPanelScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
			} else {
				optionPanelScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			}
			SD.WinForms.SetContent(optionPanelContent, node.Content);
			
			node.IsExpanded = true;
			node.IsActive = true;
		}
		
		sealed class OptionPanelNode : INotifyPropertyChanged
		{
			public readonly IOptionPanelDescriptor OptionPanelDescriptor;
			public readonly OptionPanelNode Parent;
			TreeViewOptionsDialog dialog;
			
			public OptionPanelNode(IOptionPanelDescriptor optionPanel, TreeViewOptionsDialog dialog)
			{
				this.OptionPanelDescriptor = optionPanel;
				this.dialog = dialog;
			}
			
			public OptionPanelNode(IOptionPanelDescriptor optionPanel, OptionPanelNode parent)
			{
				this.OptionPanelDescriptor = optionPanel;
				this.Parent = parent;
				this.dialog = parent.dialog;
			}
			
			public string Title {
				get {
					return OptionPanelDescriptor.Label;
				}
			}
			
			IOptionPanel optionPanel;
			
			public object Content {
				get {
					if (optionPanel == null) {
						optionPanel = OptionPanelDescriptor.OptionPanel;
						if (optionPanel == null) {
							return null;
						}
						optionPanel.LoadOptions();
						dialog.optionPanels.Add(optionPanel);
					}
					return optionPanel.Control;
				}
			}
			
			public ImageSource Image {
				get {
					if (IsActive)
						return PresentationResourceService.GetBitmapSource("Icons.16x16.SelectionArrow");
					if (OptionPanelDescriptor.ChildOptionPanelDescriptors.Any()) {
						if (IsExpanded)
							return PresentationResourceService.GetBitmapSource("Icons.16x16.OpenFolderBitmap");
						else
							return PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap");
					} else {
						return null;
					}
				}
			}
			
			List<OptionPanelNode> children;
			
			public List<OptionPanelNode> Children {
				get {
					if (children == null) {
						children = OptionPanelDescriptor.ChildOptionPanelDescriptors
							.Select(op => new OptionPanelNode(op, this)).ToList();
					}
					return children;
				}
			}
			
			bool isActive;
			
			public bool IsActive {
				get { return isActive; }
				set {
					if (isActive != value) {
						isActive = value;
						OnPropertyChanged("Image");
					}
				}
			}
			
			bool isExpanded;
			
			public bool IsExpanded {
				get { return isExpanded; }
				set {
					if (isExpanded != value) {
						isExpanded = value;
						OnPropertyChanged("IsExpanded");
						OnPropertyChanged("Image");
					}
					
					if (isExpanded && Parent != null)
						Parent.IsExpanded = true;
				}
			}
			
			bool isSelected;
			
			public bool IsSelected {
				get { return isSelected; }
				set {
					try {
						if (isSelected != value) {
							isSelected = value;
							OnPropertyChanged("IsSelected");
						}
						if (isSelected)
							dialog.SelectNode(this);
					} catch (Exception ex) {
						MessageService.ShowException(ex);
					}
				}
			}
			
			public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
			
			void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
			
			public override string ToString()
			{
				// used for keyboard navigation and screenreaders
				return this.Title;
			}
		}
	}
}
