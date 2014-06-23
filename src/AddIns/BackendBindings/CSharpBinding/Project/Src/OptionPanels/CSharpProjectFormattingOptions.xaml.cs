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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for CSharpProjectFormattingOptionPanel.xaml
	/// </summary>
	internal partial class CSharpProjectFormattingOptionPanel : ProjectOptionPanel
	{
		CSharpFormattingPolicy formattingPolicy;
		
		public CSharpProjectFormattingOptionPanel()
		{
			InitializeComponent();
		}
		
		protected override void Load(ICSharpCode.SharpDevelop.Project.MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			if (formattingPolicy != null) {
				formattingPolicy.OptionsContainer.PropertyChanged -= ContainerPropertyChanged;
			}
			formattingPolicy = CSharpFormattingPolicies.Instance.GetProjectOptions(project);
			formattingEditor.OptionsContainer = formattingPolicy.OptionsContainer;
			formattingEditor.AllowPresets = true;
			formattingEditor.OverrideGlobalIndentation = true;
			formattingPolicy.OptionsContainer.PropertyChanged += ContainerPropertyChanged;
		}
		
		protected override bool Save(ICSharpCode.SharpDevelop.Project.MSBuildBasedProject project, string configuration, string platform)
		{
			bool success = (formattingPolicy != null) && formattingPolicy.Save();
			return base.Save(project, configuration, platform) && success;
		}

		void ContainerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// A formatting option has been changed, mark project settings as dirty
			this.IsDirty = true;
		}
	}
}