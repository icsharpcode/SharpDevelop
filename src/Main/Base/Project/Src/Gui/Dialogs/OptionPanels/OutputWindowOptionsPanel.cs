// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// The Output Window options panel.
	/// </summary>
	public class OutputWindowOptionsPanel : AbstractOptionPanel
	{
		public static readonly string OutputWindowsProperty = "SharpDevelop.UI.OutputWindowOptions";
		FontSelectionPanel fontSelectionPanel;
		
		public OutputWindowOptionsPanel()
		{
		}
		
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.OutputWindowOptionsPanel.xfrm"));
			
			Properties properties = (Properties)PropertyService.Get(OutputWindowsProperty, new Properties());
			fontSelectionPanel = new FontSelectionPanel();
			fontSelectionPanel.Dock = DockStyle.Fill;
			ControlDictionary["FontGroupBox"].Controls.Add(fontSelectionPanel);
			((CheckBox)ControlDictionary["wordWrapCheckBox"]).Checked = properties.Get("WordWrap", true);
			
			fontSelectionPanel.CurrentFontString = properties.Get("DefaultFont", ResourceService.CourierNew10.ToString()).ToString();
		}
		
		public override bool StorePanelContents()
		{
			Properties properties = (Properties)PropertyService.Get(OutputWindowsProperty, new Properties());
			properties.Set("WordWrap", ((CheckBox)ControlDictionary["wordWrapCheckBox"]).Checked);
			properties.Set("DefaultFont", fontSelectionPanel.CurrentFontString);
			
			PropertyService.Set(OutputWindowsProperty, properties);
			return true;
		}
		
		
		
	}
}
