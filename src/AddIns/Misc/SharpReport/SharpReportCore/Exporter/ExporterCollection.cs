/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 19.09.2006
 * Time: 22:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpReportCore.Exporters{
	
	public class ExporterCollection<T> : Collection<T>
		where T : BaseExportColumn {
		
		public void AddRange (IEnumerable <T> items){
			foreach (T item in items) {
				this.Add (item);
			}
		}
	}
	public class PagesCollection  :Collection<SinglePage>
	{
		
	}
}
