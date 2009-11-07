/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 8/3/2009
 * Time: 7:47 PM
 */
using System;

namespace ICSharpCode.Core.Presentation
{
	public class NotifyUserGestureProfileChangedEventArgs
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
	
	public delegate void NotifyUserGestureProfileChangedEventHandler(object sender, NotifyUserGestureProfileChangedEventArgs args);
}
