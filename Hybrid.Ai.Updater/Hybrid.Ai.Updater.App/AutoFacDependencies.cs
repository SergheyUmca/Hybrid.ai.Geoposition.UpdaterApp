using Autofac;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;

namespace Hybrid.Ai.Updater.App
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BaseContext>();
            builder.RegisterType<IDbService>();
        }
    }
}