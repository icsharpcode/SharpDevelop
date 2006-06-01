/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 02.12.2004
 * Time: 10:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;


namespace SharpReportCore{
	
	/// <summary>
	/// Used by <see cref="SharpReport.Designer.BaseDesignerControl"></see>
	/// </summary>
	
	public interface  IVisitor{
		void Accept(SharpReportCore.IModelVisitor visitor);
	}
	
	public interface IModelVisitor{
			///<summary>
			/// Use this function to Visit from ReportEngine
			/// </summary>
			void Visit (SharpReportCore.ReportModel reportModel);
			///<summary>
			/// This function is used by the Designer
			/// </summary>
			void Visit (System.Windows.Forms.Control designer);
	}
}
	
