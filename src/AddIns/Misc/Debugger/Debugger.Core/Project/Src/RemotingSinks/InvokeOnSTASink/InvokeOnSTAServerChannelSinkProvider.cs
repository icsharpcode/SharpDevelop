// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

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
