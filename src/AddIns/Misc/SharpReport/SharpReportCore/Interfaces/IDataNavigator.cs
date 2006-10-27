/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 08.10.2005
 * Time: 17:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

 namespace SharpReportCore {
 	public interface IDataNavigator {
 		
 		void Fill (ReportItemCollection collection);
 		bool MoveNext () ;
 		void Reset();
 		 
 		bool HasMoreData {
 			get;
 		}
 		
 		bool HasChilds {
			get;
 		}
 		
 		int CurrentRow  {
 			get;
 		}
 		
 		int Count  {
			get;
		}
 		
 		object Current {
 			get;
 		}
 		
 		event EventHandler <ListChangedEventArgs> ListChanged;
 	}
 }
