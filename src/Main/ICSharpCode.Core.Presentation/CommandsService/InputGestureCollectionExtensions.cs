using System;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of InputGestureCollectionExtensions.
	/// </summary>
	public static class InputGestureCollectionExtensions
	{
		public static bool ContainsCopy(this InputGestureCollection gestures, InputGesture searchedGesture) {
			var searchedMultiKeyGesture = searchedGesture as MultiKeyGesture;
			var searchedKeyGesture = searchedGesture as KeyGesture;
			var searchedMouseGesture = searchedGesture as MouseGesture;
			
			foreach(var gesture in gestures) {
				if(searchedMultiKeyGesture != null) {
					var multiKeyGesture = gesture as MultiKeyGesture;
					if(multiKeyGesture != null && multiKeyGesture.Gestures != null && searchedMultiKeyGesture.Gestures != null && multiKeyGesture.Gestures.Count == searchedMultiKeyGesture.Gestures.Count) {
						var foundMatch = true;
						foreach(var partialGesture in multiKeyGesture.Gestures) {
							foreach(var searchedPartialGesture in searchedMultiKeyGesture.Gestures) {
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
