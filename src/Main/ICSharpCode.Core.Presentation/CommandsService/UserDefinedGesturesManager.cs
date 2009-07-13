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
					}
					
					_isCurrentProfileLoaded = true;
				}
				
				return _currentProfile;
			}
			set {
				_currentProfile = value;
				
				var currentProfilePath = value != null ? value.Path : null;
				
				if(currentProfilePath != null) {
					PropertyService.Set("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory", currentProfilePath);
				} else {
					PropertyService.Remove("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
				}
				
				_isCurrentProfileLoaded = true;
			}
		}
	}
}
