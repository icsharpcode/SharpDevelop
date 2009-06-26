using System;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of InputGestureCollectionExtensions.
	/// </summary>
	public static class InputGestureCollectionExtensions
	{
		public static bool ContainsTemplateForAny(this InputGestureCollection inputGestureTemplateCollection, InputGestureCollection testedInputGestureCollection, GestureCompareMode mode) {
	        foreach (InputGesture template in inputGestureTemplateCollection) {
	            if (template.IsTemplateForAny(testedInputGestureCollection, mode)) {
	                return true;
	            }   
	        }
			
			return false;
		}

		
		public static bool ContainsCopy(this InputGestureCollection gestures, InputGesture searchedGesture) {
			var searchedMultiKeyGesture = searchedGesture as MultiKeyGesture;
			var searchedKeyGesture = searchedGesture as KeyGesture;
			var searchedMouseGesture = searchedGesture as MouseGesture;
			
			foreach(var gesture in gestures) {
				if(searchedMultiKeyGesture != null) {
					var multiKeyGesture = gesture as MultiKeyGesture;
					if(multiKeyGesture != null && multiKeyGesture.Chords != null && searchedMultiKeyGesture.Chords != null && multiKeyGesture.Chords.Count == searchedMultiKeyGesture.Chords.Count) {
						var foundMatch = true;
						foreach(var partialGesture in multiKeyGesture.Chords) {
							foreach(var searchedPartialGesture in searchedMultiKeyGesture.Chords) {
								if(partialGesture.Key != searchedPartialGesture.Key || partialGesture.Modifiers != searchedPartialGesture.Modifiers) {
									foundMatch = false;
									break;
								}
							}
						}
						
						if(foundMatch) {
							return true;
						}
					}
				} else if(searchedKeyGesture != null) {
					var keyGesture = gesture as KeyGesture;
					if(keyGesture != null && keyGesture.Key == searchedKeyGesture.Key && keyGesture.Modifiers == searchedKeyGesture.Modifiers) {
						return true;
					}
				} else if(searchedMouseGesture != null) {
					var mouseGesture = gesture as MouseGesture;
					if(mouseGesture != null && mouseGesture.MouseAction == searchedMouseGesture.MouseAction && mouseGesture.Modifiers == searchedMouseGesture.Modifiers) {
						return true;
					}
				}
			}
			
			return false;
		}
	}
}
