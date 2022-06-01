using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CCreative.Compilers.Generators;

public class MainGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
	}

	public async void Execute(GeneratorExecutionContext context)
	{
		var name = String.Empty;

		foreach (var tree in context.Compilation.SyntaxTrees)
		{
			var root = await tree.GetRootAsync();

			foreach (var node in root.DescendantNodes())
			{
				if (node is ClassDeclarationSyntax syntax)
				{
					if (syntax.BaseList?.DescendantNodes().Any(a => a.GetText().ToString().Contains("IProgram")) is true)
					{
						name = syntax.Identifier.ToString();
						break;
					}
				}
			}
		}

		context.AddSource("Entry", !String.IsNullOrWhiteSpace(name) ? $"CCreative.PApplet.Initialize(new {name}());" : String.Empty);
	}
}