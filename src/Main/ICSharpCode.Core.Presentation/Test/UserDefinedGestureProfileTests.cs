using System;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using NUnit.Framework;
using System.IO;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
	public class UserDefinedGestureProfileTests
    {
    	UserGesturesProfile profile = null;
    	InputBindingInfo binding = null;
    	KeyGesture gesture = null;
    	
    	[TestFixtureSetUp]
    	public void TestFixtureSetUp()
    	{
    		PropertyService.InitializeServiceForUnitTests();
    	}
    	
    	[SetUp]
    	public void SetuUp()
    	{
    		UserDefinedGesturesManager.CurrentProfile = null;
    		profile = new UserGesturesProfile { Name = "TestProfile1" };
        	gesture = (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString("Ctrl+A");
        	binding = new InputBindingInfo { OwnerTypeName="Binding", RoutedCommandName="Binding" };
    	}
    	
		[Test]
		public void NoProfileTest()
		{
			binding.DefaultGestures.Add(gesture);
			
			Assert.AreEqual(1, binding.DefaultGestures.Count);
			Assert.AreEqual(gesture, binding.DefaultGestures[0]);
			Assert.AreEqual(1, binding.ActiveGestures.Count);
			Assert.AreEqual(gesture, binding.ActiveGestures[0]);
		}
		
		[Test]
		public void ActiveProfileTest()
		{
			binding.DefaultGestures.Add(gesture);
			
			var userDefinedGestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString("Ctrl+B;Ctrl+C");
			profile[binding.Identifier] = userDefinedGestures;
			UserDefinedGesturesManager.CurrentProfile = profile;
			
			// Default gestures stay the same
			Assert.AreEqual(1, binding.DefaultGestures.Count);
			Assert.AreEqual(gesture, binding.DefaultGestures[0]);
			
			// Only active gestures are modified
			Assert.AreEqual(2, binding.ActiveGestures.Count);
			Assert.AreEqual(userDefinedGestures[0], binding.ActiveGestures[0]);
			Assert.AreEqual(userDefinedGestures[1], binding.ActiveGestures[1]);
		}
		
		[Test]
		public void NoGestureInActiveProfileTest()
		{
			var binding2 = new InputBindingInfo { OwnerTypeName="Binding2", RoutedCommandName="Binding2" };
			binding2.DefaultGestures.Add(gesture);
			
			var userDefinedGestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString("Ctrl+B;Ctrl+C");
			profile[binding.Identifier] = userDefinedGestures;
			UserDefinedGesturesManager.CurrentProfile = profile;
			
			// Default gestures are used because current profile doesn't contain modifications to this gesture
			Assert.AreEqual(1, binding2.ActiveGestures.Count);
			Assert.AreEqual(binding2.DefaultGestures[0], binding2.ActiveGestures[0]);
		}
    }
}
