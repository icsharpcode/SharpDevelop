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
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.SharpDevelop.Widgets;

namespace CSharpBinding.Refactoring
{
	sealed class IssueOptionsViewModel : ViewModelBase
	{
		internal readonly IssueManager.IssueProvider Provider;
		readonly IssueDescriptionAttribute attribute;
		
		internal IssueOptionsViewModel(IssueManager.IssueProvider provider)
		{
			this.Provider = provider;
			this.attribute = provider.Attribute;
			this.Severity = attribute.Severity;
		}
		
		// TODO: Translate
		public string Title {
			get { return attribute.Title; }
		}
		
		public string Description {
			get { return attribute.Description; }
		}
		
		public string Category {
			get { return attribute.Category; }
		}
		
		Severity severity;
		
		public Severity Severity {
			get { return severity; }
			set { SetAndNotifyPropertyChanged(ref severity, value); }
		}
	}
}
