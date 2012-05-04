// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Description of ClassUnitTestNode.
	/// </summary>
	public class ClassUnitTestNode : UnitTestBaseNode
	{
		TestClass testClass;
		
		public TestClass TestClass {
			get { return testClass; }
		}
		
		public ClassUnitTestNode(TestClass testClass)
		{
			this.testClass = testClass;
			this.testClass.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
				if (e.PropertyName == "TestResult") {
					RaisePropertyChanged("Icon");
					var parentNode = Parent;
					while (parentNode is NamespaceUnitTestNode) {
						parentNode.RaisePropertyChanged("Icon");
						parentNode = parentNode.Parent;
					}
				}
			};
			testClass.Members.CollectionChanged += TestMembersCollectionChanged;
			testClass.NestedClasses.CollectionChanged += NestedClassesCollectionChanged;
			LazyLoading = true;
		}

		void NestedClassesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestClass c in e.NewItems) {
						Children.OrderedInsert(new ClassUnitTestNode(c), (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestClass c in e.OldItems) {
						Children.RemoveAll(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass.FullName == c.FullName);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}

		void TestMembersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestMember m in e.NewItems) {
						Children.OrderedInsert(new MemberUnitTestNode(m), (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestMember m in e.OldItems) {
						Children.RemoveAll(n => n is MemberUnitTestNode && ((MemberUnitTestNode)n).TestMember.Method.ReflectionName == m.Method.ReflectionName);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			foreach (TestClass c in testClass.NestedClasses)
				Children.Add(new ClassUnitTestNode(c));
			foreach (TestMember m in testClass.Members)
				Children.Add(new MemberUnitTestNode(m));
		}
		
		public override object Text {
			get { return testClass.Name; }
		}
		
		internal override TestResultType TestResultType {
			get { return testClass.TestResult; }
		}
	}
}
