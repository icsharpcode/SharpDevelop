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

		
		public static bool ContainsTemplateFor(this InputGestureCollection inputGestureTemplateCollection, InputGesture testedGesture, GestureCompareMode mode) {
			foreach (InputGesture template in inputGestureTemplateCollection) {
	            if (template.IsTemplateFor(testedGesture, mode)) {
	                return true;
	            }   
	        }
			
			return false;
		}
	}
}
