// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Xml.Xsl;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Commands
{
	public class ImportProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (ImportProjectDialog importProjectDialog = new ImportProjectDialog()) {
				importProjectDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				importProjectDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
}

