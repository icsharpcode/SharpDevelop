// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Configuration;
using System.IO;
using System.Resources;
using System.Security;
using System.Security.Policy;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class ComponentInspectorApplication
	{
		// Controls dialogs that show in the idle loop
		bool                   needsGettingStarted;
		bool                   startupComplete;
		
		// The file to open initially as specified on the command line
		string                 fileToOpen;
		
		ObjectBrowserForm      objectBrowserForm;
		
		public ComponentInspectorApplication()
		{
		}
		
		[STAThread]
		public static void Main(string[] args)
		{
			ComponentInspectorApplication app = new ComponentInspectorApplication();
						
			if (!app.ParseArguments(args)) {
				return;
			}
			
			try {
				app.Run();
				app.Shutdown();
			} catch (PolicyException pex) {
				ErrorDialog.Show(pex, "Security exception.  You don't have sufficient permissions to run the Inspector.",
					 "Insufficient Permissions", MessageBoxIcon.Error);
				return;
			} catch (SecurityException ex)	{
				ErrorDialog.Show(ex, "Security exception.  You don't have sufficient permissions to run the Inspector.  Permission needed: "
					 + ex.PermissionType + " state: " + ex.PermissionState,
					 "Insufficient Permissions", MessageBoxIcon.Error);
				return;
			} catch (Exception ex) {
				ErrorDialog.Show(ex, "(bug, please report) Unhandled exception.", 
					"Unhandled Exception", MessageBoxIcon.Error);
				return;
			}
		}
		
		void Run()
		{
			TraceUtil.Init();
			InitializeServices();

			needsGettingStarted = ComponentInspectorProperties.ShowGettingStartedDialog;

			Application.EnableVisualStyles();
			Application.Idle += new EventHandler(IdleHandler);
			objectBrowserForm = new ObjectBrowserForm();
							
			// If a file was specified on the command line, open only
			// that, otherwise restore any previously opened assemblies
			if (fileToOpen != null) {
				objectBrowserForm.OpenFile(fileToOpen);
			} else {
				ComSupport.RestoreComEnvironment();
				AssemblySupport.RestoreAssemblies();
			}

			startupComplete = true;
			Application.Run(objectBrowserForm);
		}
		
		void Shutdown()
		{
			SaveConfig();
		}
		
		bool ParseArguments(string[] args)
		{
			// Allow local preferences to be set from the command line
			try {
				for (int i = 0; i < args.Length; i = i + 2) {
					if (args[i].StartsWith("-")) {
						String setting = args[i].Substring(1);
						String value = args[i + 1];
						Console.WriteLine("Using setting: " + setting + " " + value);
						LocalPrefs.Set(setting, value);
					} else {
						// This is a DLL file to open
						fileToOpen = args[i];
					}
				}
				return true;
			} catch	{
				Console.WriteLine("Error in arguments: specify '-setting value'");
				return false;
			}
		}
		
		// The thread idle loop
		void IdleHandler(object sender, EventArgs e)
		{
			if (startupComplete) {
				if (needsGettingStarted) {
					needsGettingStarted = false;
					using (Form d = new GettingStartedDialog()) {
						d.ShowDialog(objectBrowserForm);
					}
				}
			}

			EventLogList.NewIncarnation();
			ObjectCreator.CheckOutstandingCreation();
		}
		
		void RunGettingStarted()
		{
			using (GettingStartedDialog d = new GettingStartedDialog()) {
				d.ShowDialog(objectBrowserForm);
			}
		}
		
		void InitializeServices()
		{
			PropertyService.InitializeService(GetConfigurationDirectory(), String.Empty, "ComponentInspector");
			PropertyService.Load();
			
			ResourceService.InitializeService(GetResourceDirectory());
			ResourceService.RegisterNeutralStrings(new ResourceManager("ComponentInspector.Resources.StringResources", typeof(ComponentInspectorApplication).Assembly));
		}
		
		string GetResourceDirectory()
		{
			string resourceDirectory = ConfigurationSettings.AppSettings.Get("ResourceDirectory");
			if (resourceDirectory != null) {
				return Path.Combine(Application.StartupPath, resourceDirectory);
			}
			return Path.Combine(Application.StartupPath, "resources");
		}
		
		/// <summary>
		/// Directory holding the ComponentInspector configuration.
		/// </summary>
		public static string GetConfigurationDirectory()
		{
			string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			return Path.Combine(appDataDirectory, "ComponentInspector");
		}
		
		void SaveConfig()
		{
			string configDirectory = GetConfigurationDirectory();
			if (!Directory.Exists(configDirectory)) {
				Directory.CreateDirectory(configDirectory);
			}
			PropertyService.Save();
		}
	}
}
