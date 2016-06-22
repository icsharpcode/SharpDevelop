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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for AssemblyInfo.xaml
	/// </summary>
	public partial class AssemblyInfoPanel
	{
		private AssemblyInfo assemblyInfo;

		public AssemblyInfoPanel()
		{
			InitializeComponent();
		}

		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			var assemblyInfoFileName = GetAssemblyInfoFileName(project);

			if (string.IsNullOrEmpty(assemblyInfoFileName))
			{
				assemblyInfo = new AssemblyInfo();
				MessageService.ShowError("${res:Dialog.ProjectOptions.AssemblyInfo.AssemblyInfoNotFound}");
			}
			else
			{
				var assemblyInfoProvider = new AssemblyInfoProvider();
				assemblyInfo = assemblyInfoProvider.ReadAssemblyInfo(assemblyInfoFileName);
			}

			var assemblyInfoViewModel = new AssemblyInfoViewModel(assemblyInfo);
			assemblyInfoViewModel.PropertyChanged += OnAssemblyInfoChanged;
			DataContext = assemblyInfoViewModel;

			base.Load(project, configuration, platform);
		}

		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if (!CheckForValidationErrors())
			{
				return false;
			}

			var assemblyInfoFileName = GetAssemblyInfoFileName(project);
			if (!string.IsNullOrEmpty(assemblyInfoFileName))
			{
				if (assemblyInfo != null)
				{
					var assemblyInfoProvider = new AssemblyInfoProvider();
					assemblyInfoProvider.MergeAssemblyInfo(assemblyInfo, assemblyInfoFileName);
				}
			}
			else
			{
				MessageService.ShowError("${res:Dialog.ProjectOptions.AssemblyInfo.AssemblyInfoNotFound}");
			}

			return base.Save(project, configuration, platform);
		}

		private string GetAssemblyInfoFileName(MSBuildBasedProject project)
		{
			var assemblyInfoProjectItem = project.Items
				.FirstOrDefault(projectItem => projectItem.FileName.GetFileNameWithoutExtension() == "AssemblyInfo");

			if (assemblyInfoProjectItem == null || assemblyInfoProjectItem.FileName == null)
				return null;

			return assemblyInfoProjectItem.FileName.ToString();
		}

		private void OnAssemblyInfoChanged(object sender, EventArgs e)
		{
			this.IsDirty = true;
		}

		private bool CheckForValidationErrors()
		{
			var wrongControl = RootGrid.Children.OfType<UIElement>().FirstOrDefault(Validation.GetHasError);
			if (wrongControl != null)
			{
				MessageService.ShowError("${res:Dialog.ProjectOptions.AssemblyInfo.IncorrectValue}");
				wrongControl.Focus();

				var textBox = wrongControl as TextBox;
				if (textBox != null) 
					textBox.SelectAll();

				return false;
			}

			return true;
		}
	}
}
