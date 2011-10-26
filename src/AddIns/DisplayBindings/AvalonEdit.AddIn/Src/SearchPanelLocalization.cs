// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class SearchPanelLocalization : Localization, INotifyPropertyChanged
	{
		static SearchPanelLocalization instance;
		
		public static SearchPanelLocalization Instance {
			get {
				if (instance == null)
					instance = new SearchPanelLocalization();
				return instance;
			}
		}
		
		public SearchPanelLocalization()
		{
			ResourceService.LanguageChanged += delegate { PropertiesChanged(); };
		}
		
		public override string ErrorText {
			get { return StringParser.Parse("${res:AddIns.AvalonEdit.SearchPanel.ErrorText}"); }
		}
		
		public override string FindNextText {
			get { return StringParser.Parse("${res:AddIns.AvalonEdit.SearchPanel.FindNextText}") + " (F3)"; }
		}
		
		public override string FindPreviousText {
			get { return StringParser.Parse("${res:AddIns.AvalonEdit.SearchPanel.FindPreviousText}") + " (Shift+F3)"; }
		}
		
		public override string MatchCaseText {
			get { return MenuService.ConvertLabel(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.MatchCase}")); }
		}
		
		public override string MatchWholeWordsText {
			get { return MenuService.ConvertLabel(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.MatchWholeWord}")); }
		}
		
		public override string NoMatchesFoundText {
			get { return StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStringNotFound}"); }
		}
		
		public override string UseRegexText {
			get { return StringParser.Parse("${res:AddIns.AvalonEdit.SearchPanel.UseRegexText}"); }
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		void PropertiesChanged()
		{
			OnPropertyChanged(null);
		}
	}
}
