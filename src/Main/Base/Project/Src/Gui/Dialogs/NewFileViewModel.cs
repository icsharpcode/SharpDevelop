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
using System.Runtime.CompilerServices;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Templates;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class NewFileViewModel : ViewModelBase
    {
        #region Backing properties
        private NewFileCategory _selectedCategory;
        private string _searchFor;
        private ObservableCollection<NewFileTemplate> _allTemplate;
        private NewFileTemplate _selectedItem;
        private string _fileName;

        #endregion

        public NewFileViewModel()
        {
            _allTemplate = new ObservableCollection<NewFileTemplate>();
           
            Categories = new ObservableCollection<NewFileCategory>();

            AllFileTemplates = new NewFileCategory("All");
            Categories.Add(AllFileTemplates);
        }

        public NewFileCategory AllFileTemplates { get; set; }

        public NewFileCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        
        public ObservableCollection<NewFileTemplate> SelectedTemplates
        {
            get
            {
                UpdatedSelection();
                return _allTemplate;
            }
        }

        public void UpdatedSelection()
        {
            _allTemplate.Clear();
            var selectedCategory = _selectedCategory;
            if (selectedCategory == null) return;
            var searchTokens = SearchFor.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in selectedCategory.Templates)
            {
                if(PassFilter(item, searchTokens))
                    _allTemplate.Add(item);
            }
        }

        static bool PassFilter(NewFileTemplate template, string[] searchTokens)
        {
            if (searchTokens.Length == 0)
                return true;
            
            var result = TextMatchesSearchTokens(searchTokens, template.DisplayName);
            var resultDescription = TextMatchesSearchTokens(searchTokens, template.Description);
            var resultCategory = TextMatchesSearchTokens(searchTokens, template.CategoryDescription);

            return result ||resultDescription || resultCategory;
        }

        static bool TextMatchesSearchTokens(string[] searchTokens, string textToSearch)
        {
            foreach (var searchToken in searchTokens)
            {
                if (textToSearch.IndexOf(searchToken, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public NewFileTemplate SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged("Description");
                FileName = GenerateCurrentFileName();
            }
        }

        public IProject Project
        {
            get;
            set;
        }

        public DirectoryName BasePath
        {
            get;
            set;
        }

        public bool AllowUntitledFiles
        {
            get;
            set;
        }
        
		string GenerateCurrentFileName()
		{
			if(SelectedItem==null)
				return string.Empty;
			return SelectedItem.Template.SuggestFileName(BasePath);
		}

        internal static string GenerateValidClassOrNamespaceName(string className, bool allowDot)
        {
            if (className == null)
                throw new ArgumentNullException("className");
            className = className.Trim();
            if (className.Length == 0)
                return string.Empty;
            var nameBuilder = new StringBuilder();
            if (className[0] != '_' && !char.IsLetter(className, 0))
                nameBuilder.Append('_');
            for (int idx = 0; idx < className.Length; ++idx)
            {
                if (Char.IsLetterOrDigit(className[idx]) || className[idx] == '_')
                {
                    nameBuilder.Append(className[idx]);
                }
                else if (className[idx] == '.' && allowDot)
                {
                    nameBuilder.Append('.');
                }
                else
                {
                    nameBuilder.Append('_');
                }
            }
            return nameBuilder.ToString();
        }
        
		internal void InitializeTemplates(IEnumerable<TemplateCategory> templateCategories)
		{
			var categories = Categories;
			foreach (var templateCategory in Sorted(templateCategories)) {
				var cat = CreateCategory(templateCategory);
				categories.Add(cat);
			}
		    SelectedCategory = AllFileTemplates;
		}

        IEnumerable<TemplateCategory> Sorted(IEnumerable<TemplateCategory> templateCategories)
        {
            return templateCategories
            	.OrderByDescending(c => c.SortOrder)
            	.ThenBy(c => StringParser.Parse(c.DisplayName));
        }

        NewFileCategory CreateCategory(TemplateCategory templateCategory)
        {
            var category = new NewFileCategory(templateCategory.DisplayName);
            foreach (var subcategory in Sorted(templateCategory.Subcategories))
            {
                var subnode = CreateCategory(subcategory);
                category.Children.Add(subnode);
            }
            foreach (var template in templateCategory.Templates.OfType<FileTemplate>())
            {
                if (!template.IsVisible(Project))
                    continue;
                var titem = new NewFileTemplate(template, category.DisplayName);
                AllFileTemplates.Templates.Add(titem);
                category.Templates.Add(titem);
            }
            return category;
        }
        
        internal FileTemplateResult BuildSelectedResult()
        {
            var selectedTemplate = SelectedItem;
            var fileName = FileName;

            var standardNamespace = "DefaultNamespace";

            var localizedTypeDescriptor = selectedTemplate.Template.CreateCustomizationObject();
            var options = new FileTemplateOptions
            {
                ClassName =
                    GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(FileName), false),
                FileName = Core.FileName.Create(fileName),
                IsUntitled = AllowUntitledFiles,
                Namespace = standardNamespace,
                CustomizationObject = localizedTypeDescriptor,
                Project = Project
            };

            var result = selectedTemplate.Template.Create(options);
            if (result != null)
                selectedTemplate.Template.RunActions(result);
            return result;
        }
        public FileTemplateResult Result { get;set; }

        public string SearchFor
        {
            get
            {
                if (string.IsNullOrEmpty(_searchFor))
                    _searchFor = string.Empty;
                return _searchFor;
            }
            set
            {
                _searchFor = value; 
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value; 
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return SelectedItem == null
                    ? string.Empty
                    : SelectedItem.Description;

            }
            set { }
        }

        public ObservableCollection<NewFileCategory> Categories { get; set; }

    }
}

