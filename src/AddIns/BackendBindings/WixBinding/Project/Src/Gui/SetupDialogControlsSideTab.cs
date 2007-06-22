// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing.Design;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.WixBinding
{
	public sealed class SetupDialogControlsSideTab : SideTabDesigner
	{
		SetupDialogControlsSideTab(SideBarControl sideBar, Category category, IToolboxService toolboxService)
			: base(sideBar, category, toolboxService)
		{
		}
		
		/// <summary>
		/// Creates a new instance of the SetupDialogControlsSideTab class.
		/// </summary>
		public static SetupDialogControlsSideTab CreateSideTab()
		{
			ComponentLibraryLoader loader = new ComponentLibraryLoader();
			loader.LoadToolComponentLibrary(ToolComponentLibraryFileName);
			
			// Fix WixBinding assembly hint path.
			Category category = (Category)loader.Categories[0];
			foreach (ToolComponent toolComponent in category.ToolComponents) {
				toolComponent.HintPath = StringParser.Parse(toolComponent.HintPath);
			}
			return new SetupDialogControlsSideTab(WixDialogDesigner.SetupDialogControlsToolBox, category, ToolboxProvider.ToolboxService);
		}
		
		/// <summary>
		/// Gets the file that contains the list of controls supported by the Wix dialog
		/// designer.
		/// </summary>
		static string ToolComponentLibraryFileName {
			get {
				Assembly assembly = typeof(SetupDialogControlsSideTab).Assembly;
        		string assemblyFilename = assembly.CodeBase.Replace("file:///", String.Empty);
        		string directory = Path.GetDirectoryName(assemblyFilename);
        		return Path.Combine(directory, "SetupDialogControlLibrary.sdcl");
			}
		}
	}
}
