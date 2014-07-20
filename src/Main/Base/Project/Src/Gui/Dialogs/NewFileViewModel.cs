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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Templates;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class NewFileViewModel : ViewModelBase
    {
        public NewFileViewModel()
        {
            AllTemplate = new ObservableCollection<NewFileTemplateItem>();
            Templates = new ObservableCollection<NewFileCategory>()
            {
                new NewFileCategory("Empty file")
            };
            Icons= new Dictionary<IImage, int>();
            Categories = new ObservableCollection<NewFileCategory>();
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

        public FileTemplateResult Result { get; set; }

        public string SearchFor { get; set; }
        public string Description { get; set; }
        public ObservableCollection<NewFileTemplateItem> AllTemplate { get; set; }

		public Dictionary<IImage, int> Icons {
			get;
			set;
		}

        public ObservableCollection<NewFileCategory> Categories { get; set; }

        public ObservableCollection<NewFileCategory> Templates { get; set; }
    }
}

