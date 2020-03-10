﻿using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Events;

namespace ProjectController
{
	[ApplicationInitializer]
	public class ProjectControllerMethods : IApplicationInitializer
	{
		public void Execute()
		{
			//files controller
			//var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			//var activeProjectFromFiles = filesController.CurrentProject;

		}
	}


	[RibbonGroup("Test", Name = "Test project icon")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class ProjectTemplateRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TestProjIconAction", Icon = "", Name = "Test icon",
		Description = "")]
	[ActionLayout(typeof(ProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ProjectAction : AbstractAction
	{
		protected override void Execute()
		{
			//projects controler open file view for selected language
			var eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var activeProject = projectsController?.CurrentProject;
			eventAggregator.Publish(new ChangeSourceContentSettingsEvent(activeProject,false,false,true));
			//var targetLanguages = activeProject?.GetProjectInfo().TargetLanguages;
			//if (targetLanguages?.Length >= 2)
			//{
			//	eventAggregator.Publish(new OpenProjectForSelectedLanguageEvent(activeProject, targetLanguages[1]));
			//	//to open just a project without target language
			//	//eventAggregator.Publish(new OpenProjectForSelectedLanguageEvent(activeProject));
			//}
		}
	}
}
