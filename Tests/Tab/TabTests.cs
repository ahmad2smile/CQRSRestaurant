using Domain.Tab;
using Domain.Tab.Commands;
using Domain.Tab.Events;
using NUnit.Framework;
using System;

namespace Tests.Tab
{
    [TestFixture]
    public class TabTests : BddTest<TabAggregate>
    {
        private Guid TestId { get; set; }
        private int TestTable { get; set; }
        private string TestWaiter { get; set; }

        [SetUp]
        public void Setup()
        {
            TestId = Guid.NewGuid();
            TestTable = 42;
            TestWaiter = "Derek";
        }

        [Test]
        public void CanOpenANewTab()
        {
            Test(
                Given(),
                When(new OpenTab
                {
                    Id = TestId,
                    TableNumber = TestTable,
                    Waiter = TestWaiter
                }),
                Then(
                    new TabOpened
                    {
                        Id = TestId,
                        TableNumber = TestTable,
                        Waiter = TestWaiter
                    })
                );
        }


    }
}
