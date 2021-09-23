using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;

namespace CCreative.Compiler
{
	public class Compiler : RoslynHost, IDisposable
	{
		private readonly RoslynWorkspace workspace;

		private Solution Solution => workspace.CurrentSolution;
		
		public Compiler() : base(additionalAssemblies: new[]
		{
			typeof(GlyphExtensions).Assembly,
			Assembly.Load("RoslynPad.Editor.Avalonia"), 
		}, RoslynHostReferences.NamespaceDefault.With(new[]
		{ 
			MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
		}))
		{
			workspace = this.CreateWorkspace();
		}

		public DocumentId AddDocument(DocumentCreationArgs args)
		{
			return this.AddDocument(args);
		}

		public void CreateProject(string name, string assemblyName, string language = "C#")
		{
			var projectId = ProjectId.CreateNewId();
			var solution = Solution.AddProject(projectId, name, assemblyName, language);

			workspace.SetCurrentSolution(solution);
		}

		public void Dispose()
		{
			workspace.Dispose();
			
			GC.SuppressFinalize(this);
		}
	}
}