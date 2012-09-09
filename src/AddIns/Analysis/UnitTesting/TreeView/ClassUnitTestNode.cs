// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.ComponentModel;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a TestClass in the tree view.
	/// </summary>
	public class ClassUnitTestNode : UnitTestNode
	{
		TestClass testClass;
		
		public TestClass TestClass {
			get { return testClass; }
		}
		
		public ClassUnitTestNode(TestClass testClass)
		{
			this.testClass = testClass;
			this.testClass.TestResultChanged += delegate {
				RaisePropertyChanged("Icon");
				RaisePropertyChanged("ExpandedIcon");
				var parentNode = Parent;
				while (parentNode is NamespaceUnitTestNode) {
					parentNode.RaisePropertyChanged("Icon");
					parentNode.RaisePropertyChanged("ExpandedIcon");
					parentNode = parentNode.Parent;
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
						Children.OrderedInsert(new ClassUnitTestNode(c), NodeTextComparer);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestClass c in e.OldItems) {
						Children.RemoveAll(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass == c);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
				default:
					throw new NotSupportedException();
			}
		}

		void TestMembersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestMember m in e.NewItems) {
						Children.OrderedInsert(new MemberUnitTestNode(m), NodeTextComparer);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestMember m in e.OldItems) {
						Children.RemoveAll(n => n is MemberUnitTestNode && ((MemberUnitTestNode)n).TestMember == m);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
				default:
					throw new NotSupportedException();
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
