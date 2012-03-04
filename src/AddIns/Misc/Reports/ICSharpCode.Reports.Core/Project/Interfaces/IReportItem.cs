// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
	

namespace ICSharpCode.Reports.Core {
	
	/// <summary>
	/// All ReportItems must implement this Interface
	/// </summary>
	public interface IReportItem 
	{
		
		string Name{get;set;}
		Size Size {get;set;}
		Point Location {get;set;}
		Font Font {get;set;}
//		bool VisibleInReport {get;set;}
		Color BackColor {get;set;}
		Color FrameColor {get;set;}
		int SectionOffset {get;set;}
		bool CanGrow {get;set;}
		bool CanShrink {get;set;} 
	}
}
