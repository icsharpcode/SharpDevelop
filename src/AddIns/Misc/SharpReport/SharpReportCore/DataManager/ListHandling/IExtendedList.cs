/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 04.08.2005
 * Time: 09:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 using System;
 using System.Collections;
 
 namespace SharpReportCore {
 	
 	
 	public interface IExtendedList{
 		
 		///<summary>
 		/// Build the Hash/IndexList we need for indexing the DataSource
 		/// </summary>
 		void BuildHashList (IList list);
 		
 		/// <summary>
 		/// This List is used as an Index to the DataSource
 		/// ListItems are added by derived Classes
 		/// </summary>
 		IList IndexList{
 			get;
 		}
 		
 		/// <summary>
 		/// returns the Name of the <see cref="ExtendedListArray"></see>
 		/// </summary>
 		 string Name {
 			get;
 		}
 		
 		///<summary>
 		/// CurrentPosition in <see cref="SharpArrayList"></see>
 		/// </summary>
 		int CurrentPosition {
 			get;set;
 		}
 	}
 }
 
