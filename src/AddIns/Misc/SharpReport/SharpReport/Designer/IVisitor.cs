/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 10.08.2005
 * Time: 13:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

 using System;
 using SharpReport.Designer;
 
 namespace SharpReport.Designer{
 	/// <summary>
 	/// Implemented in SharpReportDesigner
 	/// </summary>
 	
 	public interface  IVisitor{
 		void Accept(IDesignerVisitor visitor);
 	}
 }
