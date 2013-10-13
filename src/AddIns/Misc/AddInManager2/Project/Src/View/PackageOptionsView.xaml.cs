/*
 * Created by SharpDevelop.
 * User: WheizWork
 * Date: 13.10.2013
 * Time: 23:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.AddInManager2.ViewModel;

namespace ICSharpCode.AddInManager2.View
{
	/// <summary>
	/// Interaction logic for PackageOptionsView.xaml
	/// </summary>
	public partial class PackageOptionsView : OptionPanel
	{
		private PackageOptionsViewModel viewModel;
		
		public PackageOptionsView()
		{
			InitializeComponent();
		}
		
		private PackageOptionsViewModel ViewModel
		{
			get
			{
				if (viewModel == null)
				{
					viewModel = DataContext as PackageOptionsViewModel;
				}
				return viewModel;
			}
		}
		
		public override void LoadOptions()
		{
			ViewModel.Load();
		}
		
		public override bool SaveOptions()
		{
			ViewModel.Save();
			return true;
		}
	}
}