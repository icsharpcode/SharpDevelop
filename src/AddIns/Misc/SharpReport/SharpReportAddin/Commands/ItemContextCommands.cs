/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 24.01.2005
 * Time: 22:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using SharpReportCore;

 /// <summary>
 /// This Class contains all commands belonging to a ReportItem
 /// </summary>
 /// 
 
namespace SharpReportAddin.Commands {
	
 	/// <summary>
 	/// Cut the ReportItem
 	/// </summary>
 	/// 
	public class CutReportItem : AbstractSharpReportCommand{
		/// <summary>
		/// Creates a new ContextCommands
		/// </summary>
		public CutReportItem(){
		}
		
		public override bool IsEnabled {
			get {
				if (base.View.DesignerControl.SelectedObject is IItemRenderer) {
					return true;
				}
				return false;
			}
		}
		
		/// <summary>
		///  Cut Selected ReportItem
		/// </summary>
		/// 
		public override void Run(){
			if (this.IsEnabled){
				base.View.RemoveSelectedItem();
			}
		}
	}
	
	public class PasteReportItem : AbstractSharpReportCommand
	{
		/// <summary>
		/// Creates a new ContextCommands
		/// </summary>
		public PasteReportItem(){
		}
		
		public override bool IsEnabled {
			get {
//				if (base.View.DesignerControl.SelectedObject is ReportItem) {
//					return true;
//				} 
				return false;
			}
		}
		
		/// <summary>
		///  Cut Selected ReportItem
		/// </summary>
		/// 
		public override void Run(){
//			if (base.View.DesignerControl.SelectedObject is ReportItem) {
//				try {
//					ReportItem it = (ReportItem)base.View.DesignerControl.SelectedObject;
//					if (base.View.DesignerControl.SelectedSection.Items.Contains(it)) {
//						MessageBox.Show ("ContextCommand " + it.Name);
//						base.View.DesignerControl.SelectedSection.Items.Remove ((ReportItem)base.View.DesignerControl.SelectedObject);
//						base.View.IsDirty = true;
//					}
//				} catch (Exception) {
//					
//				}
//			}
			MessageBox.Show (this.ToString() + " not implemented");
		}
	}
	
	///<summary>Show a List of available Fields</summary>
	
	
}
