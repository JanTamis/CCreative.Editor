using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Avalonia.Threading;
using AvaloniaEdit.Document;

namespace CCreative.Editor.Helpers;

public class ConsoleWriter : StreamReader
{
	private readonly TextDocument document;
	private readonly ConcurrentQueue<string> buffer = new();

	public ConsoleWriter(TextDocument document) : base(Stream.Null)
	{
		this.document = document;

		Initialize();
	}

	private async void Initialize()
	{
		var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(5));

		while (await timer.WaitForNextTickAsync())
		{
			if (!buffer.IsEmpty)
			{
				using (document.RunUpdate())
				{
					for (var i = 0; i < 10 && buffer.TryDequeue(out var text); i++)
					{
						document.Insert(document.TextLength, text);
					}
				}
			}
		}
	}

	public override int Read(Span<char> buffer)
	{
		this.buffer.Enqueue(buffer.ToString());

		return buffer.Length;
	}

	public void Clear()
	{
		buffer.Clear();
	}

	// public override Encoding Encoding => Encoding.UTF8;
}