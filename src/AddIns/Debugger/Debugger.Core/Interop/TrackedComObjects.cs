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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Debugger.Interop
{
	public static class TrackedComObjects
	{
		static List<WeakReference> objects = new List<WeakReference>();
		
		public static void ProcessOutParameter(object parameter)
		{
			if (parameter != null) {
				if (Marshal.IsComObject(parameter)) {
					Track(parameter);
				} else if (parameter is Array) {
					foreach(object elem in (Array)parameter) {
						ProcessOutParameter(elem);
					}
				}
			}
		}
		
		public static void Track(object obj)
		{
			if (Marshal.IsComObject(obj)) {
				lock(objects) {
					objects.Add(new WeakReference(obj));
				}
			}
		}
		
		public static int ReleaseAll()
		{
			lock(objects) {
				int count = 0;
				foreach(WeakReference weakRef in objects) {
					object obj = weakRef.Target;
					if (obj != null) {
						Marshal.FinalReleaseComObject(obj);
						count++;
					}
				}
				objects.Clear();
				objects.TrimExcess();
				return count;
			}
		}
	}
}

#pragma warning restore 1591
