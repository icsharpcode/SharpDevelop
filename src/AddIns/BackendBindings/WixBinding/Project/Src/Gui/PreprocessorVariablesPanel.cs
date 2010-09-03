// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.WixBinding
{
	public class PreprocessorVariablesPanel : AbstractXmlFormsProjectOptionPanel
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
