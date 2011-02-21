// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.Reports.Core{
	public interface IDataViewStrategy:IEnumerator,IDisposable{
		
		void Sort ();
		
		void Group();
		
		void Bind();
		
		void Fill (IDataItem item);
		
		IndexList IndexList {get;}
		//test
        object myCurrent(int pos);
		CurrentItemsCollection FillDataRow();
        CurrentItemsCollection FillDataRow(int pos);
		//
		AvailableFieldsCollection AvailableFields {get;}
	
		int Count {get;}
	
 		int CurrentPosition {get;set;}
 	
 		bool HasMoreData  {get;}
 		
 		bool IsSorted {get;}
 		
 		bool IsGrouped {get;set;}
 	
	}
}
