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
using Akkatecture.Entities;
using Domain.Model.Account.ValueObjects;
using Newtonsoft.Json;

namespace Domain.Model.Account.Entities
{
    public class Transaction : Entity<TransactionId>
    {
        public AccountId Sender { get; }
        public AccountId Receiver { get; }
        public Money Amount { get; }
        public string Reason { get; }
        
        [JsonConstructor]
        public Transaction(
            TransactionId id,
            AccountId sender,
            AccountId receiver, 
            Money amount, 
            string reason)
            : base(id)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if(amount == null) throw new ArgumentNullException(nameof(amount));
            if(sender == receiver) throw new ArgumentException($"{nameof(Sender)} should be the same as {nameof(Receiver)}.");
            
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Reason = reason;
        }

        public Transaction(
            AccountId sender,
            AccountId receiver,
            Money amount, 
            string reason)
            :this(TransactionId.New,sender,receiver,amount, reason)
        {  
        }
    }
}