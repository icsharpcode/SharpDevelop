// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace SearchAndReplace
{
	public delegate void SearchAllFinishedEventHandler(object sender, SearchAllFinishedEventArgs e);
	
	public class SearchAllFinishedEventArgs : EventArgs
	{
		string pattern;
		List<SearchResult> results;
		
		public string Pattern {
			get {
				return pattern;
			}
			set {
				pattern = value;
			}
		}
		
		public List<SearchResult> Results {
			get {
				return results;
			}
			set {
				results = value;
			}
		}
		
		public SearchAllFinishedEventArgs(string pattern, List<SearchResult> results)
		{
			this.pattern = pattern;
			this.results = results;
		}
	}
}
