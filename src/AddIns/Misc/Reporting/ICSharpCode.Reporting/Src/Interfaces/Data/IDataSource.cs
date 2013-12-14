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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;

namespace ICSharpCode.Reporting.Interfaces.Data
{
	/// <summary>
	/// Description of IDataViewHandling.
	/// </summary>
	public interface IDataSource{
		
		void Bind();
		
		void Fill(List<IPrintableObject> collection);
		
		Collection<AbstractColumn> AvailableFields {get;}
		IList <object> CurrentList {get;}
		int Count {get;}
		
		object Current {get;}
		OrderGroup OrderGroup {get;}
		IGrouping<object, object> CurrentKey {get;}
		
	}
}
