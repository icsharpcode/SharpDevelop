// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.ILAsmBinding
{
	public class CompilerParametersPanel : AbstractOptionPanel
	{
		ILAsmCompilerParameters compilerParameters = null;
		
		public override void LoadPanelContents()
		{
			this.compilerParameters = (ILAsmCompilerParameters)((Properties)CustomizationObject).Get("Config");
			
			System.Windows.Forms.PropertyGrid grid = new System.Windows.Forms.PropertyGrid();
			grid.Dock = DockStyle.Fill;
			grid.SelectedObjects = new object[] { compilerParameters};
			Controls.Add(grid);
		}
		
		public override bool StorePanelContents()
		{
			return true;
		}
	}
}
