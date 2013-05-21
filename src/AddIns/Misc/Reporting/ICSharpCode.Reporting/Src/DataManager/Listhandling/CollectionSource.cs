/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.Interfaces.Data;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.DataManager.Listhandling
{
	/// <summary>
	/// Description of CollectionHandling.
	/// </summary>
	internal class CollectionSource:IDataViewHandling
	{

		private PropertyDescriptorCollection listProperties;
		private DataCollection<object> baseList;
		private ReportSettings reportSettings;
		
		
		public CollectionSource(IList list,ReportSettings reportSettings)
		{
			
			if (list.Count > 0) {
//				firstItem = list[0];

				var itemType = list[0].GetType();
				this.baseList = new DataCollection <object>(itemType);
				this.baseList.AddRange(list);
			}
			this.reportSettings = reportSettings;
			this.listProperties = this.baseList.GetItemProperties(null);
			IndexList = new IndexList();
		}
		
		public  int Count
		{
			get {
				return this.baseList.Count;
			}
		}
		
		public Collection<AbstractColumn> AvailableFields {
			get {
//				base.AvailableFields.Clear();
				var av = new Collection<AbstractColumn>();
				foreach (PropertyDescriptor p in this.listProperties){
					av.Add (new AbstractColumn(p.Name,p.PropertyType));
				}
				return av;
			}
		}
		
		public object Current {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Sort()
		{
			throw new NotImplementedException();
		}
		
		public bool MoveNext()
		{
			throw new NotImplementedException();
		}
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
		
		public IndexList IndexList {get; private set;}
		
	}
}
