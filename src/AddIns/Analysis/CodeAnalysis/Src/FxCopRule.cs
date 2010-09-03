// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeAnalysis
{
	[Serializable]
	public class FxCopRule : IComparable
	{
		readonly string checkId;
		readonly string displayName;
		readonly string categoryName;
		readonly string description;
		readonly string url;
		
		public FxCopRule(string checkId, string displayName, string categoryName, string description, string url)
		{
			this.checkId = checkId;
			this.displayName = displayName;
			this.categoryName = categoryName;
			this.description = description;
			this.url = url;
		}
		
		public string CheckId {
			get {
				return checkId;
			}
		}
		
		public string DisplayName {
			get {
				return displayName;
			}
		}
		
		public string CategoryName {
			get {
				return categoryName;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Url {
			get {
				return url;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[FxCopRule {0}#{1}]", this.categoryName, this.checkId);
		}
		
		public int CompareTo(object obj)
		{
			FxCopRule o = (FxCopRule)obj;
			int r = categoryName.CompareTo(o.categoryName);
			if (r != 0) return r;
			r = checkId.CompareTo(o.checkId);
			if (r != 0) return r;
			return displayName.CompareTo(o.displayName);
		}
	}
	
	[Serializable]
	public class FxCopCategory
	{
		readonly string name;
		readonly List<FxCopRule> rules = new List<FxCopRule>();
		
		public FxCopCategory(string name)
		{
			this.name = name;
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string DisplayName {
			get {
				return name;
			}
		}
		
		public List<FxCopRule> Rules {
			get {
				return rules;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[FxCopCategory {0}]", this.name);
		}
	}
}
