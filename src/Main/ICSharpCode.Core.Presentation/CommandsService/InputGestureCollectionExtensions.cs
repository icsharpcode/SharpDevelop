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
			var searchedKeyGesture = searchedGesture as KeyGesture;
			var searchedMouseGesture = searchedGesture as MouseGesture;
			
			foreach(var gesture in gestures) {
				if(searchedKeyGesture != null) {
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
