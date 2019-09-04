// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Sagas.AggregateSaga;

namespace Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSagaManager : AggregateSagaManager<MoneyTransferSaga,MoneyTransferSagaId,MoneyTransferSagaLocator>
    {
        // 
        public Dictionary<IIdentity, EventException> EventExceptions { get; } 
        public MoneyTransferSagaManager(Expression<Func<MoneyTransferSaga>> factory)
            : base(factory)
        {
            EventExceptions = new Dictionary<IIdentity, EventException>();
            Receive<RetryFailedMessages>(msg => RetryMessages() );
        }

        private bool RetryMessages()
        {
            foreach (var asdf in EventExceptions)
            {
                Self.Tell(asdf.Value.FailedEvent);
            }

            return true;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, 10, exception =>
            {
                if (exception is SagaHandlerException)
                {
                    var ete = exception as SagaHandlerException;
                    Logger.Warning($"******* A SagaHandlerException was received ------> " +
                                   $"{ete.EventTriggeringException.GetType().Name}");
                   
                    EventExceptions.Add(ete.EventTriggeringException.GetIdentity(), 
                        new EventException(ete.EventTriggeringException, ete.InnerException)
                        );
                    return Directive.Resume;
                }
                else return Directive.Stop;
            });
        }
    }

    public class EventException
    {
        public IDomainEvent FailedEvent { get; }
        public Exception Exception { get; }

        public EventException(IDomainEvent failedEvent, Exception exception)
        {
            FailedEvent = failedEvent;
            Exception = exception;
        }
    }
}