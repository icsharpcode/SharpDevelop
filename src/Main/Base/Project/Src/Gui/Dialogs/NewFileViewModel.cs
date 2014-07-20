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
using System.Linq;
using System.Runtime.CompilerServices;
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
        private ObservableCollection<NewFileTemplateItem> _allTemplate;
        private NewFileTemplateItem _selectedItem;

        #endregion

        public NewFileViewModel()
        {
            _allTemplate = new ObservableCollection<NewFileTemplateItem>();
           
            Icons = new Dictionary<IImage, int>();
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

        
        public ObservableCollection<NewFileTemplateItem> SelectedTemplates
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
            if (selectedCategory != null)
            {
                foreach (var item in selectedCategory.Templates)
                {
                    _allTemplate.Add(item);
                }
            }
        }

        public NewFileTemplateItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged("Description");
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

        public FileTemplateResult Result { get;set; }

        public string SearchFor
        {
            get { return _searchFor; }
            set { _searchFor = value; }
        }

        public string Description
        {
            get
            {
                return SelectedItem == null
                    ? string.Empty
                    : SelectedItem.Template.Description;

            }
            set { }
        }


        public Dictionary<IImage, int> Icons
        {
            get;
            set;
        }

        public ObservableCollection<NewFileCategory> Categories { get; set; }

        public ObservableCollection<NewFileCategory> Templates { get; set; }
    }
}

