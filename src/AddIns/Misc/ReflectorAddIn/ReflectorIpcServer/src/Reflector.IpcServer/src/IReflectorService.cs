// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Reflector.IpcServer
{
	/// <summary>
	/// Provides the ability to remote-control Reflector.
	/// </summary>
	public interface IReflectorService
	{
		/// <summary>
		/// Used to test the remoting connection without doing any action.
		/// </summary>
		/// <returns><c>true</c>.</returns>
		bool CheckIsThere();
		
		/// <summary>
		/// Gets whether the Reflector service is ready to accept commands.
		/// </summary>
		bool IsReady {
			get;
		}
		
		/// <summary>
		/// Positions the assembly browser of Reflector on the element
		/// that best matches the parameters contained in the <paramref name="element"/> parameter
		/// and brings the Reflector window to front if at least the specified assembly was found.
		/// </summary>
		/// <param name="element">A <see cref="CodeElementInfo"/> object that describes the element to go to.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="element"/> parameter is <c>null</c>.</exception>
		/// <exception cref="InvalidOperationException">The service is not ready (the <see cref="IsReady"/> property is false).</exception>
		void GoTo(CodeElementInfo element);
	}
}
