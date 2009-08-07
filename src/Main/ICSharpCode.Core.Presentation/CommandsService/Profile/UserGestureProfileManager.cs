using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Input;
using System.IO;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation  
{
	/// <summary>
	/// Manages active <see cref="UserGestureProfile" /> 
	/// </summary>
	public static class UserGestureProfileManager
	{
		private static UserGestureProfile _currentProfile;
		private static string _userGestureProfilesDirectory;
		
		static UserGestureProfileManager()
		{
			if(PropertyService.ConfigDirectory != null) {
				_userGestureProfilesDirectory = Path.Combine(PropertyService.ConfigDirectory, "UserGestureProfiles");
				if(!Directory.Exists(_userGestureProfilesDirectory)) {
					Directory.CreateDirectory(_userGestureProfilesDirectory);
				}
			}
		}
		
		/// <summary>
		/// Gets path to directory where user gesture profiles are stored as XML files
		/// </summary>
		public static string UserGestureProfilesDirectory
		{
			get {
				return _userGestureProfilesDirectory;
			}
		}
		
		/// <summary>
		/// Gets or sets current profile. 
		/// 
		/// Current profile can change <see cref="InputBindingInfo.ActiveGestures" />
		/// </summary>
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
					InvokeCurrentProfileChanged(typeof(UserGestureProfileManager), new NotifyUserGestureProfileChangedEventArgs(oldProfile, _currentProfile));
				}
			} 
		}
		
		/// <summary>
		/// Occurs when <see cref="CurrentProfile" /> is changes
		/// </summary>
		public static event NotifyUserGestureProfileChangedEventHandler CurrentProfileChanged;
	
		private static void InvokeCurrentProfileChanged(object sender, NotifyUserGestureProfileChangedEventArgs args)
		{
			if(CurrentProfileChanged != null) {
				CurrentProfileChanged.Invoke(sender, args);
			}
		}
		
		/// <summary>
		/// Reset <see cref="UserGestureProfileManager" />
		/// 
		/// Intended to use for unit tests only
		/// </summary>
		public static void Reset()
		{
			CurrentProfile = null;
			CurrentProfileChanged = null;
		}
	}
}
