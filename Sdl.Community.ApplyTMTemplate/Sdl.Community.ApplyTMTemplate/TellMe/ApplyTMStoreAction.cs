﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
    public class ApplyTMTemplateStoreAction : AbstractTellMeAction
    {
		public ApplyTMTemplateStoreAction()
		{
			Name = "Download Apply TM Template from AppStore";
		}
	    public override void Execute()
	    {
		    Process.Start("https://appstore.sdl.com/language/app");
	    }

	    public override bool IsAvailable => true;

	    public override string Category => "Apply TM Template results";

	    public override Icon Icon => PluginResources.ATTDownload;
	}
}
