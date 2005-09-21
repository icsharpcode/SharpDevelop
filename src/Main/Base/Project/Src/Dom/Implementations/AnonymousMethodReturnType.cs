// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The return type of anonymous method expressions or lambda expressions.
	/// </summary>
	public sealed class AnonymousMethodReturnType : ProxyReturnType
	{
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override IReturnType BaseType {
			get {
				return ReflectionReturnType.Delegate;
			}
		}
		
		public override string Name {
			get {
				return "delegate";
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return Name;
			}
		}
		
		public override string Namespace {
			get {
				return "";
			}
		}
		
		public override string DotNetName {
			get {
				return Name;
			}
		}
	}
}
