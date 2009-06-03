using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.Core
{	
	public delegate void GesturesPlaceHolderUpdate();
	
	/// <summary>
	/// Stores gestures place holders
	/// 
	/// Through this class one can register a handler which which would be 
	/// called when gestures associated with place holder name are updated
	/// </summary>
	public static class BaseGesturesPlaceHolderRegistry
	{
		private static Dictionary<string, GesturesPlaceHolderInfo> gesturePlaceHolders = new Dictionary<string, GesturesPlaceHolderInfo>();
		
		private static Dictionary<string, string> commandTexts = new Dictionary<string, string>();
		private static Dictionary<string, List<string>> registeredKeys = new Dictionary<string, List<string>>();
		private static Dictionary<string, List<WeakReference>> commandKeysUpdateHandlers = new Dictionary<string, List<WeakReference>>();

		/// <summary>
		/// Register a place holder
		/// </summary>
		/// <param name="placeHolderName">Place holder name. Unique application wide</param>
		/// <param name="placeHolderText">Place holder text visible to user</param>
		public static void RegisterPlaceHolder(string placeHolderName, string placeHolderText) {
			gesturePlaceHolders.Add(placeHolderName, new GesturesPlaceHolderInfo(placeHolderName, placeHolderText));
		}
		
		/// <summary>
		/// Get gestures assigned to place holder
		/// 
		/// Use KeysCollectionConverter or InputGestureCollectionConverter 
		/// to convert returned value representation to required format
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <returns>Array of gestures</returns>
		public static string[] GetGestures(string placeHolderName) {
			if(!gesturePlaceHolders.ContainsKey(placeHolderName) || gesturePlaceHolders[placeHolderName].Gestures.Count == 0) {
				return new string[] { };
			} 
			
			return gesturePlaceHolders[placeHolderName].Gestures.ToArray();
		}
		
		/// <summary>
		/// Add gestures to place holder
		/// 
		/// Use KeysCollectionConverter or InputGestureCollectionConverter 
		/// to convert gestures to string representation
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <param name="gestures">Added gestures</param>
		public static void AddGestures(string placeHolderName, IEnumerable<string> gestures) {
			if(!gesturePlaceHolders.ContainsKey(placeHolderName)) {
				throw new ApplicationException("No gestures place holder with name '" + placeHolderName + "' is registered");
			}

			foreach(string gesture in gestures) {
				if(!gesturePlaceHolders[placeHolderName].Gestures.Contains(gesture)) {
				   gesturePlaceHolders[placeHolderName].Gestures.Add(gesture);
				}
			}
		}
		
		/// <summary>
		/// Remove gestures from place holder
		/// 
		/// Use KeysCollectionConverter or InputGestureCollectionConverter 
		/// to convert gestures to string representation
		/// 
		/// Null argumens are igonred and all gestures which specify
		/// provided parameters are removed
		/// </summary>
		/// <param name="placeHolderName"></param>
		/// <param name="gestures"></param>
		public static void RemoveGestures(string placeHolderName, IEnumerable<string> gestures) {
			if(gesturePlaceHolders.ContainsKey(placeHolderName)) {
				if(gestures == null) {
					gesturePlaceHolders[placeHolderName].Gestures.Clear();
				} else {
					for(int i = gesturePlaceHolders[placeHolderName].Gestures.Count - 1; i >= 0; i--) {		
						foreach(string gesture in gestures) {
							if(gesturePlaceHolders[placeHolderName].Gestures[i] == gesture) {
								gesturePlaceHolders[placeHolderName].Gestures.RemoveAt(i);
							}
						}
					}
				}
			}
		}
		      
		/// <summary>
		/// Register update handler which will trigger when gestures in
		/// place holder are updated
		/// </summary>
		/// <param name="placeHolderName">Place holder name</param>
		/// <param name="handler">Handler which handles place holder gestures update</param>
		public static void RegisterUpdateHandler(string placeHolderName, GesturesPlaceHolderUpdate handler) {
			if(!commandKeysUpdateHandlers.ContainsKey(placeHolderName)) {
				commandKeysUpdateHandlers.Add(placeHolderName, new List<WeakReference>());
			}
			
			commandKeysUpdateHandlers[placeHolderName].Add(new WeakReference(handler));
		}
		
		/// <summary>
		/// Invoke delegates which handle gestures update in place holder
		/// 
		/// If place holder is not provided all registered handlers are invoked
		/// </summary>
		/// <param name="placeHolderName"></param>
		public static void InvokeUpdateHandlers(string placeHolderName) {
			if(placeHolderName == null) {
				foreach(var handlers in commandKeysUpdateHandlers) {
					foreach(var handler in handlers.Value) {
						if(handler != null && handler.Target != null) {
							((GesturesPlaceHolderUpdate)handler.Target).Invoke();
						}
					}
				}
			} else {
				if(commandKeysUpdateHandlers.ContainsKey(placeHolderName)) {
					foreach(var handler in commandKeysUpdateHandlers[placeHolderName]) {
						if(handler != null && handler.Target != null) {
							((GesturesPlaceHolderUpdate)handler.Target).Invoke();
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Intername representation of place holders
		/// </summary>
		class GesturesPlaceHolderInfo 
		{
			/// <summary>
			/// Place hollder name
			/// </summary>
			public string Name {
				get; private set;
			}
			
			/// <summary>
			/// Place holder text visible to user
			/// </summary>
			public string Text {
				get; private set;
			}
			
			/// <summary>
			/// Collection of gestures assigned to this place holder
			/// </summary>
			public List<string> Gestures {
				get; private set;
			}
			
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="name">Place holder name</param>
			/// <param name="text">Place holder text visible to user</param>
			public GesturesPlaceHolderInfo(string name, string text) {
				Name = name;
				Text = text;
				Gestures = new List<string>();
			}
				
		}
	}
}
