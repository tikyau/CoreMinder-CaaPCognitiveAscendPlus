using Adxstudio.Xrm.AspNet.Cms;
using Adxstudio.Xrm.AspNet.PortalBus;
using Owin;

namespace Site
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			if (SetupConfig.OwinEnabled())
			{
				ConfigureAuth(app);
			}

			app.UseApplicationRestartPluginMessage(new PluginMessageOptions());
			app.UsePortalBus<ApplicationRestartPortalBusMessage>();
			app.UsePortalBus<CacheInvalidationPortalBusMessage>();
		}
	}
}
