// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of TextStyleDecorator.
	/// </summary>
	public class TextStyleDecorator:BaseStyleDecorator
	{

		public TextStyleDecorator():base()
		{
		}
		
		public Font Font {get;set;}
		
		public StringFormat StringFormat {get;set;}
		
		public StringTrimming StringTrimming {get;set;}
			
		public ContentAlignment ContentAlignment {get;set;}
			
		
		public string DataType {get;set;}
	
		public string FormatString {get;set;}
		
		public System.Windows.Forms.RightToLeft RightToLeft {get;set;}
	}
}
