// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;

using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core{
	public interface IDataViewStrategy:IEnumerator,IDisposable{
		
		void Sort ();
		
		void Group();
		
		void Bind();
		
		void Fill (int position,ReportItemCollection collection);
		
		//rausnehmen
		void Fill (IDataItem item);
		
		IndexList IndexList {get;set;}
		
        object CurrentFromPosition(int pos);
        
		CurrentItemsCollection FillDataRow();
        CurrentItemsCollection FillDataRow(int pos);
		//
		AvailableFieldsCollection AvailableFields {get;}
	
		int Count {get;}
	
 		int CurrentPosition {get;set;}
 		
 		IExpressionEvaluatorFacade ExpressionEvaluator {get;}
	}
}
