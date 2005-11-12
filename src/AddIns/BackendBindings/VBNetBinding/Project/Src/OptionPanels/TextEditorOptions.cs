// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="markuspalme@gmx.de"/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace VBNetBinding.OptionPanels
{
	public class VBNetTextEditorPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.VBNetTextEditorOptions.xfrm"));
			Get<CheckBox>("enableEndConstructs").Checked = PropertyService.Get<bool>("VBBinding.TextEditor.EnableEndConstructs", true);
			Get<CheckBox>("enableCasing").Checked = PropertyService.Get<bool>("VBBinding.TextEditor.EnableCasing", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set<bool>("VBBinding.TextEditor.EnableEndConstructs", Get<CheckBox>("enableEndConstructs").Checked);
			PropertyService.Set<bool>("VBBinding.TextEditor.EnableCasing", Get<CheckBox>("enableCasing").Checked);
			return true;
		}
	}
}
