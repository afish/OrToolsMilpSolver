using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
    public class OrToolsVariable : IVariable
    {
        public IMilpManager MilpManager { get; }
        public Domain Domain { get; }
        public string Name { get; }
    }
}