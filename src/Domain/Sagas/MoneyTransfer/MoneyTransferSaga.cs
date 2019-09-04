﻿// The MIT License (MIT)
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
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.Specifications.Provided;
using Domain.Model.Account;
using Domain.Model.Account.Commands;
using Domain.Model.Account.Events;
using Domain.Sagas.MoneyTransfer.Events;

namespace Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSaga : AggregateSaga<MoneyTransferSaga, MoneyTransferSagaId, MoneyTransferSagaState>,
        ISagaIsStartedBy<Account, AccountId, MoneySentEvent>,
        ISagaHandles<Account, AccountId, MoneyReceivedEvent>
    {
        public IActorRef AccountAggregateManager { get; }
        public MoneyTransferSaga(IActorRef accountAggregateManager)
        {
            AccountAggregateManager = accountAggregateManager;
        }
        public bool Handle(IDomainEvent<Account, AccountId, MoneySentEvent> domainEvent)
        {
            try
            {
// Note that on event replays, the aggregate no longer appears "new".                
//                var isNewSpec = new AggregateIsNewSpecification();
//                if (isNewSpec.IsSatisfiedBy(this))
//                {
                if (!this.State.EventsSeen.Contains(domainEvent.GetIdentity()))
                {
                    Emit(new EventWasSeen(domainEvent.GetIdentity()));
                    throw new Exception("I've not seen this event before.");
                }

                var command = new ReceiveMoneyCommand(
                    domainEvent.AggregateEvent.Transaction.Receiver,
                    domainEvent.AggregateEvent.Transaction);

                AccountAggregateManager.Tell(command);

                Emit(new MoneyTransferStartedEvent(domainEvent.AggregateEvent.Transaction));
//                }

                return true;
            }
            catch (Exception excp)
            {
                throw new SagaHandlerException(excp, domainEvent);
            }
        }

        public bool Handle(IDomainEvent<Account, AccountId, MoneyReceivedEvent> domainEvent)
        {
            var spec = new AggregateIsNewSpecification().Not();
            if (spec.IsSatisfiedBy(this))
            {
                Emit(new MoneyTransferCompletedEvent(domainEvent.AggregateEvent.Transaction));
            }

            return true;
        }


    }

    public class SagaHandlerException : Exception
    {
        public IDomainEvent EventTriggeringException { get; }
        public SagaHandlerException( Exception exception, IDomainEvent @event):base("A SagaHandler exception.", exception)
        {
            EventTriggeringException = @event;
        }
    }
}