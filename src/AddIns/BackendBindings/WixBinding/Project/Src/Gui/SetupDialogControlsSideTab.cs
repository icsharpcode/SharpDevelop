// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Drawing.Design;
using System.IO;
using System.Reflection;

namespace ICSharpCode.WixBinding
{
	public sealed class SetupDialogControlsSideTab : SideTabDesigner
	{
		SetupDialogControlsSideTab(AxSideBar sideBar, Category category, IToolboxService toolboxService)
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
			Category category = (Category)loader.Categories[0];
			return new SetupDialogControlsSideTab(SharpDevelopSideBar.SideBar, category, ToolboxProvider.ToolboxService);
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
