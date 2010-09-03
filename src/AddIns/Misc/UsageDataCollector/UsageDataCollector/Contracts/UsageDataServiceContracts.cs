// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ICSharpCode.UsageDataCollector.Contracts
{
    [ServiceContract]
    public interface IUDCUploadService
    {
        [OperationContract(IsOneWay = true)]
        void UploadUsageData(UDCUploadRequest request);
    }

    [MessageContract()]
    public class UDCUploadRequest
    {
        [MessageHeader(MustUnderstand = true)]
        public string ApplicationKey;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream UsageData;
    }
}
