using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.Core.WinForms
{	
	/// <summary>
	/// Stores gestures place holders
	/// 
	/// Through this class one can register a handler which which would be 
	/// called when gestures associated with place holder name are updated
	/// </summary>
	public static class GesturePlaceHolderRegistry
	{
		/// <summary>
		/// Register a place holder
		/// </summary>
		/// <param name="placeHolderName">Place holder name. Unique application wide</param>
		/// <param name="placeHolderText">Place holder text visible to user</param>
		public static void RegisterPlaceHolder(string placeHolderName, string placeHolderText) {
			BaseGesturesPlaceHolderRegistry.RegisterPlaceHolder(placeHolderName, placeHolderText);
		}
		
		/// <summary>
		/// Get gestures assigned to place holder
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <returns>Array of keys</returns>
		public static Keys[] GetGestures(string placeHolderName) {
			var serializesGesturesColection = BaseGesturesPlaceHolderRegistry.GetGestures(placeHolderName);
			var gestures = (Keys[])new KeysCollectionConverter().ConvertFrom(serializesGesturesColection);
			
			return gestures;
		}
		
		/// <summary>
		/// Add gestures to place holder
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <param name="gestures">Added gestures</param>
		public static void AddGestures(string placeHolderName, IEnumerable<Keys> gestures) {
			var serializedGestures = (string[])new KeysCollectionConverter().ConvertTo(gestures, typeof(string[]));
			
			BaseGesturesPlaceHolderRegistry.AddGestures(placeHolderName, serializedGestures);
		}
		
		/// <summary>
		/// Remove gestures from place holder
		/// 
		/// Null argumens are igonred and all gestures which specify
		/// provided parameters are removed
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <param name="gestures">Gestures</param>
		public static void RemoveGestures(string placeHolderName, IEnumerable<Keys> gestures) {
			var serializedGestures = (string[])new KeysCollectionConverter().ConvertTo(gestures, typeof(string[]));
			
			BaseGesturesPlaceHolderRegistry.RemoveGestures(placeHolderName, serializedGestures);
		}
		
		/// <summary>
		/// Register update handler which will trigger when gestures in
		/// place holder are updated
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <param name="handler">Handler which handles place holder gestures update</param>                             
		public static void RegisterUpdateHandler(string placeHolderName, GesturesPlaceHolderUpdate handler) {
			BaseGesturesPlaceHolderRegistry.RegisterUpdateHandler(placeHolderName, handler);
		}
		
		/// <summary>
		/// Invoke delegates which handle gestures update in place holder
		/// 
		/// If place holder is not provided all registered handlers are invoked
		/// </summary>
		/// <param name="placeHolderName"></param>
		public static void InvokeUpdateHandlers(string placeHolderName) {
			BaseGesturesPlaceHolderRegistry.InvokeUpdateHandlers(placeHolderName);
		}
	}
}
