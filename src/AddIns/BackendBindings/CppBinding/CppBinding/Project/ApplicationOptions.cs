// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-06
 * Godzina: 22:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Application settings panel for c++ project.
	/// </summary>
	public class ApplicationOptions : ICSharpCode.SharpDevelop.Gui.OptionPanels.ApplicationSettings
	{
		public override void LoadPanelContents()
		{
			base.LoadPanelContents();
			ComboBox cbOutputType = Get<ComboBox>("outputType");
			helper.AddBinding("ConfigurationType", new ObservedBinding<string, ComboBox>(cbOutputType, ConvertOutputType));
			
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, helper.Configuration, helper.Platform);
			string subsystem = group.GetElementMetadata("Link", "SubSystem");
			string configurationType = project.GetEvaluatedProperty("ConfigurationType");
			OutputType validOutputType = ConfigurationTypeToOutputType(configurationType, subsystem);
			cbOutputType.SelectedIndex = Array.IndexOf((OutputType[])Enum.GetValues(typeof(OutputType)), validOutputType);
			
			TextBox tbApplicationIcon = Get<TextBox>("applicationIcon");
			helper.AddBinding(null, new ObservedBinding<object, TextBox>(tbApplicationIcon, SetApplicationIcon));
			tbApplicationIcon.Text = GetApplicationIconPathFromResourceScripts();			
			
			DisableWin32ResourceOptions();
			
			IsDirty = false;
		}
		
		#region OutputType <-> ConfigurationType property mapping
		
		/// <summary>
		/// Applies the OutputType property value from combo box control to the vcxproj project.
		/// <para>The OutputType property is translated to ConfigurationType and Subsystem properties</para>
		/// </summary>
		/// <returns>the ConfigurationType associated to OutputType</returns>
		string ConvertOutputType(ComboBox cbOutputType)
		{
			OutputType[] values = (OutputType[])Enum.GetValues(typeof(OutputType));
			OutputType outputType = values[cbOutputType.SelectedIndex];
			
			string subsystem = OutputTypeToSubsystem(outputType);
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, 
			                                            helper.Configuration, helper.Platform);
			group.SetElementMetadata("Link", "SubSystem", subsystem);
			
			return OutputTypeToConfigurationType(outputType);
		}
		
		static string OutputTypeToConfigurationType(OutputType outputType)
		{
			switch (outputType)
			{
				case OutputType.Exe:
					return "Application";
				case OutputType.Library:
					return "DynamicLibrary";
				case OutputType.Module:
					//TODO: get an apropriate way to handle netmodule creation
					//see: http://msdn.microsoft.com/en-us/library/k669k83h(VS.80).aspx
					LoggingService.Info(".netmodule output not supported, will produce a class library");
					return "DynamicLibrary";
				case OutputType.WinExe:
					return "Application";
			}
			throw new ArgumentException("Unknown OutputType value " + outputType);
		}
		
		static string OutputTypeToSubsystem(OutputType outputType)
		{
			if (OutputType.WinExe == outputType)
				return "Windows";
			return "Console";
		}
		
		static OutputType ConfigurationTypeToOutputType(string configurationType, string subsystem)
		{
			if ("Application" == configurationType && "Windows" != subsystem)
				return OutputType.Exe;
			else if ("Application" == configurationType && "Windows" == subsystem)
				return OutputType.WinExe;
			else if ("DynamicLibrary" == configurationType)
				return OutputType.Library;
			LoggingService.Info("ConfigurationType " +configurationType + " is not supported, will use Library output type");
			return OutputType.Library;
		}
		#endregion
		
		#region Application icon property mapping
		const string DEFAULT_ICON_ID = "ICON0";
		const string DEFAULT_RC_NAME = "app.rc";
		ResourceEntry foundIconEntry;
		string iconResourceScriptPath;	//path to the resource script where application icon is defined
		
		static string AddResourceScriptToProject(IProject project, string rcFileName) {
			string fileName = Path.Combine(project.Directory, rcFileName);
			FileProjectItem rcFileItem = new FileProjectItem(project, project.GetDefaultItemType(fileName));
			rcFileItem.Include = FileUtility.GetRelativePath(project.Directory, fileName);
			((IProjectItemListProvider)project).AddProjectItem(rcFileItem);
			return fileName;
		}
		
		/// <summary>
		/// Gets the icon file location from the rc files added to project. 
		/// Searches all project items of type "ResourceCompile" and returns the resource of type ICON with the lowest ID.
		/// </summary>
		/// <returns>path to the icon file or null if the icon wasn't specified</returns>
		string GetApplicationIconPathFromResourceScripts() {
			foundIconEntry = null;
			iconResourceScriptPath = null;
			IEnumerable <ProjectItem> resourceScripts = project.Items.Where(
						item => item is FileProjectItem && ((FileProjectItem)item).BuildAction == "ResourceCompile");			
			
			// search in all resource scripts, but due to limitation in resource compiler, only one of them can contain icons
			foreach (ProjectItem item in resourceScripts) {
				ResourceScript rc = new ResourceScript(item.FileName);
				if (rc.Icons.Count == 0) continue;
				if (foundIconEntry == null || rc.Icons.First().ResourceID.CompareTo(foundIconEntry.ResourceID)<0) {
					foundIconEntry = rc.Icons.First();
					iconResourceScriptPath = item.FileName;
				}
			}
			
			//when no icon was found, then select the resource script where icon definition may be created
			if (iconResourceScriptPath == null && resourceScripts.Any())
				iconResourceScriptPath = resourceScripts.First().FileName;
				
			return foundIconEntry != null ? foundIconEntry.Data : null;
		}
		
		object SetApplicationIcon(TextBox tb) {            
			string iconPath = tb.Text;
			string newIconId;
			ResourceScript rc;
			if (iconPath.Trim() == "") return null;
			if (iconResourceScriptPath != null)
			{
				rc = new ResourceScript(iconResourceScriptPath);
				newIconId = foundIconEntry != null ? foundIconEntry.ResourceID : DEFAULT_ICON_ID;
				rc.Save(iconResourceScriptPath);
			}
			else
			{
				iconResourceScriptPath = AddResourceScriptToProject(project, DEFAULT_RC_NAME);
				rc = new ResourceScript();
				newIconId = DEFAULT_ICON_ID;
			}
			
			rc.SetIcon(newIconId, iconPath);
			rc.Save(iconResourceScriptPath);
			return null;
		}		
		#endregion
		
		#region Resource file property mapping
		void DisableWin32ResourceOptions()  {
			Button win32ResourceFileBrowseButton = Get<Button>("win32ResourceFileBrowse");
			win32ResourceFileBrowseButton.Enabled = false;
			TextBox win32ResourceFileTextBox = Get<TextBox>("win32ResourceFile");
			win32ResourceFileTextBox.Enabled = false;
		}
		#endregion
    }
}
