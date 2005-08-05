// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Configuration settings for the xml editor.
	/// </summary>
	public class XmlEditorOptionsPanel : AbstractOptionPanel
	{
		static readonly string showAttributesWhenFoldedCheckBoxName = "showAttributesWhenFoldedCheckBox";
		static readonly string showSchemaAnnotationCheckBoxName = "showSchemaAnnotationCheckBox";
		
		public XmlEditorOptionsPanel()
		{
		}
		
		/// <summary>
		/// Initialises the panel.
		/// </summary>
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.XmlEditorOptionsPanel.xfrm"));
				
			((CheckBox)ControlDictionary[showAttributesWhenFoldedCheckBoxName]).Checked = XmlEditorAddInOptions.ShowAttributesWhenFolded;
			((CheckBox)ControlDictionary[showSchemaAnnotationCheckBoxName]).Checked = XmlEditorAddInOptions.ShowSchemaAnnotation;		
		}

		/// <summary>
		/// Saves any changes.
		/// </summary>
		public override bool StorePanelContents()
		{
			XmlEditorAddInOptions.ShowAttributesWhenFolded = ((CheckBox)ControlDictionary[showAttributesWhenFoldedCheckBoxName]).Checked;
			XmlEditorAddInOptions.ShowSchemaAnnotation = ((CheckBox)ControlDictionary[showSchemaAnnotationCheckBoxName]).Checked;
			
			return true;
		}
	}
}
