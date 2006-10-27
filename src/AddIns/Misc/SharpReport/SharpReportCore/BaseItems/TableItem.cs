/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 17.04.2006
 * Time: 15:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

namespace SharpReportCore {
	/// <summary>
	/// Description of TableItem.
	/// </summary>
	public class TableItem :BaseReportItem,IContainerItem{
		private Padding padding;
		private string tableName;
		private ReportItemCollection items;
		
		public TableItem():this (GlobalValues.UnboundName){
		}
		
		public TableItem(string tableName){
			this.tableName = tableName;
		}
		
		#region overrides
		
		public override string ToString(){
			return this.GetType().Name;
		}
		#endregion
		
		
		/*
		[Category("Databinding")]
		public string TableName {
			get {
				return tableName;
			}
//			set {
//				tableName = value;
//			}
		}
		*/
		
		
		#region Interface implementation of 'IContainerItem'
		
		public System.Windows.Forms.Padding Padding {
			get {
				return this.padding;
			}
			set {
				this.padding = value;
				base.NotifyPropertyChanged("Padding");
			}
		}
		
		public ReportItemCollection Items {
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
		
		public bool IsValidChild(BaseReportItem childControl){
			throw new NotImplementedException();
		}
		
		#endregion
		
	}
}
