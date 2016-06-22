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
using System.Collections.ObjectModel;

using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportModel.
	/// </summary>
	public class ReportModel :IReportModel
	{
		
		public ReportModel() {
			SectionCollection = new Collection<BaseSection>();
			ReportSettings = new ReportSettings();
		}
			
		#region Sections
		
		public IReportContainer ReportHeader {
			get {return (BaseSection)SectionCollection[0];}
		}
		
		
		public IReportContainer PageHeader {
			get {return (BaseSection)SectionCollection[1];}
		}
		
		
		public IReportContainer DetailSection {
			get {return (BaseSection)SectionCollection[2];}
		}
		
		
		public IReportContainer PageFooter {
			get {return (BaseSection)SectionCollection[3];}
		}
		
		
		public IReportContainer ReportFooter {
			get {return (BaseSection)SectionCollection[4];}
		}
		
		#endregion
		
		public IReportSettings ReportSettings{get;set;}
		
		public Collection<BaseSection> SectionCollection {get; private set;}
	}
}
