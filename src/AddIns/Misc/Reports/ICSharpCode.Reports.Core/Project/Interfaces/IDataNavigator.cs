// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

 namespace ICSharpCode.Reports.Core {
 	
	public interface IDataNavigator {
 		
 		void Fill (ReportItemCollection collection);
 		bool MoveNext () ;
 		void Reset();
 		
 		CurrentItemsCollection GetDataRow {get;}
 			
 		
 		bool HasMoreData {
 			get;
 		}
 		
 		#region Try make recursive with ChildNavigator
 		
 		IDataNavigator GetChildNavigator{get;}
 		
 		#endregion
 		
 		// childList -----------------
 		
 		bool HasChildren {get;}
 
 		// end child list --------------------
 		
 		bool IsSorted {get;}
 		
 		bool IsGrouped {get;}
 		
 		int CurrentRow  {get;}
 			
 		int Count  {get;}
	
 		object Current {get;}
 		
 		AvailableFieldsCollection AvailableFields{get;}
 		 
 	}
 }
