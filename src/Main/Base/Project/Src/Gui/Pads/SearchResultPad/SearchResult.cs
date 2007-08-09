// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2644$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SearchAndReplace
{
	public class SearchResult
	{
		public string Pattern { get; set; }
		public IList<SearchResultMatch> Results { get; set; }
		public Control SpecialPanel { get; set; }
		
		public SearchResult(string pattern, IList<SearchResultMatch> results)
		{
			if (results == null)
				throw new ArgumentNullException("results");
			this.Pattern = pattern;
			this.Results = results;
		}
		
		public SearchResult(string pattern, Control specialPanel)
		{
			if (specialPanel == null)
				throw new ArgumentNullException("specialPanel");
			this.Pattern = pattern;
			this.SpecialPanel = specialPanel;
		}
	}
}
