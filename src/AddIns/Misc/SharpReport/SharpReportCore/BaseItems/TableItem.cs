/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 17.04.2006
 * Time: 15:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpReportCore.BaseItems {
	/// <summary>
	/// Description of TableItem.
	/// </summary>
	public class TableItem :BaseReportItem,IContainerItem{
		private Padding padding;
//		private string tableName;
		
		public TableItem(){
		
		}
		
		
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
				throw new NotImplementedException();
			}
		}
		
		public bool IsValidChild(BaseReportItem childControl){
			throw new NotImplementedException();
		}
		
		#endregion
		
	}
}
