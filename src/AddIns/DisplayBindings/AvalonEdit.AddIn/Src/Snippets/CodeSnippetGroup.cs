// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// A group of snippets (for a specific file extension).
	/// </summary>
	public class CodeSnippetGroup : AbstractFreezable, INotifyPropertyChanged
	{
		public CodeSnippetGroup()
		{
		}
		
		public CodeSnippetGroup(CodeSnippetGroup g)
		{
			this.Extensions = g.Extensions;
			this.Snippets.AddRange(g.Snippets.Select(s => new CodeSnippet(s)));
		}
		
		string extensions = "";
		ObservableCollection<CodeSnippet> snippets = new ObservableCollection<CodeSnippet>();
		
		public ObservableCollection<CodeSnippet> Snippets {
			get { return snippets; }
		}
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			foreach (var snippet in this.snippets)
				snippet.Freeze();
			this.snippets.CollectionChanged += delegate { throw new NotSupportedException(); };
		}
		
		public string Extensions {
			get { return extensions; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				CheckBeforeMutation();
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
