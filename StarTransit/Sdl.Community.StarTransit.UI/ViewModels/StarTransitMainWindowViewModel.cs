﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Community.StarTransit.UI.Commands;
using Sdl.Community.StarTransit.UI.Controls;
using Sdl.Community.StarTransit.UI.Interfaces;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class StarTransitMainWindowViewModel : BaseViewModel, IWindowActions
	{
		#region Private Fields
		private ICommand _nextCommand;
		private ICommand _backCommand;
		private ICommand _createCommand;
		private bool _canExecuteNext;
		private bool _canExecuteBack;
		private bool _canExecuteCreate;
		private readonly PackageDetailsViewModel _packageDetailsViewModel;
		private readonly PackageDetails _packageDetails;
		private bool _isDetailsSelected;
		private bool _isTmSelected;
		private bool _isFinishSelected;
		private readonly FinishViewModel _finishViewModel;
		private readonly ProjectService _projectService;
		private bool _active;
		private bool _isEnabled;
		private string _color;
		private bool _hasTm;
		private TranslationMemories _translationMemories;
		private TranslationMemoriesViewModel _translationMemoriesViewModel;
		#endregion

		#region Constructors
		public StarTransitMainWindowViewModel(
			PackageDetailsViewModel packageDetailsViewModel,
			PackageDetails packageDetails,
			TranslationMemories translationMemories,
			TranslationMemoriesViewModel translationMeloriesMemoriesViewModel,
			FinishViewModel finishViewModel)
		{
			_packageDetailsViewModel = packageDetailsViewModel;
			_packageDetails = packageDetails;
			_translationMemories = translationMemories;
			_translationMemoriesViewModel = translationMeloriesMemoriesViewModel;
			CanExecuteBack = false;
			CanExecuteCreate = false;
			CanExecuteNext = true;
			_isDetailsSelected = true;
			_isTmSelected = false;
			_isFinishSelected = false;
			_finishViewModel = finishViewModel;
			Color = "#FFB69476";
			_projectService = new ProjectService();
		}
		#endregion

		#region Public Properties
		public static readonly Log Log = Log.Instance;

		public bool DetailsSelected
		{
			get { return _isDetailsSelected; }
			set
			{
				if (Equals(value, _isDetailsSelected))
				{
					return;
				}
				_isDetailsSelected = value;
				OnPropertyChanged();
			}
		}

		public bool TmSelected
		{
			get { return _isTmSelected; }
			set
			{
				if (Equals(value, _isTmSelected))
				{
					return;
				}
				_isTmSelected = value;
				OnPropertyChanged();
			}
		}

		public bool FinishSelected
		{
			get
			{
				return _isFinishSelected;
			}
			set
			{
				if (Equals(value, _isFinishSelected))
				{
					return;
				}
				_isFinishSelected = value;
				OnPropertyChanged();
			}
		}

		public bool CanExecuteNext
		{
			get { return _canExecuteNext; }
			set
			{
				if (Equals(value, _canExecuteNext))
				{
					return;
				}
				_canExecuteNext = value;
				OnPropertyChanged();
			}
		}

		public bool CanExecuteBack
		{
			get { return _canExecuteBack; }
			set
			{
				if (Equals(value, _canExecuteBack))
				{
					return;
				}

				_canExecuteBack = value;
				OnPropertyChanged();
			}
		}

		public string Color
		{
			get { return _color; }
			set
			{
				if (Equals(value, _color))
				{
					return;
				}
				_color = value;
				OnPropertyChanged();
			}
		}

		public bool CanExecuteCreate
		{
			get { return _canExecuteCreate; }
			set
			{
				if (Equals(value, _canExecuteCreate))
				{
					return;
				}
				_canExecuteCreate = value;
				OnPropertyChanged();
			}
		}

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				if (Equals(value, _isEnabled))
				{
					return;

				}
				_isEnabled = value;
				OnPropertyChanged();
			}
		}

		public bool Active
		{
			get { return _active; }
			set
			{
				if (Equals(value, _active))
				{
					return;
				}
				_active = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region Actions
		public Action CloseAction { get; set; }
		public Action<string, string> ShowWindowsMessage { get; set; }
		#endregion

		#region Commands
		public ICommand NextCommand
		{
			get { return _nextCommand ?? (_nextCommand = new CommandHandler(Next, true)); }
		}

		public ICommand BackCommand
		{
			get { return _backCommand ?? (_backCommand = new CommandHandler(Back, true)); }
			set
			{
				if (Equals(value, _backCommand))
				{
					return;
				}
				_backCommand = value;
				OnPropertyChanged();
			}
		}

		public ICommand CreateCommand
		{
			get { return _createCommand ?? (_createCommand = new CommandHandler(Create, true)); }
		}
		#endregion

		#region Public Methods
		public void Next()
		{
			try
			{
				var model = _packageDetailsViewModel.GetPackageModel();
				_hasTm = false;
				var isEmpty = IsFolderEmpty(_packageDetailsViewModel.TextLocation);

				if (isEmpty)
				{
					foreach (var pair in model.LanguagePairs)
					{
						if (pair.StarTranslationMemoryMetadatas.Count != 0)
						{
							_hasTm = true;
						}
					}//tm page is disabled
					if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm == false)
					{
						DetailsSelected = false;
						TmSelected = false;
						FinishSelected = true;
						CanExecuteBack = true;
						CanExecuteNext = false;
						_finishViewModel.Refresh();
						CanExecuteCreate = true;
						IsEnabled = false;
						Color = "Gray";
					}//tm page
					else if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm)
					{
						DetailsSelected = false;
						TmSelected = true;
						FinishSelected = false;
						CanExecuteBack = true;
						CanExecuteNext = true;
						CanExecuteCreate = false;
						IsEnabled = true;
						Color = "#FF66290B";
					}//finish page
					else if (_packageDetails.FieldsAreCompleted() && TmSelected && _translationMemories.TmFieldIsCompleted())
					{
						DetailsSelected = false;
						CanExecuteNext = false;
						CanExecuteCreate = true;
						CanExecuteBack = true;
						TmSelected = false;
						IsEnabled = true;
						FinishSelected = true;
						_finishViewModel.Refresh();
						Color = "#FFB69476";
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Next method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public void Back()
		{
			try
			{
				if (DetailsSelected)
				{
					CanExecuteBack = false;
				}
				else if (FinishSelected && _hasTm == false)
				{
					CanExecuteBack = false;
					DetailsSelected = true;
					TmSelected = false;
					CanExecuteNext = true;
					FinishSelected = false;
					CanExecuteCreate = false;
				}
				else if (FinishSelected && _hasTm)
				{
					DetailsSelected = false;
					TmSelected = true;
					FinishSelected = false;
					CanExecuteBack = true;
					CanExecuteCreate = false;
					CanExecuteNext = true;
					Color = "#FF66290B";
				}
				else if (TmSelected)
				{
					DetailsSelected = true;
					TmSelected = false;
					FinishSelected = false;
					CanExecuteBack = false;
					CanExecuteCreate = false;
					CanExecuteNext = true;
					Color = "#FFB69476";
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Back method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public async void Create()
		{
			try
			{
				Active = true;
				CanExecuteBack = CanExecuteCreate = false;
				var packageModel = _translationMemoriesViewModel.GetPackageModel();
				var isEmpty = IsFolderEmpty(packageModel.Location);
				var messageModel = new MessageModel();

				CloseAction();

				if (isEmpty)
				{
					await Task.Run(() => messageModel = _projectService.CreateProject(packageModel));
				}
				if (messageModel == null)
				{
					CanExecuteBack = CanExecuteCreate = false;
					Active = false;
					CloseAction();
					Helpers.Utils.DeleteFolder(packageModel.PathToPrjFile);
				}
				else
				{
					ShowWindowsMessage(messageModel.Title, messageModel.Message);
					Active = false;
					CanExecuteBack = CanExecuteCreate = false;
				}
				Helpers.Utils.DeleteFolder(packageModel.PathToPrjFile);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Create method: {ex.Message}\n {ex.StackTrace}");
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Check to see if the folder is empty, in case the user just paste the path in text box
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		private bool IsFolderEmpty(string folderPath)
		{
			if(string.IsNullOrEmpty(folderPath))
			{
				ShowWindowsMessage("Warning", "All fields are required!");
				return false;
			}
			if (!Helpers.Utils.IsFolderEmpty(folderPath))
			{
				ShowWindowsMessage("Folder not empty!", "Please select an empty folder");
				return false;
			}
			return true;
		}
		#endregion
	}
}