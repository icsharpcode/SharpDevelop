// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.XamlBinding
{
	sealed class ParameterComparer : IEqualityComparer<IParameter> {
		public bool Equals(IParameter x, IParameter y)
		{
			return (x.Type.ReflectionName == y.Type.ReflectionName)
				&& (x.IsOut == y.IsOut)
				&& (x.IsParams == y.IsParams)
				&& (x.IsRef == y.IsRef)
				&& (x.IsOptional == y.IsOptional);
		}
		
		public int GetHashCode(IParameter obj)
		{
			return obj.GetHashCode();
		}
	}
	
	sealed class XmlnsEqualityComparer : IEqualityComparer<XmlnsCompletionItem> {
		public bool Equals(XmlnsCompletionItem x, XmlnsCompletionItem y)
		{
			return x.Namespace == y.Namespace && x.Assembly == y.Assembly;
		}
		
		public int GetHashCode(XmlnsCompletionItem obj)
		{
			return string.IsNullOrEmpty(obj.Assembly) ? obj.Namespace.GetHashCode() : obj.Namespace.GetHashCode() ^ obj.Assembly.GetHashCode();
		}
	}
	
	sealed class XmlnsComparer : IComparer<XmlnsCompletionItem> {
		public int Compare(XmlnsCompletionItem x, XmlnsCompletionItem y)
		{
			if (x.IsUrl && y.IsUrl)
				return string.CompareOrdinal(x.Namespace, y.Namespace);
			if (x.IsUrl)
				return -1;
			if (y.IsUrl)
				return 1;
			if (x.Assembly == y.Assembly)
				return string.CompareOrdinal(x.Namespace, y.Namespace);
			else
				return string.CompareOrdinal(x.Assembly, y.Assembly);
		}
	}
}
