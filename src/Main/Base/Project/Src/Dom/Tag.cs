// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class Tag : MarshalByRefObject //Comment
	{
		string key;
		
		public string Key {
			get {
				return key;
			}
		}
		
		string commentString;
		IRegion region;
		
		public string CommentString {
			get {
				return commentString;
			}
			set {
				commentString = value;
			}
		}
		
		public IRegion Region {
			get {
				return region;
			}
			set {
				region = value;
			}
		}
		
		public Tag(string key, IRegion region)
		{
			this.key = key;
			this.region = region;
		}
	}
}
