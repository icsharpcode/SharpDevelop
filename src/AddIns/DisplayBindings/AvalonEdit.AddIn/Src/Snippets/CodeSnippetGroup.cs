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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;

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
				FreezableHelper.ThrowIfFrozen(this);
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
