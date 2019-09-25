using System;

namespace Domain.Tab.Commands
{
    public class OpenTab
    {
        public Guid Id { get; set; }
        public int TableNumber { get; set; }
        public string Waiter { get; set; }
    }
}
