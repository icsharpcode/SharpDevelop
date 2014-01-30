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
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[ContentProperty("StringProp")]
	public class ExampleClass : ISupportInitialize
	{
		internal static int nextUniqueIndex;
		
		string stringProp, otherProp, otherProp2;
		int uniqueIndex = nextUniqueIndex++;
		
		public ExampleClass()
		{
			TestHelperLog.Log("ctor" + Identity);
		}
		
		protected string Identity {
			get {
				return GetType().Name + " (" + uniqueIndex + ")";
			}
		}
		
		void ISupportInitialize.BeginInit()
		{
			TestHelperLog.Log("BeginInit " + Identity);
		}
		
		void ISupportInitialize.EndInit()
		{
			TestHelperLog.Log("EndInit " + Identity);
		}
		
		public string StringProp {
			get {
				TestHelperLog.Log("StringProp.get " + Identity);
				return stringProp;
			}
			set {
				TestHelperLog.Log("StringProp.set to " + value + " - " + Identity);
				stringProp = value;
			}
		}
		
		public string OtherProp {
			get {
				TestHelperLog.Log("OtherProp.get " + Identity);
				return otherProp;
			}
			set {
				TestHelperLog.Log("OtherProp.set to " + value + " - " + Identity);
				otherProp = value;
			}
		}
		
		public string OtherProp2 {
			get {
				TestHelperLog.Log("OtherProp2.get " + Identity);
				return otherProp2;
			}
			set {
				TestHelperLog.Log("OtherProp2.set to " + value + " - " + Identity);
				otherProp2 = value;
			}
		}
		
		object objectProp;
		
		public object ObjectProp {
			get {
				TestHelperLog.Log("ObjectProp.get " + Identity);
				return objectProp;
			}
			set {
				TestHelperLog.Log("ObjectProp.set to " + value + " - " + Identity);
				objectProp = value;
			}
		}
	}
}
