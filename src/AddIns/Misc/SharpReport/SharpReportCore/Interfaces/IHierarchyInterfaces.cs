/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 30.11.2005
 * Time: 13:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
//ToDo with NET2.0 change this Interface to the one provided by NET2.0
using System;

namespace SharpReportCore {
	
	public interface IHierarchyData{
		
		SharpIndexCollection GetChildren {
			get;
		}

		bool HasChildren {
			get;
		}
		
		object Item {
			get;
		}
		string Path {
			get;
		}
		
		string Type {
			get;
		}
	}
}
