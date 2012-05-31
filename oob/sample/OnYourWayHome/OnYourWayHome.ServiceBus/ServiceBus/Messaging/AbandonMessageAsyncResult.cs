//---------------------------------------------------------------------------------
// Copyright (c) 2011, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using OnYourWayHome.AccessControl;

    internal sealed class AbandonMessageAsyncResult : ServiceBusRequestAsyncResult<bool>
    {
        private readonly BrokeredMessage message;

        public AbandonMessageAsyncResult(BrokeredMessage message, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message.LockLocation == null)
            {
                throw new InvalidOperationException("The message cannot be completed, because it does not appear to be locked.");
            }

            this.message = message;
        }

        public BrokeredMessage Message
        {
            get { return this.message; }
        }

        protected override Uri Uri
        {
            get { return this.message.LockLocation; }
        }

        protected override string Method
        {
            get { return "PUT"; }
        }
    }
}
