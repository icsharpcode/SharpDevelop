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

using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;

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
			SD.ResourceService.LanguageChanged += delegate { PropertiesChanged(); };
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
