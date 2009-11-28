// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockWorkbenchTests
	{
		MockWorkbench workbench;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
		}
		
		[Test]
		public void ViewPassedToShowViewMethodIsAddedToViewContentsCollection()
		{
			MockViewContent view = new MockViewContent();
			workbench.ShowView(view);
			Assert.IsTrue(workbench.ViewContentCollection.Contains(view));
		}
		
		[Test]
		public void ActiveContentIsSaved()
		{
			MockViewContent view = new MockViewContent();
			workbench.ActiveContent = view;
			Assert.AreSame(view, workbench.ActiveContent);
		}
		
		[Test]
		public void ActiveViewContentIsSaved()
		{
			MockViewContent view = new MockViewContent();
			workbench.ActiveViewContent = view;
			Assert.AreSame(view, workbench.ActiveViewContent);
		}
		
		[Test]
		public void RaiseActiveViewContentChangedMethodFiresActiveViewContentChangedEvent()
		{
			bool eventFired = false;
			workbench.ActiveViewContentChanged += delegate (object source, EventArgs e)
				{ eventFired = true; };
			
			workbench.RaiseActiveViewContentChangedEvent();
			
			Assert.IsTrue(eventFired);
		}
		
		[Test]
		public void RaiseActiveViewContentChangedMethodDoesNotThrowNullReferenceExceptionWhenNoEventHandlersConfigured()
		{
			Assert.DoesNotThrow(delegate { workbench.RaiseActiveViewContentChangedEvent(); });
		}
	}
}
