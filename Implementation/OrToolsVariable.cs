using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
    public class OrToolsVariable : IVariable
    {
        public IMilpManager MilpManager { get; set; }
        public Domain Domain { get; set; }
        public string Name { get; set; }
        public Variable Variable { get; set; }
        public double? ConstantValue { get; set; }
        public override string ToString()
        {
            return $"[Name = {Name}, Domain = {Domain}, ConstantValue = {ConstantValue}, Variable = {Variable}";
        }
    }
}