using BenchmarkDotNet.Running;

// always run on release, to optimize and perform better

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

