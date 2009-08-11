using System;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	public enum GestureCompareMode 
	{
		/// <summary>
		/// Match is successfull when all chrods from both instance of <see cref="InputGesture" /> match
		/// or all gestures in both instance of <see cref="InputGestureCollections" /> match
		/// </summary>
		ExactlyMatches,
		
		/// <summary>
		/// Match is successfull when two instance of <see cref="InputGesture" /> ar conflicting.
		/// 
		/// Either one or another instance starts with the same chors as other one
		/// </summary>
		Conflicting,
		
		/// <summary>
		/// Match is successfull when template <see cref="InputGesture" /> partly matches compared <see cref="InputGesture" />.
		/// Template is found in any place within matched gesture 
		/// </summary>
		PartlyMatches,
		
		
		/// <summary>
		/// Match is successfull when examined <see cref="InputGesture" /> starts with the same keys as template
		/// </summary>
		StartsWith,
		
		/// <summary>
		/// Match is successfull when examined <see cref="InputGesture" /> starts with the same complite chords as template
		/// </summary>
		StartsWithFullChords,
	}
}
