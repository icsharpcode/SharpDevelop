// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Collections;

namespace CustomSinks
{
	class InvokeOnSTAServerChannelSinkProvider: IServerChannelSinkProvider 
	{
		private IServerChannelSinkProvider nextProvider;

		public InvokeOnSTAServerChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IServerChannelSink IServerChannelSinkProvider.CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink nextSink = nextProvider.CreateSink(channel);
			IServerChannelSink thisSink = new InvokeOnSTAServerChannelSink(nextSink);
			return thisSink;
		}

		void IServerChannelSinkProvider.GetChannelData(IChannelDataStore channelData)
		{
		}

		IServerChannelSinkProvider IServerChannelSinkProvider.Next {
			get	{
				return nextProvider;
			}
			set	{
				nextProvider = value;
			}
		}
	}
}
