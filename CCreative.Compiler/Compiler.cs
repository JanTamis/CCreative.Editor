using System;
using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;

namespace CCreative.Compilers
{
	public class Compiler : RoslynHost, IDisposable
	{
		private readonly RoslynWorkspace workspace;

		private Solution Solution => workspace.CurrentSolution;
		
		public Compiler() : base(additionalAssemblies: new[]
		{
			typeof(GlyphExtensions).Assembly
		}, RoslynHostReferences.NamespaceDefault.With(new[]
		{ 
			MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
		}))
		{
			workspace = new RoslynWorkspace(HostServices, roslynHost: this);
		}

		public Project CreateProject(string name, string assemblyName, string language = "C#")
		{
			var project = Solution.AddProject(name, assemblyName, language);

			workspace.SetCurrentSolution(project.Solution);

			return project;
		}

		public void Dispose()
		{
			workspace.Dispose();
			
			GC.SuppressFinalize(this);
		}
	}
}