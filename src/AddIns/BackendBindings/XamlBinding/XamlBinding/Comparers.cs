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
