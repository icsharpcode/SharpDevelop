using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{	
	/// <summary>
	/// Contains utility methods for work with <see cref="InputGesture" />
	/// </summary>
	public static class InputGestureExtensions
	{
		/// <summary>
		/// Determines whether provided <see cref="InputGesture" /> is qualified as template for
		/// another instance of <see cref="InputGesture" /> when examining using specific <see cref="GestureCompareMode" /> 
		/// </summary>
		/// <param name="inputGestureTemplate">Examined input gesture template</param>
		/// <param name="inputGesture">Examined input gesture</param>
		/// <param name="mode">Additional comparing rules. See <see cref="GestureCompareMode" /> for details</param>
		/// <returns><code>true</code> if template matches compared geture; otherwise <code>false</code></returns>
		public static bool IsTemplateFor(this InputGesture inputGestureTemplate, InputGesture inputGesture, GestureCompareMode mode)
		{
			if(mode == GestureCompareMode.Conflicting) {
				return inputGestureTemplate.IsTemplateFor(inputGesture, GestureCompareMode.StartsWithFullChords) || inputGesture.IsTemplateFor(inputGestureTemplate, GestureCompareMode.StartsWithFullChords);
			}
			
			var multiKeyGesture = inputGesture as MultiKeyGesture;
			var multiKeyGestureTemplate = inputGestureTemplate as MultiKeyGesture;
			var partialKeyGesture = inputGesture as PartialKeyGesture;
			var partialKeyGestureTemplate = inputGestureTemplate as PartialKeyGesture;
			var keyGesture = inputGesture as KeyGesture;
			var keyGestureTemplate = inputGestureTemplate as KeyGesture;
			
			// Create partial key gestures from key gestures
			if(multiKeyGesture == null && partialKeyGesture == null && keyGesture != null) {
				partialKeyGesture = new PartialKeyGesture(keyGesture);
			}
			
			if(multiKeyGestureTemplate == null && partialKeyGestureTemplate == null && keyGestureTemplate != null) {
				partialKeyGestureTemplate = new PartialKeyGesture(keyGestureTemplate);
			}
			
			// Create multi key gestures from gestures
			if(multiKeyGesture == null && partialKeyGesture != null) {
				multiKeyGesture = new MultiKeyGesture(new [] { partialKeyGesture });
			}
			
			if(multiKeyGestureTemplate == null && partialKeyGestureTemplate != null) {
				multiKeyGestureTemplate = new MultiKeyGesture(new [] { partialKeyGestureTemplate });
			}
			
			if(multiKeyGesture == null 
			   || multiKeyGestureTemplate == null
			   || multiKeyGesture.Chords == null
			   || multiKeyGestureTemplate.Chords == null
			   || multiKeyGestureTemplate.Chords.Count == 0
			   || multiKeyGesture.Chords.Count == 0
			   || (mode == GestureCompareMode.ExactlyMatches && multiKeyGestureTemplate.Chords.Count != multiKeyGesture.Chords.Count)) {
				return false;
			}
			
			// Stores N previous chords
			var previousChords = new LinkedList<PartialKeyGesture>();
			
			// Search whether chord sequece matches template sequenc in any place
			foreach (PartialKeyGesture chord in multiKeyGesture.Chords) {
				// Accumulates previous chords
				previousChords.AddLast(chord);
				if (previousChords.Count > multiKeyGestureTemplate.Chords.Count) {
					previousChords.RemoveFirst();
				}
				
				// Only start comparing with template when needed amount of previous chords where collected
				if (previousChords.Count == multiKeyGestureTemplate.Chords.Count) {
					var multiKeyGestureTemplateEnumerator = multiKeyGestureTemplate.Chords.GetEnumerator();
					var previousChordsEnumerator = previousChords.GetEnumerator();
					
					multiKeyGestureTemplateEnumerator.Reset();
					
					// Compare two chord sequences in a loop
					var multiKeyGesturesMatch = true;
					
					var i = 0;
					while (previousChordsEnumerator.MoveNext() && multiKeyGestureTemplateEnumerator.MoveNext()) {
						var template = multiKeyGestureTemplateEnumerator.Current;
						var gesture = previousChordsEnumerator.Current;
						
						if (((mode == GestureCompareMode.StartsWith || mode == GestureCompareMode.PartlyMatches) && i == previousChords.Count - 1)
						   || (mode == GestureCompareMode.PartlyMatches && i == 0)) {
							
							if(!(template.Modifiers == (gesture.Modifiers & template.Modifiers) && (template.Key == Key.None || template.Key == gesture.Key))) {
								multiKeyGesturesMatch = false;
								break;                        		
							}
						} else if (template.Modifiers != gesture.Modifiers || template.Key != gesture.Key) {
							multiKeyGesturesMatch = false;
							break;
						}
						
						i++;
					}
					
					if(multiKeyGesturesMatch) {
						return true;
					}
					
					if (mode == GestureCompareMode.StartsWith || mode == GestureCompareMode.StartsWithFullChords) {
						break;
					}
				}
			}
			
			return false;
		}
		
		
		/// <summary>
		/// Determines whether this instance of <see cref="InputGesture"/> qualifies as template for any of examined gestures
		/// </summary>
		/// <param name="thisGesture">Examined input gesture template</param>
		/// <param name="examinedGestures">Collection of examined gestures</param>
		/// <param name="mode">Additional comparing rules. See <see cref="GestureCompareMode" /> for details</param>
		/// <returns><code>true</code> if template matches one of the compared getures; otherwise <code>false</code></returns>
		public static bool IsTemplateForAny(this InputGesture thisGesture, InputGestureCollection examinedGestures, GestureCompareMode mode)
		{
			foreach (InputGesture gesture in examinedGestures) {
				if (thisGesture.IsTemplateFor(gesture, mode)) {
					return true;
				}
			}
			
			return false;
		}
	}
}
