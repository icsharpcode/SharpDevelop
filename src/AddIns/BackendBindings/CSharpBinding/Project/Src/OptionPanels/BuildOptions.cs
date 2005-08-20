// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace CSharpBinding.OptionPanels
{
	public class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("BuildOptions.xfrm");
			InitializeHelper();
			
			InitOutputPath();
			InitXmlDoc();
			
			helper.BindString("conditionalSymbolsTextBox", "DefineConstants");
			helper.BindBoolean("optimizeCodeCheckBox", "Optimize", false);
			helper.BindBoolean("allowUnsafeCodeCheckBox", "AllowUnsafeBlocks", false);
			helper.BindBoolean("checkForOverflowCheckBox", "CheckForOverflowUnderflow", false);
			helper.BindBoolean("noCorlibCheckBox", "NoStdLib", false);
			
			InitAdvanced();
			helper.BindStringEnum("fileAlignmentComboBox", "FileAlignment",
			                      "4096",
			                      new StringPair("512", "512"),
			                      new StringPair("1024", "1024"),
			                      new StringPair("2048", "2048"),
			                      new StringPair("4096", "4096"),
			                      new StringPair("8192", "8192"));
			InitWarnings();
		}
	}
}
