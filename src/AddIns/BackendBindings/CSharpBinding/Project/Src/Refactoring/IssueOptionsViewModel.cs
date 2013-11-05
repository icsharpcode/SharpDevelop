// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
