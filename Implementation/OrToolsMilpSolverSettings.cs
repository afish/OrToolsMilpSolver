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

        public OrToolsMilpSolverSettings(Solver solver)
		{
			Solver = solver;
		}

		public Solver Solver { get; }
	}
}