/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.04.2013
 * Time: 18:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of Baseexport.
	/// </summary>
	public class BaseExporter
	{
		public BaseExporter(Collection<IPage> pages)
		{
			if (pages == null) {
				throw new ArgumentException("pages");
			}
			
			Pages = pages;
		}
		
		public virtual void Run () {
			
		}


	    protected Collection<IPage> Pages {get; set;}
	}
}
