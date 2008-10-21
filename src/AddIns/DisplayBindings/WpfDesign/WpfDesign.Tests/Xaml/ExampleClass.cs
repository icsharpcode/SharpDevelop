// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3070 $</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.Xaml
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
