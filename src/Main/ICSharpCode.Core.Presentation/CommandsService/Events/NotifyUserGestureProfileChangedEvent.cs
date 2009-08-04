using System;

namespace ICSharpCode.Core.Presentation
{
	public class NotifyUserGestureProfileChangedEventArgs : EventArgs
	{
		private UserGesturesProfile _oldProfile;
		public UserGesturesProfile OldProfile
		{
			get {
				return _oldProfile;
			}
		}
		
		private UserGesturesProfile _newProfile;
		public UserGesturesProfile NewProfile
		{
			get {
				return _newProfile;
			}
		}
		
		public NotifyUserGestureProfileChangedEventArgs(UserGesturesProfile oldProfile, UserGesturesProfile newProfile) 
		{
			_oldProfile = oldProfile;
			_newProfile = newProfile;
		}
	}
	
	public delegate void NotifyUserGestureProfileChangedEventHandler(object sender, NotifyUserGestureProfileChangedEventArgs args);
}
