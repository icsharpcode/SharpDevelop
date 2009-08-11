using System;
using System.Windows.Input;
using System.Linq;
using System.Collections;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of InputGestureCollectionExtensions.
	/// </summary>
	public static class InputGestureCollectionExtensions
	{
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
		
		
		public static bool ContainsTemplateFor(this InputGestureCollection inputGestureTemplateCollection, InputGesture testedGesture, GestureCompareMode mode) {
			foreach (InputGesture template in inputGestureTemplateCollection) {
				if (template.IsTemplateFor(testedGesture, mode)) {
					return true;
				}   
			}
			
			return false;
		}
		
		/// <summary>
		/// Sort key gestures within input gesture collection by length and type
		/// Shorter gestures should be first so they could be matched first.
		/// </summary>
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
