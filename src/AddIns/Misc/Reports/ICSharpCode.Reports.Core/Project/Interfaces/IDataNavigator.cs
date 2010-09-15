// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

 namespace ICSharpCode.Reports.Core {
 	
	public interface IDataNavigator {
 		
 		void Fill (ReportItemCollection collection);
 		bool MoveNext () ;
 		void Reset();
 		
 		CurrentItemsCollection GetDataRow();
 		
 		bool HasMoreData {
 			get;
 		}
 		
 		#region Try make recursive with ChildNavigator
 		
 		IDataNavigator GetChildNavigator();
 		
 		#endregion
 		
 		// childList -----------------
 		
 		bool HasChildren {get;}
 		
 		
 		int ChildListCount {get;}
 		
 		void FillChild (ReportItemCollection collection);
 		
 		// end child list --------------------
 		
 		bool IsSorted {get;}
 		
 		bool IsGrouped {get;}
 		
 		int CurrentRow  {get;}
 			
 		int Count  {get;}
	
 		object Current {get;}
 		
 		AvailableFieldsCollection AvailableFields{get;}
 		 
// 		System.Collections.IEnumerator RangeEnumerator(int start, int end);
 			
 	}
 }
