using System.ComponentModel;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace NotifyPropertyChangedBenchmark;

internal class Program
{
	private static void Main(string[] args)
	{
		BenchmarkRunner.Run<Test>(null!, args);
	}
}

[MemoryDiagnoser]
public class Test
{
	private readonly NoCache noCache = new();
	private readonly Cache cache = new();

	public Test()
	{
		// これがないと PropertyChanged イベントが null になるので通知しなくなる。
		noCache.PropertyChanged += PropertyChanged;
		cache.PropertyChanged += PropertyChanged;
	}

	private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		
	}

	[Benchmark]
	public void NotUseCach()
	{
		noCache.Value = 1;
	}

	[Benchmark(Baseline = true)]
	public void UseCach()
	{
		cache.Value = 1;
	}
}

public class NoCache : INotifyPropertyChanged
{
	private int _value;

	public int Value
	{
		get => _value;
		set
		{
			_value = value;
			NotifyPropertyChanged(nameof(Value));
		}
	}

	protected void NotifyPropertyChanged([CallerMemberName] string? caller = null)
	{
		PropertyChanged?.Invoke(this, new(caller));
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class Cache : INotifyPropertyChanged
{
	private int _value;

	public int Value
	{
		get => _value;
		set
		{
			_value = value;
			NotifyPropertyChanged(EventArgsCache.ValuePropertyChanged);
		}
	}

	protected void NotifyPropertyChanged(PropertyChangedEventArgs e)
	{
		PropertyChanged?.Invoke(this, e);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	internal static class EventArgsCache
	{
		internal static readonly PropertyChangedEventArgs ValuePropertyChanged = new(nameof(Value));
	}
}
