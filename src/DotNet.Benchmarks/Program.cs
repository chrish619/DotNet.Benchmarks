using BenchmarkDotNet.Running;

// See https://aka.ms/new-console-template for more information
BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);