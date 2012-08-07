// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public static class XElementExtensions
	{
		public static void SetIsGetterAttributeValue(this XElement element, object value)
		{
			element.SetAttributeValue("isGetter", value);
		}
		
		public static void SetIsSetterAttributeValue(this XElement element, object value)
		{
			element.SetAttributeValue("isSetter", value);
		}
	}
}
