using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Input;
using System.IO;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Manages user defined gestures
	/// </summary>
	public static class UserDefinedGesturesManager
	{
		private static string _userGestureProfilesDirectory;
		
		static UserDefinedGesturesManager()
		{
			if(PropertyService.ConfigDirectory != null) {
				_userGestureProfilesDirectory = Path.Combine(PropertyService.ConfigDirectory, "UserGestureProfiles");
				if(!Directory.Exists(_userGestureProfilesDirectory)) {
					Directory.CreateDirectory(_userGestureProfilesDirectory);
				}
			}
			
			CurrentProfileChanged += CurrentProfile_CurrentProfileChanged;
		}
		
		/// <summary>
		/// Path to file where current user defined gestures are set
		/// </summary>
		public static string UserGestureProfilesDirectory
		{
			get {
				return _userGestureProfilesDirectory;
			}
		}
		
		private static UserGesturesProfile _currentProfile;
		private static bool _isCurrentProfileLoaded;
		
		public static event UserGestureProfileChangedHandler CurrentProfileChanged;
		private static Dictionary<InputBindingIdentifier, HashSet<InputBindingGesturesChangedHandler>> ActiveGesturesChangedHandlers = new Dictionary<InputBindingIdentifier, HashSet<InputBindingGesturesChangedHandler>>();
		
		public static void AddActiveGesturesChangedHandler(InputBindingIdentifier identifier, InputBindingGesturesChangedHandler handler) 
		{
			if(!ActiveGesturesChangedHandlers.ContainsKey(identifier)) {
				ActiveGesturesChangedHandlers.Add(identifier, new HashSet<InputBindingGesturesChangedHandler>());
			}
			
			ActiveGesturesChangedHandlers[identifier].Add(handler);
		}
		
		public static void RemoveActiveGesturesChangedHandler(InputBindingIdentifier identifier, InputBindingGesturesChangedHandler handler) 
		{
			if(ActiveGesturesChangedHandlers.ContainsKey(identifier)) {
				ActiveGesturesChangedHandlers[identifier].Remove(handler);
			}
		}
		
		private static void InvokeActiveGesturesChangedHandlers(InputBindingIdentifier identifier) 
		{
			var args = new InputBindingGesturesChangedArgs { InputBindingIdentifier = identifier };
			if(ActiveGesturesChangedHandlers.ContainsKey(identifier)) {
				foreach(var handler in ActiveGesturesChangedHandlers[identifier]) {
					handler.Invoke(typeof(UserDefinedGesturesManager), args);
				}
			}
		}
		
		public static UserGesturesProfile CurrentProfile
		{
			get {
				if(!_isCurrentProfileLoaded)
				{
					var path = PropertyService.Get("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
					if(path != null && File.Exists(path)) {
						var profile = new UserGesturesProfile();
						profile.Path = path;
						profile.Load();
						
						_currentProfile = profile;
						
						if(CurrentProfileChanged != null) {
							var args = new UserGestureProfileChangedArgs();
							args.NewProfile = profile;
							CurrentProfileChanged.Invoke(typeof(UserDefinedGesturesManager), args);
						}
					}
					
					_isCurrentProfileLoaded = true;
				}
				
				return _currentProfile;
			}
			set {
				var currentProfileBackup = _currentProfile;
				_currentProfile = value;
				
				if(_currentProfile != currentProfileBackup && CurrentProfileChanged != null) {
					var args = new UserGestureProfileChangedArgs();
					args.NewProfile = _currentProfile;
					args.OldProfile = currentProfileBackup;
					CurrentProfileChanged.Invoke(typeof(UserDefinedGesturesManager), args);
				}
				
				var currentProfilePath = value != null ? value.Path : null;
				
				if(currentProfilePath != null) {
					PropertyService.Set("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory", currentProfilePath);
				} else {
					PropertyService.Remove("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
				}
				
				_isCurrentProfileLoaded = true;
			}
		}
		
		private static void CurrentProfile_CurrentProfileChanged(object sender, UserGestureProfileChangedArgs args) {
			var identifiers = new HashSet<InputBindingIdentifier>();
			
			if(args.OldProfile != null) {
				foreach(var gesture in args.OldProfile) {
					identifiers.Add(gesture.Key);
				}
			}
	
			if(args.NewProfile != null) {
				foreach(var gesture in args.NewProfile) {
					identifiers.Add(gesture.Key);
				}
			}
			
			foreach(var identifier in identifiers) {
				InvokeActiveGesturesChangedHandlers(identifier);
			}
		}
	}
	
	public class UserGestureProfileChangedArgs
	{
		public UserGesturesProfile OldProfile
		{
			get; set;
		}
		
		public UserGesturesProfile NewProfile
		{
			get; set;
		}
	}
	
	public delegate void UserGestureProfileChangedHandler(object sender, UserGestureProfileChangedArgs args);
	
}
