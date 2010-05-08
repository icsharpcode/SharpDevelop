// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.Reports.Core{
	public interface IDataViewStrategy:IEnumerator,IDisposable{
		
		void Sort ();
		
		void Bind();
		
		void Fill (IReportItem item);
		//test
		CurrentItemsCollection FillDataRow();
		//
		AvailableFieldsCollection AvailableFields {get;}
	
		int Count {get;}
	
 		int CurrentPosition {get;set;}
 	
 		bool HasMoreData  {
 			get;
 		}
 		
 		bool IsSorted {get;}
 		/*
 		bool IsFiltered{get;}
 		
 		bool IsGrouped {get;}
 		
 		bool HasChildren {get;}
 		
 		IndexList ChildRows {get;}
 */
	}
}
