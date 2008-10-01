/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 12/22/2006
 * Time: 3:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ClassWizard
{
	public class AddClassForm : BaseSharpDevelopForm
	{
		public AddClassForm()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ClassWizard.Resources.AddClassForm.xfrm"));
			Get<Button>("ok").Click += OkButtonClick;
			Get<RadioButton>("createNewFile").CheckedChanged += FileCreationModeChanged;
			Get<RadioButton>("addToFile").CheckedChanged += FileCreationModeChanged;
			Get<Button>("selectClass").Click += SelectClassButtonClick;
			Get<Button>("selectInterfaces").Click += SelectInterfacesButtonClick;

			ComboBox visibility = Get<ComboBox>("visibility");
			visibility.Items.Add("Public");
			visibility.Items.Add("Private");
			visibility.Items.Add("Protected");
			visibility.Items.Add("Internal");
			visibility.SelectedIndex = 0;
			
			classes.ClassType = ICSharpCode.SharpDevelop.Dom.ClassType.Class;
			classes.SelectMultiple = false;
			
			interfaces.ClassType = ICSharpCode.SharpDevelop.Dom.ClassType.Interface;
			interfaces.SelectMultiple = true;
		}
		
		private ClassesListDialog classes = new ClassesListDialog();
		private ClassesListDialog interfaces = new ClassesListDialog();
		
		protected override void OnShown(EventArgs e)
		{
			IProject proj = ProjectService.CurrentProject;
			IProjectContent projCont = proj.CreateProjectContent();
			classes.Project = proj;
			interfaces.Project = proj;

			ComboBox existingFiles = Get<ComboBox>("existingFiles");
			existingFiles.Items.Clear();
			foreach (ProjectItem projectItem in ProjectService.CurrentProject.Items)
			{
				if (projectItem.ItemType == ItemType.Compile)
					existingFiles.Items.Add(FileUtility.GetRelativePath(proj.Directory, projectItem.FileName));
			}
			
			ComboBox namespaces = Get<ComboBox>("namespace");
			namespaces.Items.Clear();
			
			foreach (string nsn in projCont.NamespaceNames)
				namespaces.Items.Add(nsn);
			
			base.OnShown(e);
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			IProject proj = ProjectService.CurrentProject;

			NamespaceDeclaration domNS = new NamespaceDeclaration(Get<ComboBox>("namespace").Text);
			domNS.AddChild(new UsingDeclaration("System"));
			domNS.AddChild(new UsingDeclaration("System.Collections.Generic"));

			Modifiers mods = Modifiers.None;
			
			if (Get<CheckBox>("isStatic").Checked)
				mods |= Modifiers.Static;

			if (Get<CheckBox>("isAbstract").Checked)
				mods |= Modifiers.Abstract;
			
			if (Get<CheckBox>("isSealed").Checked)
				mods |= Modifiers.Sealed;

			if (Get<CheckBox>("isPartial").Checked)
				mods |= Modifiers.Partial;
			
			ComboBox visibility = Get<ComboBox>("visibility");
			switch (visibility.SelectedIndex)
			{
					case 0: mods |= Modifiers.Public; break;
					case 1: mods |= Modifiers.Private; break;
					case 2: mods |= Modifiers.Protected; break;
					case 3: mods |= Modifiers.Internal; break;
			}
			
			TypeDeclaration domType = new TypeDeclaration(mods, null);
			domType.Name = Get<TextBox>("className").Text;
			domType.Type = ICSharpCode.NRefactory.Ast.ClassType.Class;
			
			ListBox ifacesList = Get<ListBox>("implementedInterfaces");
			foreach (string c in ifacesList.Items)
				domType.BaseTypes.Add(new TypeReference(c));

			domNS.AddChild(domType);
			
			if (Get<RadioButton>("createNewFile").Checked)
			{
				StreamWriter sw;
				string filename = Get<TextBox>("newFileName").Text;
				
				if (!Path.HasExtension(filename))
					filename += "." + proj.LanguageProperties.CodeDomProvider.FileExtension;
				
				filename = Path.Combine(proj.Directory, filename);
				sw = File.CreateText(filename);
				sw.Write(proj.LanguageProperties.CodeGenerator.GenerateCode(domNS, String.Empty));
				sw.Close();

				FileProjectItem fpi = new FileProjectItem(proj, ItemType.Compile);
				fpi.FileName = filename;
				ProjectService.AddProjectItem(proj, fpi);
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
				proj.Save();
			}
			else
			{
				string filename = Path.Combine(proj.Directory, Get<ComboBox>("existingFiles").Text);
				IViewContent viewContent = FileService.OpenFile(filename);
				
				proj.LanguageProperties.CodeGenerator.InsertCodeAtEnd(DomRegion.Empty, GetDocument(viewContent), new AbstractNode[] { domNS });
			}
		}
		
		private static IDocument GetDocument(IViewContent viewContent)
		{
			ITextEditorControlProvider provider1 = viewContent as ITextEditorControlProvider;
			if (provider1 == null)
			{
				return null;
			}
			return new TextEditorDocument (provider1.TextEditorControl.Document);
		}

		void SelectClassButtonClick(object sender, EventArgs e)
		{
			if (classes.ShowDialog())
			{
				Get<TextBox>("inheritFrom").Text = classes.SelectedClasses[0].FullyQualifiedName;
			}
		}
		
		void SelectInterfacesButtonClick(object sender, EventArgs e)
		{
			if (interfaces.ShowDialog())
			{
				ListBox ifacesList = Get<ListBox>("implementedInterfaces");
				foreach (IClass c in interfaces.SelectedClasses)
				{
					ifacesList.Items.Add(c.FullyQualifiedName);
				}
			}
		}
		
		void FileCreationModeChanged(object sender, EventArgs e)
		{
			bool createNewState = Get<RadioButton>("createNewFile").Checked;
			bool addToFileState = Get<RadioButton>("addToFile").Checked;
			
			Get<TextBox>("newFileName").Enabled = createNewState;
			Get<Button>("browse").Enabled = createNewState;
			Get<ComboBox>("existingFiles").Enabled = addToFileState;
		}
	}
}
	
