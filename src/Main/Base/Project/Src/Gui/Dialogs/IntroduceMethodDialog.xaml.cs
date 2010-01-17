// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Interaction logic for IntroduceMethodDialog.xaml
	/// </summary>
	public partial class IntroduceMethodDialog : Window
	{
		public IClass CallingClass { get; private set; }
		
		ITextEditor editor;
		MethodDeclaration method;
		
		public IntroduceMethodDialog(IClass callingClass, MethodDeclaration method, ITextEditor editor)
		{
			InitializeComponent();
			
			CallingClass = callingClass;
			this.editor = editor;
			this.method = method;
			
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
			CodeGenerator gen = CallingClass.ProjectContent.Language.CodeGenerator;
			
			if (rbExisting.IsChecked == true) {
				IClass c = (IClass)classList.SelectedItem;
				gen.InsertCodeAtEnd(c.BodyRegion, new RefactoringDocumentAdapter(editor.Document), method);
			}
			if (rbNew.IsChecked == true) {
				TypeDeclaration type = new TypeDeclaration(Modifiers.Static, null);
				
				type.Name = newClassName.Text;
				
				type.AddChild(method);
				
				gen.InsertCodeAfter(CallingClass, new RefactoringDocumentAdapter(editor.Document), type);
			}
			Close();
		}
		
		void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			// do nothing
			Close();
		}
	}
}