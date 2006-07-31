// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class PreprocessorVariablesPanel : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			InitializeHelper();
			
			NameValueListEditor editor = new NameValueListEditor();
			editor.ListChanged += NameValueEditorListChanged;
			SemicolonSeparatedNameValueListBinding b = new SemicolonSeparatedNameValueListBinding(editor);
			helper.AddBinding("DefineConstants", b);
			Controls.Add(editor);
			b.CreateLocationButton(editor);

			helper.AddConfigurationSelector(this);
		}
		
		void NameValueEditorListChanged(object source, EventArgs e)
		{
			IsDirty = true;
		}
	}
}
