﻿using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_PrintReport_Action",
		Name = "ReportsViewer_PrintReport_Name",
		Description = "ReportsViewer_PrintReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Print"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 5, DisplayType.Normal)]
	public class PrintReportAction : BaseReportAction
	{
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			_reportsViewerController.Print();
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public override void Initialize()
		{
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
			_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

			SetEnabled();
		}

		public void Run()
		{
			Execute();
		}

		private void ReportsViewerController_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			_canEnable = e.SelectedReports?.Count > 0;
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}