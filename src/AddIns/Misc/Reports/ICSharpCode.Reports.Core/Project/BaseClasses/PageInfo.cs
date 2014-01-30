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
using System.Collections;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses
{
	/// <summary>
	/// Description of PageInfo.
	/// </summary>
	public class PageInfo:IPageInfo
	{
		
		private Hashtable parameterHash;
		
		
		public PageInfo(int pageNumber)
		{
			this.PageNumber = pageNumber;
		}
		
		public int PageNumber {get;set;}
			
		
		public int TotalPages {get;set;}
	
		
		public string ReportName {get;set;}
			
		
		public string ReportFileName {get;set;}
		
	
		public string ReportFolder {
			get{
				return System.IO.Path.GetDirectoryName(this.ReportFileName);
			}
		}
		
			
		
		public DateTime ExecutionTime {get;set;}
			
		
		public Hashtable ParameterHash{
		get{
				if (this.parameterHash == null) {
					this.parameterHash  = new Hashtable();
				}
				return parameterHash;
			}
			set {this.parameterHash = value;}
		}
		
		
		public IDataNavigator IDataNavigator {get;set;}
	}
}
