// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//	 <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static string[] commandLineArgs = null;
		
		public static string[] CommandLineArgs {
			get {
				return commandLineArgs;
			}
		}
		
		static void ShowErrorBox(object sender, ThreadExceptionEventArgs eargs)
		{
			DialogResult result = new ExceptionBox(eargs.Exception).ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			
			switch (result) {
				case DialogResult.Ignore:
					break;
				case DialogResult.Abort:
					Application.Exit();
					break;
				case DialogResult.Yes:
					break;
			}
		}
		
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic.dll",
			"Microsoft.JScript.dll",
			"mscorlib.dll",
			"System.Data.dll",
			"System.Design.dll",
			"System.DirectoryServices.dll",
			"System.Drawing.Design.dll",
			"System.Drawing.dll",
			"System.EnterpriseServices.dll",
			"System.Management.dll",
			"System.Messaging.dll",
			"System.Runtime.Remoting.dll",
			"System.Runtime.Serialization.Formatters.Soap.dll",

			"System.Security.dll",
			"System.ServiceProcess.dll",
			"System.Web.Services.dll",
			"System.Web.dll",
			"System.Windows.Forms.dll",
			"System.dll",
			"System.XML.dll"
		};
		
		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			commandLineArgs = args;
			bool noLogo = false;
			
			SplashScreenForm.SetCommandLineArgs(args);
			
			foreach (string parameter in SplashScreenForm.GetParameterList()) {
				switch (parameter.ToUpper()) {
					case "NOLOGO":
						noLogo = true;
						break;
				}
			}
			
			if (!noLogo) {
				SplashScreenForm.SplashScreen.Show();
			}
			
			Application.ThreadException += new ThreadExceptionEventHandler(ShowErrorBox);
			
//	TODO:		
//			bool ignoreDefaultPath = false;
//			string [] addInDirs = ICSharpCode.SharpDevelop.AddInSettingsHandler.GetAddInDirectories(out ignoreDefaultPath);
//			SetAddInDirectories(addInDirs, ignoreDefaultPath);
			
			AddInTree.Auswerter.Add("ActiveContentExtension", new ActiveContentExtensionAuswerter());
			AddInTree.Auswerter.Add("ActiveViewContentUntitled", new ActiveViewContentUntitledAuswerter());
			AddInTree.Auswerter.Add("ActiveWindowState", new ActiveWindowStateAuswerter());
			AddInTree.Auswerter.Add("CombineOpen", new CombineOpenAuswerter());
			AddInTree.Auswerter.Add("DebuggerSupports", new DebuggerSupportsAuswerter());
			AddInTree.Auswerter.Add("IsProcessRunning", new IsProcessRunningAuswerter());
			AddInTree.Auswerter.Add("OpenWindowState", new OpenWindowStateAuswerter());
			AddInTree.Auswerter.Add("WindowActive", new WindowActiveAuswerter());
			AddInTree.Auswerter.Add("WindowOpen", new WindowOpenAuswerter());
			AddInTree.Auswerter.Add("ProjectActive", new ProjectActiveAuswerter());
			AddInTree.Auswerter.Add("ProjectOpen", new ProjectOpenAuswerter());
			AddInTree.Auswerter.Add("TextContent", new ICSharpCode.SharpDevelop.DefaultEditor.Conditions.TextContentAuswerter());
			
			AddInTree.Erbauer.Add("DialogPanel", new DialogPanelErbauer());
			AddInTree.Erbauer.Add("DisplayBinding", new DisplayBindingErbauer());
			AddInTree.Erbauer.Add("Pad", new PadErbauer());
			AddInTree.Erbauer.Add("LanguageBinding", new LanguageBindingErbauer());
			AddInTree.Erbauer.Add("Parser", new ParserErbauer());
			AddInTree.Erbauer.Add("EditAction", new ICSharpCode.SharpDevelop.DefaultEditor.Codons.EditActionErbauer());
			AddInTree.Erbauer.Add("SyntaxMode", new ICSharpCode.SharpDevelop.DefaultEditor.Codons.SyntaxModeErbauer());
			
			PropertyService.Load();
			
			StringParser.RegisterStringTagProvider(new ICSharpCode.SharpDevelop.Commands.SharpDevelopStringTagProvider());
			
			AddInTree.Load();
			
			// .NET base autostarts
			// taken out of the add-in tree for performance reasons (every tick in startup counts)
			new ICSharpCode.SharpDevelop.Commands.InitializeWorkbenchCommand().Run();
			
			// run workspace autostart commands
			try {
				ArrayList commands = AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				foreach (ICommand command in commands) {
					command.Run();
				}
			} catch (XmlException e) {
				MessageBox.Show("Could not load XML :" + Environment.NewLine + e.Message);
				return;
			} catch (Exception e) {
				MessageBox.Show("Loading error, please reinstall :"  + Environment.NewLine + e.ToString());
				return;
			} finally {
				if (SplashScreenForm.SplashScreen != null) {
					SplashScreenForm.SplashScreen.Dispose();
				}
			}
			
			// finally start the workbench.
			try {
				new ICSharpCode.SharpDevelop.Commands.StartWorkbenchCommand().Run();
			} finally {
				// unloading 
				ProjectService.CloseSolution();
				FileService.Unload();
				PropertyService.Save();
			}
		}
	}
}
