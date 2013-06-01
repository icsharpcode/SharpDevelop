/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.ObjectModel;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;

namespace ICSharpCode.Reporting.Interfaces.Data
{
	/// <summary>
	/// Description of IDataViewHandling.
	/// </summary>
	public interface IDataViewHandling:IEnumerator{
		
		void Sort ();
		
		void Group();
		
		void Bind();
		
//		void Fill (int position,ReportItemCollection collection);
		
		//rausnehmen
//		void Fill (IDataItem item);
		
		void Fill(ReportItemCollection collection);
		
		IndexList IndexList {get;}
		
//        object CurrentFromPosition(int pos);
        
//		CurrentItemsCollection FillDataRow();
//        CurrentItemsCollection FillDataRow(int pos);
		//
		Collection<AbstractColumn> AvailableFields {get;}
	
		int Count {get;}
	
 		int CurrentPosition {get;set;}
 		
// 		IExpressionEvaluatorFacade ExpressionEvaluator {get;}
	}
}
