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
	public partial class NewFileWindow : Window
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
				
		    InitializeTemplates(templateCategories, ViewModel.Categories);
		}
		
		void InitializeTemplates(IEnumerable<TemplateCategory> templateCategories, ObservableCollection<NewFileCategory> categories)
		{
			foreach (var templateCategory in Sorted(templateCategories)) {
				var cat = CreateCategory(templateCategory);
				categories.Add(cat);
			}
		}

        IEnumerable<TemplateCategory> Sorted(IEnumerable<TemplateCategory> templateCategories)
        {
            return templateCategories.OrderByDescending(c => c.SortOrder).ThenBy(c => StringParser.Parse(c.DisplayName));
        }

        NewFileCategory CreateCategory(TemplateCategory templateCategory)
        {
            var node = new NewFileCategory(templateCategory.DisplayName);
            foreach (var subcategory in Sorted(templateCategory.Subcategories))
            {
                var subnode = CreateCategory(subcategory);
                node.Children.Add(subnode);
            }
            foreach (var template in templateCategory.Templates.OfType<FileTemplate>())
            {
                if (!template.IsVisible(ViewModel.Project))
                    continue;
                var titem = new NewFileTemplateItem(template);
                if (titem.Template.Icon != null)
                {
                    ViewModel.Icons[titem.Template.Icon] = 0; // "create template icon"
                }
                ViewModel.AllFileTemplates.Templates.Add(titem);
                ViewModel.SelectedTemplates.Add(titem);
                node.Templates.Add(titem);
            }
            return node;
        }
	    private void okButtonClick(object sender, RoutedEventArgs e)
        {

            var selectedTemplate = ViewModel.SelectedItem;
            var fileName = ViewModel.FileName;

            var standardNamespace = "DefaultNamespace";

            var localizedTypeDescriptor = selectedTemplate.Template.CreateCustomizationObject();
            var options = new FileTemplateOptions
            {
                ClassName =
                    GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(ViewModel.FileName), false),
                FileName = FileName.Create(fileName),
                IsUntitled = ViewModel.AllowUntitledFiles,
                Namespace = standardNamespace,
                CustomizationObject = localizedTypeDescriptor,
                Project = ViewModel.Project
            };

	        var result = selectedTemplate.Template.Create(options);
            DialogResult = true;
            if (result != null)
                selectedTemplate.Template.RunActions(result);
            Close();
	    }

	    private void cancelButtonClick(object sender, RoutedEventArgs e)
	    {
            Close();
	    }
	    
	    internal static string GenerateValidClassOrNamespaceName(string className, bool allowDot)
		{
			if (className == null)
				throw new ArgumentNullException("className");
			className = className.Trim();
			if (className.Length == 0)
				return string.Empty;
			StringBuilder nameBuilder = new StringBuilder();
			if (className[0] != '_' && !char.IsLetter(className, 0))
				nameBuilder.Append('_');
			for (int idx = 0; idx < className.Length; ++idx) {
				if (Char.IsLetterOrDigit(className[idx]) || className[idx] == '_') {
					nameBuilder.Append(className[idx]);
				} else if (className[idx] == '.' && allowDot) {
					nameBuilder.Append('.');
				} else {
					nameBuilder.Append('_');
				}
            }
            return nameBuilder.ToString();
        }
			
		void CategoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedCategory = (NewFileCategory)categoryTreeView.SelectedItem;
            ViewModel.UpdatedSelection();
		}
	}
}