// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for IntroduceMethodDialog.xaml
	/// </summary>
	public partial class IntroduceMethodDialog : Window
	{
		public object Result { get; private set; }
		public bool IsNew { get; private set; }
		
		public IntroduceMethodDialog(IClass callingClass)
		{
			InitializeComponent();

			var classes = GetAllStaticClasses(callingClass.ProjectContent.Project as IProject);
			
			if (!classes.Any())
				rbNew.IsChecked = true;
			else {
				classList.ItemsSource = classes;
				classList.SelectedItem = classes.FirstOrDefault(c => c.Name.Contains("Extensions")) ?? classes.First();
			}
		}
		
		IEnumerable<IClass> GetAllStaticClasses(IProject project)
		{
			IProjectContent projectContent = ParserService.GetProjectContent(project);
			if (projectContent != null) {
				foreach (IClass c in projectContent.Classes) {
					if (c.IsStatic)
						yield return c;
				}
			}
		}
		
		void OKButtonClick(object sender, RoutedEventArgs e)
		{
			IsNew = rbNew.IsChecked == true;
			
			if (rbExisting.IsChecked == true)
				Result = classList.SelectedItem;
			
			if (IsNew)
				Result = newClassName.Text;

			DialogResult = true;
			Close();
		}
		
		void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			// do nothing
			DialogResult = false;
			Close();
		}
	}
}
