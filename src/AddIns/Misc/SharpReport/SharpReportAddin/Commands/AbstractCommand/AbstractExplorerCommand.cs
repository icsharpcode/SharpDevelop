/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 08.06.2005
 * Time: 22:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

using SharpReportAddin;

/// <summary>
/// Description of AbstractExplorerCommand
/// </summary>
namespace SharpReportAddin.Commands{	
	public abstract class AbstractExplorerCommand : AbstractMenuCommand{
		FieldsExplorer fieldsExplorer = null;
		
		protected AbstractExplorerCommand(){
			Type type = typeof(FieldsExplorer);
			fieldsExplorer = (SharpReportAddin.FieldsExplorer)WorkbenchSingleton.Workbench.GetPad(type).PadContent;
			if (fieldsExplorer == null) {
				throw new NullReferenceException ("AbstractExplorerCommand : No FieldExplorer Pad  available");
			}
		}
			/// <summary>
		/// Enabled or disabled the command
		/// <remarks> /remarks>
		/// </summary>				
		public override bool IsEnabled {
			get{
				if (fieldsExplorer != null) {
					return true;
				}
				return false;
			}			
			set{}
		}
		
		public FieldsExplorer FieldsExplorer {
			get {
				return fieldsExplorer;
			}
		}
		
	}
}
