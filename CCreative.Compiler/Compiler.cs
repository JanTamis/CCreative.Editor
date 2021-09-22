using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.BraceMatching;

namespace CCreative.Compiler
{
	public class Compiler
	{
		// private readonly AdhocWorkspace space;

		public Compiler()
		{
			var host = new RoslynHost(references: RoslynHostReferences.NamespaceDefault.With(new[]
			{ 
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
			}));

			var service = host.GetService<IBraceMatchingService>();
		}
	}
}