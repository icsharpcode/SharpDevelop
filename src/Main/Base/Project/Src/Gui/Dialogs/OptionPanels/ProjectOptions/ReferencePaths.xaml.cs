/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.03.2012
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ReferencePathsXAML.xaml
	/// </summary>
	public  partial class ReferencePaths : ProjectOptionPanel
	{
		public ReferencePaths()
		{
			InitializeComponent();
			
			editor.BrowseForDirectory = true;
			editor.TitleText = StringParser.Parse("${res:Global.Folder}:");
			editor.ListCaption = StringParser.Parse("${res:Dialog.ProjectOptions.ReferencePaths}:");
			editor.ListChanged += delegate { IsDirty = true; };
		}
		
		
		public ProjectProperty<string> ReferencePath {
			get {
				return GetProperty("ReferencePath", "",TextBoxEditMode.EditEvaluatedProperty);
			}
		}
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			string prop  = GetProperty("ReferencePath", "", TextBoxEditMode.EditRawProperty).Value;
			
			string[] values = prop.Split(';');
			if (values.Length == 1 && values[0].Length == 0) {
				editor.LoadList(new string[0]);
			} else {
				editor.LoadList(values);
			}
		}
	
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			ReferencePath.Value = String.Join(";", editor.GetList());
			return base.Save(project, configuration, platform);
		}
		
	}
}