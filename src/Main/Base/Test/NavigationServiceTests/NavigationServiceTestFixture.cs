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
using NUnit.Framework;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Tests.NavigationServiceTests
{
	/// <summary>
	/// Provides unit tests for the <see cref="NavigationService"/>.
	/// </summary>
	/// <remarks>
	/// <para>Code Navigation Requirements:</para>
	/// <list type="definition">
	///   <term><see href="http://wiki.sharpdevelop.net/default.aspx/SharpDevelop.SharpDevelop21Features">#develop wiki</see>:</term>
	///   <description>
	///     <list type="bullet">
	/// 	  <item>backwards & forwards through visited positions in the editor windows</item>
	///     </list>
	///   </description>
	///   <term>Daniel Grunwald [sharpdevelop-contributors@lists.sourceforge.net]:</term>
	///   <description>
	///     <list type="bullet">
	///       <item>includes back/forward command to reset the cursor position/active file</item>
	///       <item>saves class+method+line info of the cursor</item>
	///       <item>"undo" cursor position changes</item>
	///       <item>when adding a cursor position to the history, all other
	///             cursor positions in the same method are removed</item>
	///       <item>the "navigate back" command would be a SplitButton in the toolbar</item>
	///       <item>the dropdown would show class+method names</item>
	///     </list>
	///   </description>
	/// </list>
	/// <para> The <see cref="NavigationService.Count">Count</see> and list of
	/// points returned by <see cref="NavigationService.GetListOfPoints">
	/// GetListOfPoints()</see> are both assumed to contain the
	/// <see cref="NavigationService.CurrentPosition">CurrentPosition</see>.
	/// </para>
	/// <para>As the current position is also exposed a property, clients can
	/// then make the choice to include or exclude it when iterating over the
	/// list of points.</para>
	/// </remarks>
	[TestFixture]
	public class NavigationServiceTestFixture
	{
		#region private members
		INavigationPoint p;
		INavigationPoint p_5;
		INavigationPoint p_6;
		INavigationPoint p_37;
		INavigationPoint q;
		INavigationPoint r;
		INavigationPoint s;
		INavigationPoint t;
		#endregion
		
		#region the tests
		
		// TODO: build "CreateNUnitTest" command
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose an <see cref="INavigationPoint"/>
		/// reference for the current "position" in the
		/// editor windows, and return <see langword="null"/>
		/// when the history is empty.
		/// </summary>
		public void CurrentPositionTest()
		{
			Assert.IsNull(NavigationService.CurrentPosition);
			
			NavigationService.CurrentPosition = p;
			Assert.IsInstanceOf(typeof(INavigationPoint),
			                        NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// be able to log and remember changes to the
		/// <see cref="NavigationService.CurrentPosition">
		/// CurrentPosition</see>.
		/// </summary>
		public void LogTest()
		{
			NavigationService.CurrentPosition = p;
			Assert.AreEqual(p, NavigationService.CurrentPosition);
			
			NavigationService.CurrentPosition = q;
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			
			NavigationService.CurrentPosition = r;
			Assert.AreEqual(r, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a list of points in it's history:
		/// <see cref="ICollection<T>"/> of type
		/// <see cref="INavigationPoint"/>.
		/// </summary>
		/// <remarks>necessary for testing and for menu building</remarks>
		public void GetListOfPointsTest()
		{
			Assert.IsInstanceOf(typeof(ICollection<INavigationPoint>),
			                        NavigationService.Points);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a method to empty the points in its
		/// history.
		/// </summary>
		public void ClearHistoryTest()
		{
			NavigationService.ClearHistory(true);

			ICollection<INavigationPoint> history = NavigationService.Points;
			Assert.AreEqual(0, history.Count);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a property of type <see cref="int"/>
		/// indicating how many points are currently
		/// stored in it's history.
		/// </summary>
		public void CountTest()
		{
			Assert.IsInstanceOf(typeof(int),
			                        NavigationService.Count);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a property of type <see cref="int"/>
		/// indicating how many points are currently
		/// stored in it's history.
		/// </summary>
		public void CountingTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.CurrentPosition = p;
			Assert.AreEqual(1, NavigationService.Count);
			
			NavigationService.CurrentPosition = q;
			Assert.AreEqual(2, NavigationService.Count);
			
			NavigationService.CurrentPosition = r;
			Assert.AreEqual(3, NavigationService.Count);

			NavigationService.ClearHistory(true);
			Assert.AreEqual(0, NavigationService.Count);
			
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// ignore requests to log a null reference.
		/// </summary>
		public void LogNullTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			NavigationService.CurrentPosition = null;
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.CurrentPosition = p;
			NavigationService.CurrentPosition = null;
			Assert.AreEqual(p, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose methods that allow clients to
		/// suspend and resume logging of positions.
		/// </summary>
		public void SuspendLoggingTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.SuspendLogging();
			NavigationService.CurrentPosition = p;
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.ResumeLogging();
			NavigationService.CurrentPosition = q;
			Assert.AreEqual(q, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a Property to access the state of
		/// logging.
		/// </summary>
		public void IsLoggingTest()
		{
			Assert.IsTrue(NavigationService.IsLogging);
			
			NavigationService.SuspendLogging();
			Assert.IsFalse(NavigationService.IsLogging);
			
			NavigationService.ResumeLogging();
			Assert.IsTrue(NavigationService.IsLogging);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// <list type="bullet>
		///   <item>include a back/forward command to reset the 
		///         cursor position/active file</item>
		///   <item>be able to "undo" cursor position changes</item>
		/// </list>
		/// </summary>
		public void GoTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			Assert.IsNull(NavigationService.CurrentPosition);

			TestNavigationPoint.CurrentTestPosition = null;
			Assert.IsNull(TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.CurrentPosition = p;
			NavigationService.CurrentPosition = q;
			NavigationService.CurrentPosition = TestNavigationPoint.CurrentTestPosition = r;
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(0);
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(-1);
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);			
			Assert.AreEqual(q, TestNavigationPoint.CurrentTestPosition);

			NavigationService.Go(1);
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);			
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(-2);
			Assert.AreEqual(p, NavigationService.CurrentPosition);			
			Assert.AreEqual(p, TestNavigationPoint.CurrentTestPosition);

			NavigationService.Go(1);
			Assert.AreEqual(q, NavigationService.CurrentPosition);			
			Assert.AreEqual(q, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(1);
			Assert.AreEqual(r, NavigationService.CurrentPosition);			
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);

			// now attempt to go beyond the bounds of the list;
			// we should meet this request as best as possible
			// and generate no errors.
			NavigationService.Go(-5);
			Assert.AreEqual(p, NavigationService.CurrentPosition);			
			Assert.AreEqual(p, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(7);
			Assert.AreEqual(r, NavigationService.CurrentPosition);			
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// support navigating directly to a specific 
		/// point.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When the target point is not in the history,
		/// the <see cref="NavigationService"/> must
		/// add it such that advancing back restores the
		/// position that was current before this request
		/// and advancing forwards returns the same results
		/// had this request never been made.
		/// </para>
		/// </remarks>
		public void GoDirectTest()
		{
			Assert.AreEqual(0, NavigationService.Count);

			NavigationService.Log(p);
			NavigationService.Log(q);
			NavigationService.Log(r);
			TestNavigationPoint.CurrentTestPosition = r;
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
			Assert.AreEqual(r, TestNavigationPoint.CurrentTestPosition);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsFalse(NavigationService.CanNavigateForwards);
			
			NavigationService.Go(p);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(p, NavigationService.CurrentPosition);
			Assert.AreEqual(p, TestNavigationPoint.CurrentTestPosition);
			Assert.IsFalse(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);			
			
			NavigationService.Go(s);
			
			Assert.AreEqual(4, NavigationService.Count);
			Assert.AreEqual(s, NavigationService.CurrentPosition);
			Assert.AreEqual(s, TestNavigationPoint.CurrentTestPosition);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);			
			
			NavigationService.Go(-1);
			Assert.AreEqual(p, NavigationService.CurrentPosition);
			Assert.AreEqual(p, TestNavigationPoint.CurrentTestPosition);
			
			NavigationService.Go(2);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			Assert.AreEqual(q, TestNavigationPoint.CurrentTestPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose boolean properties indicating whether
		/// or not it is possible to navigate forwards
		/// or backwards within the history.
		/// </summary>
		public void CanNavigateTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			Assert.IsFalse(NavigationService.CanNavigateBack);
			Assert.IsFalse(NavigationService.CanNavigateForwards);
			
			NavigationService.Log(p);
			Assert.IsFalse(NavigationService.CanNavigateBack);
			Assert.IsFalse(NavigationService.CanNavigateForwards);
			
			NavigationService.Log(q);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsFalse(NavigationService.CanNavigateForwards);
			
			NavigationService.Go(-1);
			Assert.IsFalse(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// ignore multiple successive requests to
		/// log identical points.
		/// </summary>
		public void LogMultipleIdenticalSuccessiveTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.Log(p);
			NavigationService.Log(p);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(p, NavigationService.CurrentPosition);
			
			NavigationService.Log(q);
			NavigationService.Log(r);
			NavigationService.Log(r);
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// support multiple successive requests to
		/// log equivalent points, where equivalency 
		/// is measured by INavigationPoint.Equals(), 
		/// in which case the new point replaces the 
		/// current point.
		/// </summary>
		public void LogMultipleEquivalentSuccessiveTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			Assert.IsTrue(p_5.Equals(p_6));
			
			NavigationService.Log(p_5);
			NavigationService.Log(p_5);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(p_5, NavigationService.CurrentPosition);
			
			NavigationService.Log(p_6);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(p_6, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// allow requests to log multiple instances of
		/// the same position (where sameness is tested
		/// by INavigationPoint.<see cref="INavigationPoint.Equals">
		/// Equals()</see>), provided they are not logged 
		/// successively.
		/// </summary>
		public void LogMultipleEquivalentDisjointTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			Assert.IsTrue(p_5.Equals(p_6));
			
			NavigationService.Log(p_5);
			NavigationService.Log(p_5);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(p_5, NavigationService.CurrentPosition);
			
			NavigationService.Log(q);
			
			NavigationService.Log(p_6);
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(p_6, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose an event that fires when the
		/// navigation list has changed
		/// </summary>
		public void HistoryChangedTest()
		{
			navigationChangedCount = 0;
			NavigationService.HistoryChanged += NavigationHistoryChanged;

			Assert.AreEqual(0, NavigationService.Count);
			Assert.AreEqual(0, navigationChangedCount);
			
			NavigationService.Log(p);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(1, navigationChangedCount);
			
			NavigationService.Log(p_5);
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(2, navigationChangedCount);
			
			NavigationService.Log(q);
			Assert.AreEqual(2, NavigationService.Count);
			Assert.AreEqual(3, navigationChangedCount);
			
			NavigationService.ClearHistory(true);
			Assert.AreEqual(0, NavigationService.Count);
			Assert.AreEqual(4, navigationChangedCount);
			
			NavigationService.HistoryChanged -= NavigationHistoryChanged;
		}
		
		int navigationChangedCount;
		
		public void NavigationHistoryChanged(object sender, EventArgs e)
		{
			navigationChangedCount++;
		}
		#endregion

		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// expose a method to empty the points in its
		/// history without forgetting the current
		/// position
		/// </summary>
		public void ClearHistoryRetainCurrentTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.Log(p);
			NavigationService.Log(q);
			
			Assert.AreEqual(2, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			NavigationService.ClearHistory();
			
			Assert.AreEqual(1, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
		}
		
		[Test]
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// ignore requests to go directly to a null
		/// position.
		/// </summary>
		public void GoDirectToNullTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.Log(p);
			NavigationService.Log(q);
			NavigationService.Log(r);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
			
			NavigationService.Go(null);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(r, NavigationService.CurrentPosition);
		}
		
		/*
		[Test]
		[Ignore] // this test disabled on purpose - DA
		/// <summary>
		/// The <see cref="NavigationService"/> must
		/// ignore requests to log a point equivalent
		/// to the next position either forwards or back.
		/// </summary>
		public void LogDuplicateNextTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			NavigationService.Log(p);
			NavigationService.Log(q);
			NavigationService.Log(r);
			
			NavigationService.Go(-1);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);
			
			NavigationService.Log(p);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);
			
			NavigationService.Log(r);
			
			Assert.AreEqual(3, NavigationService.Count);
			Assert.AreEqual(q, NavigationService.CurrentPosition);
			Assert.IsTrue(NavigationService.CanNavigateBack);
			Assert.IsTrue(NavigationService.CanNavigateForwards);
		}
		*/
		
		[Test]
		/// <summary>
		/// Addresses <see cref="http://bugtracker.sharpdevelop.net/Default.aspx?p=4&i=939">SD2-939</see>: 
		/// Code Navigate Back dropdown first menu has a blank filename
		/// </summary>
		public void IgnoreBlankFilenamesTest()
		{
			Assert.AreEqual(0, NavigationService.Count);
			
			INavigationPoint testPoint = new TestNavigationPoint(String.Empty);
			Assert.AreEqual(String.Empty, testPoint.FileName);
			
			NavigationService.Log(testPoint);
			Assert.AreEqual(0, NavigationService.Count);
		}
		
		#region setup / tear down
		
		[SetUp]
		public void BeforeEachTest()
		{
			NavigationService.ClearHistory(true);
			NavigationService.ResumeLogging();
		}
		
		[TearDown]
		public void AfterEachTest()
		{
			NavigationService.ClearHistory(true);
			NavigationService.ResumeLogging();
		}
		
		/// <summary>
		/// Create several <see cref="TestNavigationPoint"/>s
		/// for use during tests.
		/// </summary>
		[TestFixtureSetUp]
		public void Init()
		{
			p = new TestNavigationPoint("p.cs");
			p_5 = new TestNavigationPoint("p.cs", 5);
			p_6 = new TestNavigationPoint("p.cs", 6);
			p_37 = new TestNavigationPoint("p.cs", 37);
			q = new TestNavigationPoint("q.cs");
			r = new TestNavigationPoint("r.cs");
			s = new TestNavigationPoint("s.cs");
			t = new TestNavigationPoint("t.cs");
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		#endregion
	}
}
