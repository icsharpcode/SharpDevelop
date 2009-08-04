using System;

namespace ICSharpCode.Core.Presentation
{
	public class NotifyUserGestureProfileChangedEventArgs : EventArgs
	{
		private UserGestureProfile _oldProfile;
		public UserGestureProfile OldProfile
		{
			get {
				return _oldProfile;
			}
		}
		
		private UserGestureProfile _newProfile;
		public UserGestureProfile NewProfile
		{
			get {
				return _newProfile;
			}
		}
		
		public NotifyUserGestureProfileChangedEventArgs(UserGestureProfile oldProfile, UserGestureProfile newProfile) 
		{
			_oldProfile = oldProfile;
			_newProfile = newProfile;
		}
	}
	
	public delegate void NotifyUserGestureProfileChangedEventHandler(object sender, NotifyUserGestureProfileChangedEventArgs args);
}
