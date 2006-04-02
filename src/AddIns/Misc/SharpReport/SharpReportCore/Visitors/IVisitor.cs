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
		
	public interface IModel{
			void Accept(IModelVisitor visitor);
	}	
	
	public interface IModelVisitor{
			void Visit (SharpReportCore.ReportModel reportModel);
	}
}
	
