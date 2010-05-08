// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

/// <summary>
///
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.11.2005 13:43:22
/// </remarks>

namespace ICSharpCode.Reports.Core {
	/*
	public class GroupSeparator : BaseComparer,IHierarchyData {
		private string typeName = "GroupSeperator";
		
		int groupLevel;
		IndexList childs ;
		
		public GroupSeparator(SortColumnCollection owner, int listIndex, object[] values,int groupLevel):
			base(owner,listIndex,values) {
			this.groupLevel = groupLevel;
		}
		
		
		public int GroupLevel {
			get {
				return groupLevel;
			}
		}
		
		public IndexList GetChildren {
			get {
				if (this.childs == null) {
					this.childs = new IndexList("ChildList");
				}
				return childs;
				
			}
		}
		
		#region Comparer
		internal int CompareTo(BaseComparer value)
		{
			// we shouldn't get to this point
			if (value == null)
				throw new ArgumentNullException("value");
			
			if (value.ObjectArray.Length != base.ObjectArray.Length){
				string s = String.Format(CultureInfo.CurrentCulture,
				                         "{0} {1} {2}",
				                         this.GetType().ToString(),
				                         value.ObjectArray.Length,
				                         base.ObjectArray.Length);
				throw new ReportException(s);
			}
				
			
			int compare = 0;
			
			for (int index = 0; index < base.ObjectArray.Length; index++)
			{
				object leftValue = base.ObjectArray[index];
				object rightValue = value.ObjectArray[index];
				// Indizes sind hier deckungsgleich
				
				GroupColumn groupColumn = (GroupColumn)base.ColumnCollection[index];

				bool descending = (groupColumn.SortDirection == ListSortDirection.Descending);
				
				// null means equl
				if (leftValue == null || leftValue == System.DBNull.Value)
				{
					if (rightValue != null && rightValue != System.DBNull.Value)
					{
						return (descending) ? 1 : -1;
					}
					
					// Beide Null
					continue;
				}
				
				if (rightValue == null || rightValue == System.DBNull.Value)
				{
					return (descending) ? -1 : 1;
				}
				
				
				if (leftValue.GetType() != rightValue.GetType()){
					string s = String.Format(CultureInfo.CurrentCulture,
					                         "{0} {1} {2}",this.GetType().ToString(),
					                         leftValue.GetType().ToString(),
					                         rightValue.GetType().ToString());
					
					throw new ReportException(s);
				}
					
				
				if (leftValue.GetType() == typeof(string))
				{
					compare = String.Compare((string)leftValue, (string)rightValue,
					                         !groupColumn.CaseSensitive, base.ColumnCollection.Culture);
				}
				else
				{
					compare = ((IComparable)leftValue).CompareTo(rightValue);
				}
				
				// Sind ungleich, tauschen je nach Richtung
				if (compare != 0)
				{
					return (descending) ? -compare : compare;
				}
			}
			
			// Gleich Werte, dann Index bercksichtigen
			return this.ListIndex.CompareTo(value.ListIndex);
		}
		
		
		
		public override int CompareTo(object obj) {
			 base.CompareTo(obj);
			return this.CompareTo((BaseComparer)obj);
		}
		
		
		#endregion
		
		
		#region SharpReportCore.IHierarchyData interface implementation
		
		public virtual bool HasChildren {
			get {
				return (this.childs.Count > 0);
			}
			
			
		}
		
		public object Item {
			get {
				return this;
			}
		}
		
		public string Path {
			get {
				StringBuilder sb = new StringBuilder();
				foreach (object o in base.ObjectArray) {
					sb.Append(o.ToString() + ";");
				}
				sb.Remove(sb.Length -1,1);
				return sb.ToString();
			}
		}
		
		public string Type {
			get {
				return this.typeName;
			}
		}
		
//		public IHierarchicalEnumerable GetChildren() {
//			return this.childs;
//		}
//		
//		public IHierarchyData GetParent() {
//			return null;
//		}
	
		#endregion
		
//		public override string ToString (){
//			return this.typeName;
//		}
	}
	*/
}
