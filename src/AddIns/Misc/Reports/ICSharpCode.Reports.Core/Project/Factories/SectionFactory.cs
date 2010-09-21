// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
	
/// <summary>
/// Factory for ReportSections
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 01.09.2005 17:05:03
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	internal sealed class SectionFactory
	{
		private SectionFactory ()
		{
			
		}
		public static BaseSection Create(string name) {
			if (String.IsNullOrEmpty(name)) {
				String str = String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                           "<{0}>",name);
				throw new UnknownItemException(str);
			}
			return new BaseSection(name);
		}
		
	}
}
