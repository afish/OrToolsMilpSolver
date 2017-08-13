using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
	public class OrToolsMilpSolverSettings : MilpManagerSettings
	{
		public OrToolsMilpSolverSettings()
		{
			Solver = Solver.CreateSolver("OrTools", "CBC_MIXED_INTEGER_PROGRAMMING");
		}

		public Solver Solver { get; set; }
	}
}