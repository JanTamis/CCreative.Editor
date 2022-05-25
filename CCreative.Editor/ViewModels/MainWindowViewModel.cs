using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using CCreative.Compilers;
using CCreative.Editor.Helpers;
using CCreative.Editor.ViewModels.PopupViewModels;
using ReactiveUI;

namespace CCreative.Editor.ViewModels
{
	public class MainWindowViewModel : ViewModelBase, IDisposable
	{
		private TabViewModel _currentTab;
		private TextDocument _document;
		private ObservableCollection<TabViewModel> Tabs { get; } = new();

		private Compiler? compiler;
		private Process? process;

		private string _path;

		private object bufferLock = new();

		private int previousLineCount = 0;
		private bool isRunning;
		private bool canScrollDown;

		private PeriodicTimer? consoleTimer;
		private CancellationTokenSource consoleCancelSource;
		private List<char> consoleBuffer = new();

		private double previousDocumentHeight;

		public TextDocument ConsoleDocument
		{
			get => ConsoleEditor.Document;
			set
			{
				ConsoleEditor.Document = value;

				this.RaiseAndSetIfChanged(ref _document, value);
			}
		}

		public TextEditor ConsoleEditor { get; set; }

		public int ConsoleLineCount
		{
			get
			{
				var lineCount = ConsoleDocument.LineCount;

				if (Math.Abs(ConsoleEditor.TextArea.TextView.ScrollOffset.Y - previousDocumentHeight) <= ConsoleEditor.TextArea.TextView.DefaultLineHeight * (lineCount - previousLineCount + 1))
				{
					ScrollDown();
				}

				previousDocumentHeight = ConsoleEditor.TextArea.TextView.DocumentHeight - ConsoleEditor.Bounds.Height;
				previousLineCount = lineCount;

				if (ConsoleDocument.Lines[lineCount - 1].Length is 0)
				{
					lineCount--;
				}

				this.RaisePropertyChanged(nameof(CanScrollDown));

				return lineCount;
			}
		}

		public TabViewModel CurrentTab
		{
			get => _currentTab;
			set => this.RaiseAndSetIfChanged(ref _currentTab, value);
		}

		public bool IsRunning
		{
			get => isRunning;
			set => this.RaiseAndSetIfChanged(ref isRunning, value);
		}

		public bool CanScrollDown => previousDocumentHeight - ConsoleEditor.TextArea.TextView.VerticalOffset > ConsoleEditor.TextArea.TextView.DefaultLineHeight;
		public bool CanClear => ConsoleDocument.TextLength > 0;

		public MainWindowViewModel()
		{
			Initialize();

			ConsoleEditor = new TextEditor
			{
				IsReadOnly = true,
				FontFamily = new FontFamily("Cascadia Code"),
			};

			ConsoleDocument.TextLengthChanged += delegate
			{
				this.RaisePropertyChanged(nameof(ConsoleLineCount));
				this.RaisePropertyChanged(nameof(CanClear));
			};
			ConsoleEditor.TextArea.TextView.ScrollOffsetChanged += (_, _) => this.RaisePropertyChanged(nameof(CanScrollDown));
		}

		private async void Initialize()
		{
			IsRunning = true;

			Task.Run(() => { compiler = new Compiler("Test", "Test", typeof(Vector).Assembly, typeof(object).Assembly, Assembly.Load("System.Runtime")); }).Wait();

			IsRunning = false;
			AddTab("Tab 1");
		}

		public async Task NewTab()
		{
			if (compiler is not null)
			{
				var tabViewModel = await DialogHost.DialogHost.Show(new AddTabViewModel());
			
				if (tabViewModel is AddTabViewModel model && !String.IsNullOrWhiteSpace(model.Name))
				{
					AddTab(model.Name);
				}
			}
		}

		private void AddTab(string name)
		{
			if (compiler is not null)
			{
				var editor = TextEditorFactory.CreateTextEditor();
				var id = compiler.CreateDocument(name);
			
				TextEditorFactory.RegisterEvents(editor, id, compiler);
				TextEditorFactory.RegisterLayers(editor, id, compiler);
			
				Tabs.Add(new TabViewModel(name, editor, id));
				CurrentTab = Tabs[^1];
			}
		}

		public async Task Start()
		{
			IsRunning = true;
			ClearConsole();

			GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, false, true);

			if (compiler is not null)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "project.dll");

				File.Delete(filePath);
				Console.SetOut(new DocumentTextWriter(ConsoleDocument, 0));

#if DEBUG
				var action = await compiler.Compile();

				await Task.Run(() => action(null));

				return;
#endif

				var success = false;

				using (var peStream = File.OpenWrite(filePath))
				{
					success = await compiler.Save(peStream, null);
				}

				if (success)
				{
					if (consoleTimer is null)
					{
						InitializeConsoleTimer();
					}

					process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							Arguments = filePath,
							FileName = "dotnet",
							UseShellExecute = false,
							RedirectStandardOutput = true,
						},
					};

					var isNewProcess = process.Start();
					var buffer = new char[1];

					if (isNewProcess)
					{
						while (!process.HasExited)
						{
							await process.StandardOutput.ReadAsync(buffer);

							lock (bufferLock)
							{
								consoleBuffer.Add(buffer[0]);
							}
						}
					}
					else
					{
						process.Kill();
					}

					consoleTimer?.Dispose();
					consoleTimer = null;

					lock (bufferLock)
					{
						if (!consoleBuffer.TrueForAll(Char.IsWhiteSpace))
						{
							ConsoleDocument.Insert(ConsoleDocument.TextLength, new string(CollectionsMarshal.AsSpan(consoleBuffer)));
						}

						consoleBuffer.Clear();
					}
				}
			}

			IsRunning = false;

			async void InitializeConsoleTimer()
			{
				consoleTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(5));

				while (await consoleTimer.WaitForNextTickAsync())
				{
					lock (bufferLock)
					{
						if (consoleBuffer.Count > 0)
						{
							using (ConsoleDocument.RunUpdate())
							{
								ConsoleDocument.Insert(ConsoleDocument.TextLength, new string(CollectionsMarshal.AsSpan(consoleBuffer)));
								consoleBuffer.Clear();
							}
						}
					}
				}
			}
		}

		public void Stop()
		{
			PApplet.Stop();

			process?.Kill(true);
		}

		public void ClearConsole()
		{
			lock (bufferLock)
			{
				ConsoleDocument.Remove(0, ConsoleDocument.TextLength);
			}
		}

		public void ScrollDown()
		{
			lock (bufferLock)
			{
				ConsoleEditor.ScrollToEnd();
			}
		}

		public void Dispose()
		{
			// compiler?.Dispose();
			GC.SuppressFinalize(this);
		}

		~MainWindowViewModel()
		{
			Dispose();
		}
	}
}