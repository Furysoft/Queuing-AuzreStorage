// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Furysoft.Queuing.AzureStorage.Tests
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Serilog;

    /// <summary>
    /// The Test Base
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase" /> class.
        /// </summary>
        protected TestBase()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            this.LoggerFactory = new LoggerFactory().AddSerilog();
        }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Writes the time elapsed.
        /// </summary>
        /// <param name="sw">The sw.</param>
        protected void WriteTimeElapsed(Stopwatch sw)
        {
            Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds}ms ({sw.Elapsed})");
        }
    }
}