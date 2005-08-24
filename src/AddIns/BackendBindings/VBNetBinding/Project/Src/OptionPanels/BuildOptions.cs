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

namespace VBNetBinding.OptionPanels
{
	public class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("BuildOptions.xfrm");
			InitializeHelper();
			
			helper.BindString("conditionalSymbolsTextBox", "DefineConstants");
			helper.BindBoolean("optimizeCodeCheckBox", "Optimize", false);
			helper.BindBoolean("removeOverflowCheckBox", "RemoveIntegerChecks", false);
			
			helper.BindStringEnum("optionStrictComboBox", "OptionStrict", "Off",
			                      new StringPair("Off", "Strict Off"),
			                      new StringPair("On", "Strict On"));
			helper.BindStringEnum("optionExplicitComboBox", "OptionExplicit", "On",
			                      new StringPair("Off", "Explicit Off"),
			                      new StringPair("On", "Explicit On"));
			helper.BindStringEnum("optionCompareComboBox", "OptionCompare", "Binary",
			                      new StringPair("Binary", "Compare Binary"),
			                      new StringPair("Text", "Compare Text"));
			
			InitOutputPath();
			InitXmlDoc();
			InitAdvanced();
			InitWarnings();
		}
		
		public override bool StorePanelContents()
		{
			return base.StorePanelContents();
		}
	}
}
