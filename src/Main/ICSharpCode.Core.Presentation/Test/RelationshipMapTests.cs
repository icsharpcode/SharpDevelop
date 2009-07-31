using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using NUnit.Framework;

namespace ICSharpCode.Core.Presentation.Tests
{
    [TestFixture]
    public class RelationshipMapTests
    {
        [Test]
        public void WeakReferenceSupportTest()
        {
        	var map = new RelationshipMap<string, WeakReference>();
        	var uiElement = new UIElement();
        	var container = new WeakReference(uiElement);
        	map.Add("test", container);
        	
			uiElement = null;
			GC.Collect();
        	
			Assert.IsNull(map.MapForward("test").First().Target);
			Assert.AreEqual(1, map.MapBackward(container).Count);
        }
        
        [Test]
        public void WeakEqualityComparerSupportTest()
        {
        	var map = new RelationshipMap<string, WeakReference>(null, new WeakReferenceEqualirtyComparer());
        	var uiElement = new UIElement();
        	var container = new WeakReference(uiElement);
        	map.Add("test", container);
        	
			Assert.AreEqual(1, map.MapForward("test").Count);
			Assert.AreEqual(1, map.MapBackward(new WeakReference(uiElement)).Count);
        	
        	map.Remove("test", new WeakReference(uiElement));
        	
			Assert.AreEqual(0, map.MapForward("test").Count);
			Assert.AreEqual(0, map.MapBackward(new WeakReference(uiElement)).Count);
        }
    }
}
