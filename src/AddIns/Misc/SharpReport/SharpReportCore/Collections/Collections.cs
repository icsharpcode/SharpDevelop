/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 30.12.2005
 * Time: 11:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;


namespace SharpReportCore{
	
	public class CollectionItemEventArgs<T> : EventArgs
	{
		T item;
		
		public T Item {
			get {
				return item;
			}
		}
		
		public CollectionItemEventArgs(T item)
		{
			this.item = item;
		}
	}
	///<summary>
	/// Comparer to Sort the <see cref="IItemRendererCollection"></see>
	/// by System.Drawing.Location.Y, so we have the IItemRenderers in the Order we use them
	/// (Line by Line)
	/// </summary>
	public class LocationSorter : IComparer<IItemRenderer>  {
		public int Compare(IItemRenderer left, IItemRenderer right){
			if (left == null){
				if (right == null){
					return 0;
				}
				return -1;
			}
			if (right == null){
				return 1;
			}
			
			if (left.Location.Y == right.Location.Y){
				return left.Location.X - right.Location.X;
			}
			return left.Location.Y - right.Location.Y;
		}
	}
	

	
	///<summary>
	///  A collection that holds <see cref='IItemRenderer'/> objects.
	///</summary>
	
	public class ReportItemCollection: List<IItemRenderer>{
		
		public event EventHandler<CollectionItemEventArgs<IItemRenderer>> Added;
		public event EventHandler<CollectionItemEventArgs<IItemRenderer>> Removed;
		
		public void SortByLocation () {
			if (this.Count > 1) {
				this.Sort(new LocationSorter());
				for (int i = 0;i < this.Count ; i ++) {
					IItemRenderer col = this[i];
				}
			}
		}
		
		public IItemRenderer Find (string itemName) {
			for (int i = 0;i < this.Count ; i ++) {
				IItemRenderer col = this[i];
				if (String.Compare(col.Name,itemName)== 0){
					return col;
				}
			}
			return null;
		}
		
		
		public new void Add(IItemRenderer item){
			base.Add(item);
			this.SortByLocation();
			this.OnAdded (item);
		}
		
		
		
		public new bool Remove(IItemRenderer item)
		{
			if (base.Remove (item)) {
				this.SortByLocation();
				this.OnRemoved(item);
				return true;
			}
			return false;
		}
		
		
		
		void OnAdded(IItemRenderer item){
			if (Added != null)
				Added(this, new CollectionItemEventArgs<IItemRenderer>(item));
		}
		
		void OnRemoved( IItemRenderer item){
			if (Removed != null)
				Removed(this, new CollectionItemEventArgs<IItemRenderer>(item));
		}
	}
	
	/// <summary>
	/// This class holds all the available Sections of an Report
	/// </summary>
	public class ReportSectionCollection: List<BaseReportObject>{
		
		public BaseReportObject Find (string columnName) {
			for (int i = 0;i < this.Count ; i ++) {
				BaseReportObject col = this[i];
				if (String.Compare(col.Name.ToLower(CultureInfo.CurrentCulture),
				                   columnName.ToLower(CultureInfo.CurrentCulture))== 0){
					return col;
				}
			}
			
			return null;
		}
		
	}
	

	
	[Serializable()]
	public class ColumnCollection: List<AbstractColumn>{
		private System.Globalization.CultureInfo culture;
		
		public ColumnCollection(){
			culture = CultureInfo.CurrentCulture;
		
		}
		
		
		public AbstractColumn Find (string columnName) {
			for (int i = 0;i < this.Count ; i ++) {
				AbstractColumn col = (AbstractColumn)this[i];
				if (String.Compare(col.ColumnName.ToLower(CultureInfo.CurrentCulture),
				                   columnName.ToLower(CultureInfo.CurrentCulture))== 0){
					return col;
				}
			}
			return null;
		}
	
		/// <summary>
		/// The Culture is used for rirect String Comparison
		/// </summary>
		
		public CultureInfo Culture
		{
			get { return culture; }
			set 
			{
				if (value == null)
					value = CultureInfo.CurrentCulture;
				culture = value; 
			}
		}
	}
	
	
	public class AbstractParametersCollection: List<AbstractParameter>{
		public AbstractParameter Find (string parameterName) {
			for (int i = 0;i < this.Count ; i ++) {
				AbstractParameter par = this[i];
				if (String.Compare(par.ParameterName,parameterName)== 0){
					return par;
				}
			}
			return null;
		}
	}
	
}
