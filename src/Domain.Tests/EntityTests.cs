using System;
using Akkatecture.Entities;
using Domain.Model.Account;
using Domain.Model.Account.Entities;
using Domain.Model.Account.ValueObjects;
using Newtonsoft.Json;
using Xunit;

namespace Domain.Tests
{
	public class EntityTests
	{
		[Fact]
		public void Test_TransactionEntitySerialization()
		{
			var transaction = new Transaction( AccountId.New,AccountId.New, 
				new Money(1.0m), "some test reason");
			var transactionId = transaction.Id;
			var serializedObject = JsonConvert.SerializeObject(transaction);
			var deserializedObject = JsonConvert.DeserializeObject<Transaction>(serializedObject);

			Assert.True(transaction.Equals(deserializedObject));
			
		}
	}
}
