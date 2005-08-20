// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class BuildEvents : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.BuildEvents.xfrm");
			
			InitializeHelper();
			
			ConnectBrowseButton("preBuildEventBrowseButton",
			                    "preBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			ConnectBrowseButton("postBuildEventBrowseButton",
			                    "postBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			helper.BindString("preBuildEventTextBox", "PreBuildEvent");
			helper.BindString("postBuildEventTextBox", "PostBuildEvent");
			helper.BindEnum<RunPostBuildEvent>("runPostBuildEventComboBox", "RunPostBuildEvent");
		}
	}
}
