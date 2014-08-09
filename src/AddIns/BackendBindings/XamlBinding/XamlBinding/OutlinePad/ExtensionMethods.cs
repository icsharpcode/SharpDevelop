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
using ICSharpCode.NRefactory;
	
namespace ICSharpCode.XamlBinding
{	
	/// <summary>
	/// Description of TextLocationExtensions.
	/// </summary>
	static class TextLocationExtensions {

		public static bool IsInfinite(this TextLocation location) {
			return location == null || location.Line == int.MaxValue || location.Column == int.MaxValue;
		}

		public static bool IsValid(this TextLocation location) {
			if (location.IsEmpty)
				return false;
			if (location.IsInfinite())
				return false;			
			return true;
		}
		
		public static bool IsInside(this TextLocation location, TextLocation startLocation, TextLocation endLocation) {
			if (location.IsEmpty)
				return false;
			
			return location.Line >= startLocation.Line &&
				(location.Line <= endLocation.Line   || endLocation.Line == -1) &&
				(location.Line != startLocation.Line || location.Column >= startLocation.Column) &&
				(location.Line != endLocation.Line   || location.Column <= endLocation.Column);
		}		
	}
}
