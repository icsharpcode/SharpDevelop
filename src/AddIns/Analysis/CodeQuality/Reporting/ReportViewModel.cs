/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.02.2012
 * Time: 18:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of ReportViewModel.
	/// </summary>
	internal class ReportViewModel
	{
		public ReportViewModel()
		{
		}
		
		public AssemblyNode Node {get;set;}
		
		
		public string Name
		{
			get {return Node.Name;}
		}
		
		
	}
}
