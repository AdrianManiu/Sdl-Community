﻿using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using Sdl.TellMe.ProviderApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class MTEnhancedCommunityForumAction : AbstractTellMeAction
	{
		public MTEnhancedCommunityForumAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.ForumIcon;


	}
}