﻿using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	[View(
		Id = "SdLTmAnonymizerController",
		Name = "SDLTM Anonymizer",
		Icon = "icon",
		Description = "Anonymize personal information in Translation Memories",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
		AllowViewParts = true)]
	public class TmAnonymizerViewController : AbstractViewController
	{
		private static TmAnonymizerViewControl _control;
		private static TmAnonymizerExplorerControl _explorerControl;
		private static TmAnonymizerLogViewController _logViewController;

		private static MainViewModel _model;
		private static SettingsService _settingsService;

		protected override void Initialize(IViewContext context)
		{
			_settingsService = new SettingsService(new PathInfo());

			_model = new MainViewModel(_settingsService, this);

			_control = new TmAnonymizerViewControl(_model);
			_explorerControl = new TmAnonymizerExplorerControl(_model);
			_logViewController = new TmAnonymizerLogViewController(_model);
		}

		public UserControl ContentControl => _control;

		protected override Control GetContentControl()
		{
			return _control;
		}

		protected override Control GetExplorerBarControl()
		{
			return _explorerControl;
		}


		[RibbonGroup("TmAnonymizerSettingsRibbonGroup", "  Settings  ")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class TmAnonymizerSettingsRibbonGroup : AbstractRibbonGroup
		{
		}

		[Action("TmAnonymizerTmRibbonGroupSettingsAction", typeof(TmAnonymizerViewController), Name = " Settings ", Icon = "Settings", Description = "Settings ")]
		[ActionLayout(typeof(TmAnonymizerSettingsRibbonGroup), 7, DisplayType.Large)]
		public class TmAnonymizerTmRibbonGroupSettingsAction : AbstractAction
		{
			protected override void Execute()
			{			
				var settingsWindow = new SettingsWindow();
				var settingsViewModel = new SettingsViewModel(settingsWindow, _settingsService);
				settingsWindow.DataContext = settingsViewModel;

				settingsWindow.ShowDialog();				
			}
		}


		[RibbonGroup("TmAnonymizerTMRibbonGroup", "Translation Memories")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class TmAnonymizerTmRibbonGroup : AbstractRibbonGroup
		{
		}

		[Action("TmAnonymizerTmRibbonGroupAddFileBasedTmAction", typeof(TmAnonymizerViewController), Name = "Add file-based TM", Icon = "TranslationMemory", Description = "Add file-based TM")]
		[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 6, DisplayType.Large)]
		public class TmAnonymizerTmRibbonGroupAddFileBasedTmAction : AbstractAction
		{
			protected override void Execute()
			{
				_model?.TranslationMemoryViewModel.AddFileBasedTm();
			}
		}

		[Action("TmAnonymizerTmRibbonGroupAddServerTmAction", typeof(TmAnonymizerViewController), Name = "Add server TM", Icon = "ServerBasedTranslationMemory", Description = "Add server TM")]
		[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 5, DisplayType.Large)]
		public class TmAnonymizerTmRibbonGroupAddServerTmAction : AbstractAction
		{
			protected override void Execute()
			{
				_model?.TranslationMemoryViewModel.AddServerTm();
			}
		}

		[Action("TmAnonymizerTMRibbonGroupOpenFolderAction", typeof(TmAnonymizerViewController), Name = "Select Folder", Icon = "TranslationMemoriesFolder_Open", Description = "Add all file-based TMs in the selected folder")]
		[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 4, DisplayType.Normal)]
		public class TmAnonymizerTmRibbonGroupOpenFolderAction : AbstractAction
		{
			protected override void Execute()
			{
				_model?.TranslationMemoryViewModel.SelectFolder();
			}
		}

		[Action("TmAnonymizerTmRibbonGroupRemoveTmAction", typeof(TmAnonymizerViewController), Name = "Remove TM", Icon = "Remove", Description = "Remove TM")]
		[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 3, DisplayType.Normal)]
		public class TmAnonymizerTmRibbonGroupRemoveTmAction : AbstractAction
		{
			protected override void Execute()
			{
				_model?.TranslationMemoryViewModel.RemoveTm();
			}
		}

		[Action("TmAnonymizerTmRibbonGroupRemoveTmCacheAction", typeof(TmAnonymizerViewController), Name = "Clear TM Cache", Icon = "RemoveCache", Description = "Clear TM Cache")]
		[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 2, DisplayType.Normal)]
		public class TmAnonymizerTmRibbonGroupRemoveTmCacheAction : AbstractAction
		{
			protected override void Execute()
			{
				_model?.TranslationMemoryViewModel.ClearCache();
			}
		}


		[RibbonGroup("TmAnonymizerHelpRibbonGroup", "    Help    ")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class TmAnonymizerHelpRibbonGroup : AbstractRibbonGroup
		{
		}

		[Action("TmAnonymizerHelpRibbonGroupHelpAction", typeof(TmAnonymizerViewController), Name = "Online Help", Icon = "wiki", Description = "An wiki page will be opened in browser with user documentation")]
		[ActionLayout(typeof(TmAnonymizerHelpRibbonGroup), 1, DisplayType.Large)]
		public class TmAnonymizerHelpRibbonGroupHelpAction : AbstractAction
		{
			protected override void Execute()
			{
				System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdltmanonymizer");
			}
		}

		[Action("TmAnonymizerHelpRibbonGroupAboutAction", typeof(TmAnonymizerViewController), Name = "About", Icon = "information", Description = "About")]
		[ActionLayout(typeof(TmAnonymizerHelpRibbonGroup), 0, DisplayType.Large)]
		public class TmAnonymizerHelpRibbonGroupAboutAction : AbstractAction
		{
			protected override void Execute()
			{
				var about = new AboutBox();
				about.ShowDialog();
			}
		}
	}
}
