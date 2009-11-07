using System;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Represent a method that will handle <see cref="ICSharpCode.Core.Presentation.UserGestureProfileManager.CurrentProfileChanged" /> event
	/// </summary>
	public delegate void NotifyUserGestureProfileChangedEventHandler(object sender, NotifyUserGestureProfileChangedEventArgs args);
	
	/// <summary>
	/// Provides data for <see cref="ICSharpCode.Core.Presentation.UserGestureProfileManager.CurrentProfileChanged" /> event
	/// </summary>
	public class NotifyUserGestureProfileChangedEventArgs : EventArgs
	{
		private UserGestureProfile _newProfile;
		private UserGestureProfile _oldProfile;
		
		/// <summary>
		/// Gets current <see cref="UserGestureProfile" /> before modification
		/// </summary>
		public UserGestureProfile OldProfile
		{
			get {
				return _oldProfile;
			}
		}
		
		/// <summary>
		/// Gets current <see cref="UserGestureProfile" /> after modification
		/// </summary>
		public UserGestureProfile NewProfile
		{
			get {
				return _newProfile;
			}
		}
		
		/// <summary>
		/// Create new instance of <see cref="NotifyUserGestureProfileChangedEventArgs" />
		/// </summary>
		/// <param name="oldProfile">Current <see cref="UserGestureProfile" /> before modification</param>
		/// <param name="newProfile">Current <see cref="UserGestureProfile" /> after modification</param>
		public NotifyUserGestureProfileChangedEventArgs(UserGestureProfile oldProfile, UserGestureProfile newProfile) 
		{
			_oldProfile = oldProfile;
			_newProfile = newProfile;
		}
	}
}
