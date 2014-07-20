/*
 * Created by SharpDevelop.
 * User: Ciprian Khlud
 * Date: 7/19/2014
 * Time: 10:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
	/// Interaction logic for NewFileWindow.xaml
	/// </summary>
	public partial class NewFileWindow
    {
	    public NewFileViewModel ViewModel 
		{
			get
			{
				return (NewFileViewModel)DataContext;
			}			
		}
		public NewFileWindow()
		{
			InitializeComponent();
		}
		public NewFileWindow(IProject project, DirectoryName basePath, IEnumerable<TemplateCategory> templateCategories)
		{
			DataContext = new NewFileViewModel();
			
			InitializeComponent();
			ViewModel.Project = project;
			ViewModel.BasePath = basePath;
			ViewModel.AllowUntitledFiles = basePath == null;
			if(ViewModel.AllowUntitledFiles)
			{
				categoryTreeView.Focus();
			}
			else
			{
				fileNameTextBox.Focus();
			}
				
		    ViewModel.InitializeTemplates(templateCategories);
		}
	    void okButtonClick(object sender, RoutedEventArgs e)
        {
            var result = ViewModel.BuildSelectedResult();
	        ViewModel.Result = result;

            DialogResult = result!=null;
            Close();
	    }

        void cancelButtonClick(object sender, RoutedEventArgs e)
	    {
            Close();
	    }
	    
		void CategoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedCategory = (NewFileCategory)categoryTreeView.SelectedItem;
            ViewModel.UpdatedSelection();
		}

        private void OnUpdateSearch(object sender, TextChangedEventArgs e)
        {
        	ViewModel.SearchFor = SearchTextBox.Text;
            ViewModel.UpdatedSelection();
        }
    }
}