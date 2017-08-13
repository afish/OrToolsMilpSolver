using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
	public interface IOrToolsVariable : IVariable
	{
		Variable Variable { get; set; }
	}
}