﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MAD.UnitOfWorkExecutor.Tests")]
namespace MAD.UnitOfWorkExecutor
{
    public sealed class UOWExe
    {
        private readonly IServiceProvider serviceProvider;
        private readonly CancellationTokenSource cancellationTokenSource;

        internal UOWExe(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Start()
        {
            IServiceScope rootScope = this.serviceProvider.CreateScope();

            ExecutorService unitOfWorkAttributeService = rootScope.ServiceProvider.GetRequiredService<ExecutorService>();
            await unitOfWorkAttributeService.Start();
        }

        public async Task Run()
        {
            await this.Start();
        }

        public async Task Stop()
        {
            (this.serviceProvider as IDisposable)?.Dispose();
        }
    }
}
