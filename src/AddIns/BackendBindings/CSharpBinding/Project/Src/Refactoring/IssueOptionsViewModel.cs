// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop.Widgets;

namespace CSharpBinding.Refactoring
{
	public class IssueOptionsViewModel : ViewModelBase
	{
		readonly Type providerType;
		readonly IssueDescriptionAttribute attribute;
		
		public IssueOptionsViewModel(Type providerType, IssueDescriptionAttribute attribute)
		{
			this.providerType = providerType;
			this.attribute = attribute;
			this.Severity = attribute.Severity;
		}
		
		public Type ProviderType {
			get { return providerType; }
		}
		
		// TODO: Translate
		public string DisplayName {
			get { return attribute.Title; }
		}
		
		public string ToolTip {
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
