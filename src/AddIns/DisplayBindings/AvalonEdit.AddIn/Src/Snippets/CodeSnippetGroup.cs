// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// A group of snippets (for a specific file extension).
	/// </summary>
	public class CodeSnippetGroup : INotifyPropertyChanged
	{
		string extensions = "";
		ObservableCollection<CodeSnippet> snippets = new ObservableCollection<CodeSnippet>();
		
		public ObservableCollection<CodeSnippet> Snippets {
			get { return snippets; }
		}
		
		public string Extensions {
			get { return extensions; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				if (extensions != value) {
					extensions = value;
					OnPropertyChanged("Extensions");
				}
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
