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
