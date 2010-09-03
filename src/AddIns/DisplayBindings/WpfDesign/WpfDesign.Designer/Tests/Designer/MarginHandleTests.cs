// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class MarginHandleTests : ModelTestHelper
	{
		private DesignItem _button;

        [TestFixtureSetUp]
        public void Intialize()
        {
            _button = CreateGridContext("<Button Margin=\"21,21,21,21\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\"/>");
            var gridItem = _button.Parent;
            gridItem.Properties[FrameworkElement.HeightProperty].SetValue("200");
            gridItem.Properties[FrameworkElement.WidthProperty].SetValue("200");
        }
        
        [Test]
        public void HandleLength()
        {
            Intialize();
            var handle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Left);
            Assert.AreEqual(21, handle.HandleLength);
            Assert.IsTrue(handle.Visibility == Visibility.Visible, "Handle is not visible");
            Assert.IsTrue(handle.Stub.Visibility == Visibility.Hidden, "Stub is Visible");
        }

        [Test]
        public void MoveTest()
        {
            Intialize();
            var handle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Left);
            var grid = _button.Parent.View;
            grid.Measure(grid.DesiredSize);
            grid.Arrange(new Rect(grid.DesiredSize));
            grid.UpdateLayout();
            
            PlacementTests.Move(new Vector(20, 20), _button);
            Assert.AreEqual(41,handle.HandleLength);
            Assert.IsTrue(handle.Visibility == Visibility.Visible, "Handle is not visible");
            Assert.IsTrue(handle.Stub.Visibility == Visibility.Hidden, "Stub is Visible");
        }

        [Test]
        [TestCase(20,20)]
        [TestCase(20,400)]
        [TestCase(400,20)]
        public void ResizeTest(double width,double height)
        {
            var handle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Left);
            var grid = _button.Parent.View;
            grid.Measure(grid.DesiredSize);
            grid.Arrange(new Rect(grid.DesiredSize));
            grid.UpdateLayout();

            PlacementTests.Resize(new Rect(20, 20, width, height), _button);
            Assert.AreEqual(20, handle.HandleLength);
            Assert.IsTrue(handle.Visibility == Visibility.Visible, "Handle is not visible");
            Assert.IsTrue(handle.Stub.Visibility == Visibility.Hidden, "Stub is Visible");
        }

        [Test]
        public void ChangeHorizontalAlignment()
        {
            var leftHandle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Left);
            var rightHandle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Right);
            _button.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Right);

            Assert.IsFalse(leftHandle.Visibility == Visibility.Visible, "Left Handle is visible");
            Assert.IsFalse(leftHandle.Stub.Visibility == Visibility.Hidden, "Left Stub is not Visible");

            Assert.IsTrue(rightHandle.Visibility == Visibility.Visible, "Right Handle is not visible");
            Assert.IsTrue(rightHandle.Stub.Visibility == Visibility.Hidden, "Right Stub is Visible");            
        }

        [Test]
        public void ChangeVerticalAlignment()
        {
            var topHandle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Top);
            var bottomHandle = new MarginHandle(_button, new AdornerPanel(), HandleOrientation.Bottom);
            _button.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Bottom);

            Assert.IsFalse(topHandle.Visibility == Visibility.Visible, "Top Handle is visible");
            Assert.IsFalse(topHandle.Stub.Visibility == Visibility.Hidden, "Top Stub is not Visible");

            Assert.IsTrue(bottomHandle.Visibility == Visibility.Visible, "Bottom Handle is not visible");
            Assert.IsTrue(bottomHandle.Stub.Visibility == Visibility.Hidden, "Bottom Stub is Visible");
        }
	}
}
