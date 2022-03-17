using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using CCreative.Compilers.Enums;
using CCreative.Compilers.Generators;
using CCreative.Compilers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;
using Microsoft.IO;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForCanBeConvertedToForeach

namespace CCreative.Compilers
{
	public class Compiler : IDisposable
	{
		private readonly AdhocWorkspace workspace;
		private GeneratorDriver driver;

		private static readonly RecyclableMemoryStreamManager manager = new();

		private Solution Solution => workspace.CurrentSolution;

		private Project Project => Solution.Projects.FirstOrDefault()!;

		public Compiler(string name, string assemblyName, params Assembly[] assemblies)
		{
			workspace = new AdhocWorkspace(MefHostServices.DefaultHost);

			var project = Solution.AddProject(name, assemblyName, "C#");
			project = project.AddMetadataReferences(assemblies.Select(s => MetadataReference.CreateFromFile(s.Location)));
			var document = project.AddDocument("Usings", "global using static CCreative.Math; global using static CCreative.PApplet; global using CCreative;");

			project = document.Project;

			workspace.TryApplyChanges(project.Solution);

			driver = CSharpGeneratorDriver.Create(new MainGenerator());
		}

		public DocumentId CreateDocument(string name, string text = "")
		{
			var document = Project.AddDocument(name, text);

			workspace.TryApplyChanges(document.Project.Solution);

			return document.Id;
		}

		public void UpdateDocument(DocumentId id, string text)
		{
			var document = GetDocument(id);

			if (document is null)
			{
				throw new ArgumentException("Provide a valid id", nameof(id));
			}

			document = document.WithText(SourceText.From(text));
			workspace.TryApplyChanges(document.Project.Solution);
		}

		public void RemoveDocument(DocumentId id)
		{
			workspace.TryApplyChanges(Solution.RemoveDocument(id));
		}

		public async IAsyncEnumerable<Diagnostic> GetErrors([EnumeratorCancellation] CancellationToken token = default)
		{
			var project = Project;

			if (project is null)
			{
				throw new Exception("Create a project first");
			}

			var compilation = await project.GetCompilationAsync(token);

			if (compilation is not null)
			{
				driver?.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var errors, token);

				await using var stream = manager.GetStream();

				var result = compilation.Emit(stream, cancellationToken: token);

				for (var i = 0; i < result.Diagnostics.Length; i++)
				{
					var diagnostic = result.Diagnostics[i];

					if (diagnostic is { Severity: not DiagnosticSeverity.Hidden, Location.Kind: LocationKind.SourceFile })
					{
						yield return diagnostic;
					}
				}
			}
		}

		public async IAsyncEnumerable<Diagnostic> GetErrors(DocumentId id, [EnumeratorCancellation] CancellationToken token = default)
		{
			var document = GetDocument(id);
			var project = document!.Project;
			var tree = await document.GetSyntaxTreeAsync(token);

			if (project is null)
			{
				throw new Exception("Create a project first");
			}

			var compilation = await project.GetCompilationAsync(token);

			if (compilation is not null)
			{
				driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, token);

				await using var stream = manager.GetStream();

				var result = compilation.Emit(stream, cancellationToken: token);

				for (var i = 0; i < result.Diagnostics.Length; i++)
				{
					var diagnostic = result.Diagnostics[i];

					if (diagnostic is { Severity: not DiagnosticSeverity.Hidden, Location.Kind: LocationKind.SourceFile } && diagnostic.Location.SourceTree == tree)
					{
						yield return diagnostic;
					}
				}
			}
		}

		public async IAsyncEnumerable<CompletionItem?> GetCompletions(DocumentId id, int position, [EnumeratorCancellation] CancellationToken token = default)
		{
			var document = GetDocument(id);

			if (document is not null)
			{
				var service = CompletionService.GetService(document);

				if (service is not null)
				{
					var completions = await service.GetCompletionsAsync(document, position, cancellationToken: token);

					if (completions is not null)
					{
						foreach (var completion in completions.Items.DistinctBy(d => d.DisplayText))
						{
							yield return completion;
						}
					}
				}
			}
		}

		public async Task<CompletionChange?> GetChanges(DocumentId id, CompletionItem item, CancellationToken token = default)
		{
			var document = GetDocument(id);
			CompletionChange change = null;

			if (document is not null)
			{
				var service = CompletionService.GetService(document);

				if (service is not null)
				{
					change = await service.GetChangeAsync(document, item, cancellationToken: token);
				}
			}

			return change;
		}

		public async Task<bool> ShouldTriggerCompletion(DocumentId id, int position, CancellationToken token = default)
		{
			var document = GetDocument(id);

			if (document is not null)
			{
				var service = CompletionService.GetService(document);

				if (service is not null)
				{
					return service.ShouldTriggerCompletion(await document.GetTextAsync(token), position, CompletionTrigger.Invoke);
				}
			}

			return false;
		}

		public async IAsyncEnumerable<ClassifierModel> GetClassifiers(DocumentId id, int start, int length, [EnumeratorCancellation] CancellationToken token = default)
		{
			var document = GetDocument(id);

			if (document is not null)
			{
				var spans = await Classifier.GetClassifiedSpansAsync(document, new TextSpan(start, length), token);

				foreach (var span in spans)
				{
					if (span.ClassificationType != ClassificationTypeNames.StaticSymbol)
					{
						yield return new ClassifierModel(span.TextSpan.Start, span.TextSpan.Length, span.ClassificationType switch
						{
							ClassificationTypeNames.Comment => ClassifierType.Comment,
							ClassificationTypeNames.ExcludedCode => ClassifierType.ExcludedCode,
							ClassificationTypeNames.Identifier => ClassifierType.Identifier,
							ClassificationTypeNames.Keyword => ClassifierType.Keyword,
							ClassificationTypeNames.ControlKeyword => ClassifierType.ControlKeyword,
							ClassificationTypeNames.NumericLiteral => ClassifierType.NumericLiteral,
							ClassificationTypeNames.Operator => ClassifierType.Operator,
							ClassificationTypeNames.OperatorOverloaded => ClassifierType.OperatorOverloaded,
							ClassificationTypeNames.PreprocessorKeyword => ClassifierType.PreprocessorKeyword,
							ClassificationTypeNames.StringLiteral => ClassifierType.StringLiteral,
							ClassificationTypeNames.WhiteSpace => ClassifierType.WhiteSpace,
							ClassificationTypeNames.Text => ClassifierType.Text,

							ClassificationTypeNames.StaticSymbol => ClassifierType.StaticSymbol,

							ClassificationTypeNames.PreprocessorText => ClassifierType.PreprocessorText,
							ClassificationTypeNames.Punctuation => ClassifierType.Punctuation,
							ClassificationTypeNames.VerbatimStringLiteral => ClassifierType.VerbatimStringLiteral,
							ClassificationTypeNames.StringEscapeCharacter => ClassifierType.StringEscapeCharacter,

							ClassificationTypeNames.ClassName => ClassifierType.ClassName,
							ClassificationTypeNames.DelegateName => ClassifierType.DelegateName,
							ClassificationTypeNames.EnumName => ClassifierType.EnumName,
							ClassificationTypeNames.InterfaceName => ClassifierType.InterfaceName,
							ClassificationTypeNames.ModuleName => ClassifierType.ModuleName,
							ClassificationTypeNames.StructName => ClassifierType.StructName,
							ClassificationTypeNames.TypeParameterName => ClassifierType.TypeParameterName,
							"record class name" => ClassifierType.RecordClassName,
							"record struct name" => ClassifierType.RecordStructName,

							ClassificationTypeNames.FieldName => ClassifierType.FieldName,
							ClassificationTypeNames.EnumMemberName => ClassifierType.EnumMemberName,
							ClassificationTypeNames.ConstantName => ClassifierType.ConstantName,
							ClassificationTypeNames.LocalName => ClassifierType.LocalName,
							ClassificationTypeNames.ParameterName => ClassifierType.ParameterName,
							ClassificationTypeNames.MethodName => ClassifierType.MethodName,
							ClassificationTypeNames.ExtensionMethodName => ClassifierType.ExtensionMethodName,
							ClassificationTypeNames.PropertyName => ClassifierType.PropertyName,
							ClassificationTypeNames.EventName => ClassifierType.EventName,
							ClassificationTypeNames.NamespaceName => ClassifierType.NamespaceName,
							ClassificationTypeNames.LabelName => ClassifierType.LabelName,

							ClassificationTypeNames.XmlDocCommentAttributeName => ClassifierType.XmlDocCommentAttributeName,
							ClassificationTypeNames.XmlDocCommentAttributeQuotes => ClassifierType.XmlDocCommentAttributeQuotes,
							ClassificationTypeNames.XmlDocCommentAttributeValue => ClassifierType.XmlDocCommentAttributeValue,
							ClassificationTypeNames.XmlDocCommentCDataSection => ClassifierType.XmlDocCommentCDataSection,
							ClassificationTypeNames.XmlDocCommentComment => ClassifierType.XmlDocCommentComment,
							ClassificationTypeNames.XmlDocCommentDelimiter => ClassifierType.XmlDocCommentDelimiter,
							ClassificationTypeNames.XmlDocCommentEntityReference => ClassifierType.XmlDocCommentEntityReference,
							ClassificationTypeNames.XmlDocCommentName => ClassifierType.XmlDocCommentName,
							ClassificationTypeNames.XmlDocCommentProcessingInstruction => ClassifierType.XmlDocCommentProcessingInstruction,
							ClassificationTypeNames.XmlDocCommentText => ClassifierType.XmlDocCommentText,

							ClassificationTypeNames.XmlLiteralAttributeName => ClassifierType.XmlLiteralAttributeName,
							ClassificationTypeNames.XmlLiteralAttributeQuotes => ClassifierType.XmlLiteralAttributeQuotes,
							ClassificationTypeNames.XmlLiteralAttributeValue => ClassifierType.XmlLiteralAttributeValue,
							ClassificationTypeNames.XmlLiteralCDataSection => ClassifierType.XmlLiteralCDataSection,
							ClassificationTypeNames.XmlLiteralComment => ClassifierType.XmlLiteralComment,
							ClassificationTypeNames.XmlLiteralDelimiter => ClassifierType.XmlLiteralDelimiter,
							ClassificationTypeNames.XmlLiteralEntityReference => ClassifierType.XmlLiteralEntityReference,
							ClassificationTypeNames.XmlLiteralName => ClassifierType.XmlLiteralName,
							ClassificationTypeNames.XmlLiteralProcessingInstruction => ClassifierType.XmlLiteralProcessingInstruction,
							ClassificationTypeNames.XmlLiteralText => ClassifierType.XmlLiteralText,

							ClassificationTypeNames.RegexComment => ClassifierType.RegexComment,
							ClassificationTypeNames.RegexCharacterClass => ClassifierType.RegexCharacterClass,
							ClassificationTypeNames.RegexAnchor => ClassifierType.RegexAnchor,
							ClassificationTypeNames.RegexQuantifier => ClassifierType.RegexQuantifier,
							ClassificationTypeNames.RegexGrouping => ClassifierType.RegexGrouping,
							ClassificationTypeNames.RegexAlternation => ClassifierType.RegexAlternation,
							ClassificationTypeNames.RegexText => ClassifierType.RegexText,
							ClassificationTypeNames.RegexSelfEscapedCharacter => ClassifierType.RegexSelfEscapedCharacter,
							ClassificationTypeNames.RegexOtherEscape => ClassifierType.RegexOtherEscape,

							_ => throw new ArgumentException($"{span.ClassificationType} is not yet registered"),
						});
					}
				}
			}
		}

		public async IAsyncEnumerable<QuickInfoGroupModel> GetInfo(DocumentId id, int position, [EnumeratorCancellation] CancellationToken token = default)
		{
			var document = GetDocument(id);
			var infoService = QuickInfoService.GetService(document);

			if (document is not null && infoService is not null)
			{
				var quickInfo = await infoService.GetQuickInfoAsync(document, position, token);

				for (var i = 0; i < quickInfo?.Sections.Length; i++)
				{
					var section = quickInfo.Sections[i];

					yield return new QuickInfoGroupModel
					{
						SectionType = Enum.Parse<QuickInfoSectionType>(section.Kind),
						Infos = section.TaggedParts.Select(s => new QuickInfoModel(s.Text, Enum.Parse<TagType>(s.Tag))),
					};
				}
			}
		}

		public async Task<Action<string[]>?> Compile(CancellationToken token = default)
		{
			var project = Project;

			if (project is null)
			{
				throw new Exception("Create a project first");
			}

			var compilation = await project.GetCompilationAsync(token);

			if (compilation is not null)
			{
				driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, token);

				await using var stream = manager.GetStream();

				var result = compilation.Emit(stream, cancellationToken: token);

				if (result.Success)
				{
					var assembly = Assembly.Load(stream.ToArray());
					var entry = assembly.EntryPoint;

					return entry?.CreateDelegate<Action<string[]>>();
				}
			}

			return null;
		}

		public async Task<bool> Save(Stream peStream, Stream pdbStream, CancellationToken token = default)
		{
			var project = Project;

			if (project is null)
			{
				throw new Exception("Create a project first");
			}

			var compilation = await project.GetCompilationAsync(token);
			driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, token);

			return compilation?.GetEntryPoint(token) is not null && compilation.Emit(peStream, pdbStream, cancellationToken: token).Success;
		}

		private Document? GetDocument(DocumentId id)
		{
			return Solution.GetDocument(id);
		}

		public void Dispose()
		{
			workspace.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}