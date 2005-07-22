// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class EditTemplateDialog : BaseSharpDevelopForm 
	{
		CodeTemplate codeTemplate;
		
		public CodeTemplate CodeTemplate {
			get {
				return codeTemplate;
			}
		}
		
		public EditTemplateDialog(CodeTemplate codeTemplate)
		{
			this.codeTemplate = codeTemplate;
			InitializeComponents();
		}
		
		void AcceptEvent(object sender, EventArgs e)
		{
			codeTemplate.Shortcut    = ControlDictionary["templateTextBox"].Text;
			codeTemplate.Description = ControlDictionary["descriptionTextBox"].Text;
		}
		
		void InitializeComponents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.EditTemplateDialog.xfrm"));
			
			ControlDictionary["templateTextBox"].Text    = codeTemplate.Shortcut;
			ControlDictionary["descriptionTextBox"].Text = codeTemplate.Description;
			
			ControlDictionary["okButton"].Click += new EventHandler(AcceptEvent);
			
			Owner = (Form)WorkbenchSingleton.Workbench;
			Icon  = null;
		}
	}
}
