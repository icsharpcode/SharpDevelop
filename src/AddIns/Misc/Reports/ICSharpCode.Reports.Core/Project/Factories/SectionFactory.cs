// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
