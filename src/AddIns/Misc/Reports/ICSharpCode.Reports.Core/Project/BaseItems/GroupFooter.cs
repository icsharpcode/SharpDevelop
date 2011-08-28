/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.11.2010
 * Time: 19:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of BaseGroupeFooter.
	/// </summary>
	public class GroupFooter:BaseRowItem
	{
		
		public GroupFooter()
		{
		}

        public bool PageBreakOnGroupChange { get; set; }
	}
}
