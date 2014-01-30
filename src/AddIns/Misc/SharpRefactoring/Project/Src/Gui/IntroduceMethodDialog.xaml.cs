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
