using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using NUnit.Framework;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.Core.Presentation.Tests
{
	/// <summary>
	/// Description of InputGestureTests.
	/// </summary>
	[TestFixture]
	public class InputGestureTest
	{		
		public MultiKeyGestureConverter multiKeyGestureConverter = new MultiKeyGestureConverter();
		public PartialKeyGestureConverter partialKeyGestureConverter = new PartialKeyGestureConverter();
		public KeyGestureConverter keyGestureConverter = new KeyGestureConverter();
		
		[Test]
		public void MultiKeyGesturesMatchExactlyTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var result1 = template1.IsTemplateFor(original1, GestureCompareMode.ExactlyMatches);
			Assert.IsTrue(result1);
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+V");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+V");
			var result2 = template2.IsTemplateFor(original2, GestureCompareMode.ExactlyMatches);
			Assert.IsTrue(result2);
		}
		
		[Test]
		public void MultiKeyGesturesDoNotMatchExactlyTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original1.IsTemplateFor(template1, GestureCompareMode.ExactlyMatches));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+V");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original2.IsTemplateFor(template2, GestureCompareMode.ExactlyMatches));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template3.IsTemplateFor(original3, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original3.IsTemplateFor(template3, GestureCompareMode.ExactlyMatches));
		}

		[Test]
		public void KeyGesturesMatchExactlyTest()
		{
			var template1 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original1 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsTrue(template1.IsTemplateFor(original1, GestureCompareMode.ExactlyMatches));
			Assert.IsTrue(original1.IsTemplateFor(template1, GestureCompareMode.ExactlyMatches));
			
			var template2 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsTrue(template2.IsTemplateFor(original2, GestureCompareMode.ExactlyMatches));
			Assert.IsTrue(original2.IsTemplateFor(template2, GestureCompareMode.ExactlyMatches));
			
			var template3 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			var original3 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			Assert.IsTrue(template3.IsTemplateFor(original3, GestureCompareMode.ExactlyMatches));
			Assert.IsTrue(original3.IsTemplateFor(template3, GestureCompareMode.ExactlyMatches));
			
			var template4 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("F");
			var original4 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("F");
			Assert.IsTrue(template4.IsTemplateFor(original4, GestureCompareMode.ExactlyMatches));
			Assert.IsTrue(original4.IsTemplateFor(template4, GestureCompareMode.ExactlyMatches));
			
			var template5 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift");
			var original5 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift");
			Assert.IsTrue(template5.IsTemplateFor(original5, GestureCompareMode.ExactlyMatches));
			Assert.IsTrue(original5.IsTemplateFor(template5, GestureCompareMode.ExactlyMatches));
		}
		

		[Test]
		public void KeyGesturesDoNotMatchExactlyTest()
		{
			var template1 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original1 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original1.IsTemplateFor(template1, GestureCompareMode.ExactlyMatches));
			
			var template2 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original2 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Alt+C");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original2.IsTemplateFor(template2, GestureCompareMode.ExactlyMatches));
			
			var template3 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+F");
			Assert.IsFalse(template3.IsTemplateFor(original3, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original3.IsTemplateFor(template3, GestureCompareMode.ExactlyMatches));
			
			var template4 = (KeyGesture)keyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			var original4 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsFalse(template4.IsTemplateFor(original4, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original4.IsTemplateFor(template4, GestureCompareMode.ExactlyMatches));
			
			var template5 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("F");
			var original5 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("D");
			Assert.IsFalse(template5.IsTemplateFor(original5, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original5.IsTemplateFor(template5, GestureCompareMode.ExactlyMatches));
			
			var template6 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift");
			var original6 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl");
			Assert.IsFalse(template6.IsTemplateFor(original6, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original6.IsTemplateFor(template6, GestureCompareMode.ExactlyMatches));
			
			var template7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,C");
			var original7 = (PartialKeyGesture)partialKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template7.IsTemplateFor(original7, GestureCompareMode.ExactlyMatches));
			Assert.IsFalse(original7.IsTemplateFor(template7, GestureCompareMode.ExactlyMatches));
		}
		
		[Test]
		public void MultiKeyGesturesMatchStartsWithTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsTrue(template1.IsTemplateFor(original1, GestureCompareMode.StartsWith));
			Assert.IsTrue(original1.IsTemplateFor(template1, GestureCompareMode.StartsWith));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsTrue(template2.IsTemplateFor(original2, GestureCompareMode.StartsWith));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsTrue(template3.IsTemplateFor(original3, GestureCompareMode.StartsWith));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsTrue(template4.IsTemplateFor(original4, GestureCompareMode.StartsWith));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsTrue(template5.IsTemplateFor(original5, GestureCompareMode.StartsWith));
		}
		
		[Test]
		public void MultiKeyGesturesDoNotMatchStartsWithTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.StartsWith));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.StartsWith));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+E");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template3.IsTemplateFor(original3, GestureCompareMode.StartsWith));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Shift+D");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template4.IsTemplateFor(original4, GestureCompareMode.StartsWith));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,E");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template5.IsTemplateFor(original5, GestureCompareMode.StartsWith));
			
			var template6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Alt+");
			var original6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template6.IsTemplateFor(original6, GestureCompareMode.StartsWith));
			
			var template7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("E");
			var original7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template7.IsTemplateFor(original7, GestureCompareMode.StartsWith));
			
			var template8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Alt+");
			var original8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsFalse(template8.IsTemplateFor(original8, GestureCompareMode.StartsWith));
		}
		
		[Test]
		public void MultiKeyGesturesMatchPartlyTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsTrue(template1.IsTemplateFor(original1, GestureCompareMode.PartlyMatches));
			Assert.IsTrue(original1.IsTemplateFor(template1, GestureCompareMode.PartlyMatches));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsTrue(template2.IsTemplateFor(original2, GestureCompareMode.PartlyMatches));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsTrue(template3.IsTemplateFor(original3, GestureCompareMode.PartlyMatches));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("D");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsTrue(template4.IsTemplateFor(original4, GestureCompareMode.PartlyMatches));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsTrue(template5.IsTemplateFor(original5, GestureCompareMode.PartlyMatches));
			
			var template6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("C,Ctrl+");
			var original6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsTrue(template6.IsTemplateFor(original6, GestureCompareMode.PartlyMatches));
			
			var template7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D,D");
			var original7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsTrue(template7.IsTemplateFor(original7, GestureCompareMode.PartlyMatches));
			
			var template8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+");
			var original8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsTrue(template8.IsTemplateFor(original8, GestureCompareMode.PartlyMatches));
		}
		
		[Test]
		public void MultiKeyGesturesDoNotMatchPartlyTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.PartlyMatches));
			Assert.IsFalse(original1.IsTemplateFor(template1, GestureCompareMode.PartlyMatches));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.PartlyMatches));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("E");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template4.IsTemplateFor(original4, GestureCompareMode.PartlyMatches));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Alt+");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template5.IsTemplateFor(original5, GestureCompareMode.PartlyMatches));
			
			var template6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("C,Ctrl+,D");
			var original6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template6.IsTemplateFor(original6, GestureCompareMode.PartlyMatches));
			
			var template7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Alt+D,D");
			var original7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template7.IsTemplateFor(original7, GestureCompareMode.PartlyMatches));
		}
		
		
		
		
		
		[Test]
		public void MultiKeyGesturesMatchStartsFullChordWithTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsTrue(template1.IsTemplateFor(original1, GestureCompareMode.StartsWithFullChords));
			Assert.IsTrue(original1.IsTemplateFor(template1, GestureCompareMode.StartsWithFullChords));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsTrue(template2.IsTemplateFor(original2, GestureCompareMode.StartsWithFullChords));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsTrue(template3.IsTemplateFor(original3, GestureCompareMode.StartsWithFullChords));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsTrue(template4.IsTemplateFor(original4, GestureCompareMode.StartsWithFullChords));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D,Ctrl+D");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D,Ctrl+D,Ctrl+E");
			Assert.IsTrue(template5.IsTemplateFor(original5, GestureCompareMode.StartsWithFullChords));
			
			var template6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D,D");
			var original6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D,D,E");
			Assert.IsTrue(template6.IsTemplateFor(original6, GestureCompareMode.StartsWithFullChords));
		}
		
		[Test]
		public void MultiKeyGesturesDoNotMatchStartsWithFullChordTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.StartsWith));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.StartsWith));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+E");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template3.IsTemplateFor(original3, GestureCompareMode.StartsWith));
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Shift+D");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template4.IsTemplateFor(original4, GestureCompareMode.StartsWith));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,E");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template5.IsTemplateFor(original5, GestureCompareMode.StartsWith));
			
			var template6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Alt+");
			var original6 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template6.IsTemplateFor(original6, GestureCompareMode.StartsWith));
			
			var template7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("E");
			var original7 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.IsFalse(template7.IsTemplateFor(original7, GestureCompareMode.StartsWith));
			
			var template8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Alt+");
			var original8 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsFalse(template8.IsTemplateFor(original8, GestureCompareMode.StartsWith));
		}

		[Test]
		public void MultiKeyGesturesConflictingTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			Assert.IsTrue(template1.IsTemplateFor(original1, GestureCompareMode.Conflicting));
			Assert.IsTrue(original1.IsTemplateFor(template1, GestureCompareMode.Conflicting));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsTrue(template2.IsTemplateFor(original2, GestureCompareMode.Conflicting));
			Assert.IsTrue(original2.IsTemplateFor(template2, GestureCompareMode.Conflicting));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsTrue(template3.IsTemplateFor(original3, GestureCompareMode.StartsWith));
			Assert.IsTrue(original3.IsTemplateFor(template3, GestureCompareMode.Conflicting));
		}		

		[Test]
		public void MultiKeyGesturesAreNotConflictingTest()
		{
			var template1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original1 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+E");
			Assert.IsFalse(template1.IsTemplateFor(original1, GestureCompareMode.Conflicting));
			Assert.IsFalse(original1.IsTemplateFor(template1, GestureCompareMode.Conflicting));
			
			var template2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+D");
			var original2 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D");
			Assert.IsFalse(template2.IsTemplateFor(original2, GestureCompareMode.Conflicting));
			Assert.IsFalse(original2.IsTemplateFor(template2, GestureCompareMode.Conflicting));
			
			var template3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+E");
			var original3 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+C,Ctrl+D,D");
			Assert.IsFalse(template3.IsTemplateFor(original3, GestureCompareMode.StartsWith));
			Assert.IsFalse(original3.IsTemplateFor(template3, GestureCompareMode.Conflicting));
			
			
			var template4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+");
			var original4 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,D");
			Assert.False(template4.IsTemplateFor(original4, GestureCompareMode.Conflicting));
			Assert.False(original4.IsTemplateFor(template4, GestureCompareMode.Conflicting));
			
			var template5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+");
			var original5 = (MultiKeyGesture)multiKeyGestureConverter.ConvertFromInvariantString("Ctrl+Shift+D,Ctrl+D");
			Assert.IsFalse(template5.IsTemplateFor(original5, GestureCompareMode.Conflicting));
			Assert.IsFalse(original5.IsTemplateFor(template5, GestureCompareMode.Conflicting));
		}
	}
}
