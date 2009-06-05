using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace SearchAndReplace
{
	/// <summary>
	/// Interaction logic for ResultsTreeView.xaml
	/// </summary>
	public partial class ResultsTreeView : TreeView
	{
		public ResultsTreeView()
		{
			InitializeComponent();
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Return) {
				SearchNode node = this.SelectedItem as SearchNode;
				if (node != null)
					node.ActivateItem();
				e.Handled = true;
			}
			base.OnKeyDown(e);
		}
		
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			SearchNode node = this.SelectedItem as SearchNode;
			if (node != null)
				node.ActivateItem();
			e.Handled = true;
		}
	}
}
