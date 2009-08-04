using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Input;
using System.IO;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Manages user defined gestures
	/// </summary>
	public static class UserGestureManager
	{
		private static string _userGestureProfilesDirectory;
		
		static UserGestureManager()
		{
			if(PropertyService.ConfigDirectory != null) {
				_userGestureProfilesDirectory = Path.Combine(PropertyService.ConfigDirectory, "UserGestureProfiles");
				if(!Directory.Exists(_userGestureProfilesDirectory)) {
					Directory.CreateDirectory(_userGestureProfilesDirectory);
				}
			}
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
		
		private static UserGestureProfile _currentProfile;
	
		public static UserGestureProfile CurrentProfile
		{
			get {
				return _currentProfile;
			}
			set {
				var oldProfile = _currentProfile;
				_currentProfile = value;
				
				var currentProfilePath = value != null ? value.Path : null;
				if(currentProfilePath != null) {
					PropertyService.Set("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory", currentProfilePath);
				} else {
					PropertyService.Remove("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
				}
				
				if(_currentProfile != oldProfile) {
					InvokeCurrentProfileChanged(typeof(UserGestureManager), new NotifyUserGestureProfileChangedEventArgs(oldProfile, _currentProfile));
				}
			} 
		}
		
		public static event NotifyUserGestureProfileChangedEventHandler CurrentProfileChanged;
	
		public static void InvokeCurrentProfileChanged(object sender, NotifyUserGestureProfileChangedEventArgs args)
		{
			if(CurrentProfileChanged != null) {
				CurrentProfileChanged.Invoke(sender, args);
			}
		}
		
		public static void Reset()
		{
			CurrentProfile = null;
			CurrentProfileChanged = null;
		}
	}
}
