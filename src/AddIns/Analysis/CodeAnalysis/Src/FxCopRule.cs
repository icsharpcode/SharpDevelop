// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
