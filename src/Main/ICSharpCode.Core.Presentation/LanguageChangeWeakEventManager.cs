// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.Core.Presentation
{
	public sealed class LanguageChangeWeakEventManager : WeakEventManager
	{
		/// <summary>
		/// Adds a weak event listener.
		/// </summary>
		public static void AddListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedAddListener(null, listener);
		}
		
		/// <summary>
		/// Removes a weak event listener.
		/// </summary>
		public static void RemoveListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedRemoveListener(null, listener);
		}
		
		/// <summary>
		/// Gets the current manager.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		private static LanguageChangeWeakEventManager CurrentManager {
			get {
				LanguageChangeWeakEventManager manager = (LanguageChangeWeakEventManager)GetCurrentManager(typeof(LanguageChangeWeakEventManager));
				if (manager == null) {
					manager = new LanguageChangeWeakEventManager();
					SetCurrentManager(typeof(LanguageChangeWeakEventManager), manager);
				}
				return manager;
			}
		}
		
		protected override void StartListening(object source)
		{
			ResourceService.LanguageChanged += DeliverEvent;
		}
		
		protected override void StopListening(object source)
		{
			ResourceService.LanguageChanged -= DeliverEvent;
		}
	}
}
