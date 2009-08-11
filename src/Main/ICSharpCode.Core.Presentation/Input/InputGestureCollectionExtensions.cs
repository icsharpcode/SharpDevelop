using System;
using System.Windows.Input;
using System.Linq;
using System.Collections;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Contains utility methods for work with <see cref="InputGestureCollection" />
	/// </summary>
	public static class InputGestureCollectionExtensions
	{
		/// <summary>
		/// Determines whether any item from first <see cref="InputGestureCollection" /> is qualified as template for
		/// any item from second <see cref="InputGestureCollection" /> when examining using specific <see cref="GestureCompareMode" /> 
		/// </summary>
		/// <param name="inputGestureTemplateCollection">Collection of examined templates</param>
		/// <param name="testedInputGestureCollection">Collection of examined gestures</param>
		/// <param name="mode">Additional comparing rules. See <see cref="GestureCompareMode" /> for details</param>
		/// <returns><code>true</code> if first collection contains item which can be qualified as template for any item from second collection; otherwise <code>false</code></returns>
		public static bool ContainsTemplateForAny(this InputGestureCollection inputGestureTemplateCollection, InputGestureCollection testedInputGestureCollection, GestureCompareMode mode) {
			if((inputGestureTemplateCollection == null || inputGestureTemplateCollection.Count == 0) && (testedInputGestureCollection == null || testedInputGestureCollection.Count == 0)) {
				return true;
			}
			foreach (InputGesture template in inputGestureTemplateCollection) {
				if (template.IsTemplateForAny(testedInputGestureCollection, mode)) {
					return true;
				}   
			}
			
			return false;
		}
		
		/// <summary>
		/// Determines whether any item from first <see cref="InputGestureCollection" /> is qualified as template for
		/// provided <see cref="InputGesture" /> when examining using specific <see cref="GestureCompareMode" /> 
		/// </summary>
		/// <param name="inputGestureTemplateCollection">Collection of examined templates</param>
		/// <param name="testedInputGestureCollection">Examined Gesture</param>
		/// <param name="mode">Additional comparing rules. See <see cref="GestureCompareMode" /> for details</param>
		/// <returns><code>true</code> if collection contains template which match provided <see cref="InputGesture" />; otherwise <code>false</code></returns>
		public static bool ContainsTemplateFor(this InputGestureCollection inputGestureTemplateCollection, InputGesture testedGesture, GestureCompareMode mode) {
			foreach (InputGesture template in inputGestureTemplateCollection) {
				if (template.IsTemplateFor(testedGesture, mode)) {
					return true;
				}   
			}
			
			return false;
		}
		
		/// <summary>
		/// Sort bindings in <see cref="InputBindingCollection" /> according to number of chords each <see cref="InputBinding" />
		/// contains. 
		/// 
		/// Bindings associated with gestures having more chords should be on top.
		/// </summary>
		/// <param name="inputGestureCollection"></param>
		public static void SortByChords(this InputBindingCollection inputGestureCollection) 
		{
			ArrayList.Adapter(inputGestureCollection).Sort(new ChordsCountComparer());
		}
		
		private class ChordsCountComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var xInputBinding = (InputBinding)x;
				var yInputBinding = (InputBinding)y;
				
				var xMultiKeyGesture = xInputBinding.Gesture as MultiKeyGesture;
				var yMultiKeyGesture = yInputBinding.Gesture as MultiKeyGesture;
				
				var xLength = 0;
				if(xMultiKeyGesture != null) {
					xLength = xMultiKeyGesture.Chords != null ? xMultiKeyGesture.Chords.Count : 0;
				} else if(xInputBinding.Gesture is KeyGesture) {
					xLength = 1;
				}
				
				var yLength = 0;
				if(yMultiKeyGesture != null) {
					yLength = yMultiKeyGesture.Chords != null ? yMultiKeyGesture.Chords.Count : 0;
				} else if(yInputBinding.Gesture is KeyGesture) {
					yLength = 1;
				}
				
				return xLength.CompareTo(yLength);
			}
		}
	}
}
