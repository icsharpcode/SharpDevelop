//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//
//using ICSharpCode.Core;
//using Chimps.Debug.Cor;
//
//namespace ICSharpCode.Core
//{
//	public delegate void BreakEventHandler(object sender, BreakEventArgs e);
//	
//	public class BreakEventArgs
//	{
//		Chimps.Debug.Cor.DbgAppDomain appDomain;
//		Chimps.Debug.Cor.DbgThread thread;
//		
//		public MethodCall[] CallStack {
//			get {
//				
//				return DebuggerService.GetStackList(thread.ID);
//			}
//		}
//		
//		public BreakEventArgs(Chimps.Debug.Cor.DbgAppDomain appDomain, Chimps.Debug.Cor.DbgThread thread)
//		{
//			this.appDomain = appDomain;
//			this.thread    = thread;
//		}
//	}
//}
//
