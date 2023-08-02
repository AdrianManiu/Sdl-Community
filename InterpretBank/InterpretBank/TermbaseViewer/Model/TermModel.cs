﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InterpretBank.Commands;

namespace InterpretBank.TermbaseViewer.Model
{
	public class TermModel : INotifyPropertyChanged
	{
		private string _commentAll;
		private bool _edited;
		private bool _isEditing;
		private int _sourceLanguageIndex;
		private string _sourceTerm;
		private string _sourceTermComment1;
		private string _sourceTermComment2;
		private int _targetLanguageIndex;
		private string _targetTerm;
		private string _targetTermComment1;
		private string _targetTermComment2;

		public TermModel()
		{ }

		public TermModel(long id, string targetTerm, string targetTermComment1, string targetTermComment2, string sourceTerm, string sourceTermComment1, string sourceTermComment2, string commentAll, int sourceLanguageIndex, int targetLanguageIndex)
		{
			Id = id;
			_targetTerm = targetTerm;
			_targetTermComment1 = targetTermComment1;
			_targetTermComment2 = targetTermComment2;
			_sourceTerm = sourceTerm;
			_sourceTermComment1 = sourceTermComment1;
			_sourceTermComment2 = sourceTermComment2;
			_commentAll = commentAll;
			_sourceLanguageIndex = sourceLanguageIndex;
			_targetLanguageIndex = targetLanguageIndex;

			SetOriginalTerm();

			_edited = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string CommentAll
		{
			get => _commentAll;
			set => SetField(ref _commentAll, value);
		}

		public bool Edited
		{
			get
			{
				var properties = GetType().GetProperties().Where(p => p.Name.ToLower().Contains("term") || p.Name == "CommentAll");
				var x = properties.Any(propertyInfo => !propertyInfo.GetValue(this).Equals(propertyInfo.GetValue(OriginalTerm)));
				return _edited = x;
			}
			set
			{
				SetField(ref _edited, value);
			}
		}

		public long Id { get; set; }

		public bool IsEditing
		{
			get => _isEditing;
			set
			{
				if (value == _isEditing)
					return;
				_isEditing = value;
				OnPropertyChanged();
			}
		}

		public ICommand RevertCommand => new RelayCommand(Revert);

		public int SourceLanguageIndex
		{
			get => _sourceLanguageIndex;
			set => _sourceLanguageIndex = value;
		}

		// {
		// 	get
		// 	{
		// 		var properties = GetType().GetProperties().Where(p => p.Name.ToLower().Contains("term") || p.Name == "CommentAll");
		// 		return properties.Any(propertyInfo => !propertyInfo.GetValue(this).Equals(propertyInfo.GetValue(OriginalTerm)));
		// 	}
		// }
		public string SourceTerm
		{
			get => _sourceTerm;
			set => SetField(ref _sourceTerm, value);
		}

		public string SourceTermComment1
		{
			get => _sourceTermComment1;
			set => SetField(ref _sourceTermComment1, value);
		}

		public string SourceTermComment2
		{
			get => _sourceTermComment2;
			set => SetField(ref _sourceTermComment2, value);
		}

		public int TargetLanguageIndex
		{
			get => _targetLanguageIndex;
			set => _targetLanguageIndex = value;
		}

		public string TargetTerm
		{
			get => _targetTerm;
			set => SetField(ref _targetTerm, value);
		}

		public string TargetTermComment1
		{
			get => _targetTermComment1;
			set => SetField(ref _targetTermComment1, value);
		}

		public string TargetTermComment2
		{
			get => _targetTermComment2;
			set => SetField(ref _targetTermComment2, value);
		}

		private TermModel OriginalTerm { get; set; }

		public override bool Equals(object obj)
		{
			return obj is TermModel other && Id == other.Id && CommentAll == other.CommentAll && TargetTermComment1 == other.TargetTermComment1 && TargetTermComment2 == other.TargetTermComment2 && TargetTerm == other.TargetTerm && SourceTerm == other.SourceTerm && SourceTermComment1 == other.SourceTermComment1 && SourceTermComment2 == other.SourceTermComment2;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (CommentAll != null ? CommentAll.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment1 != null ? TargetTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment2 != null ? TargetTermComment2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTerm != null ? TargetTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTerm != null ? SourceTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment1 != null ? SourceTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment2 != null ? SourceTermComment2.GetHashCode() : 0);
				return hashCode;
			}
		}

		public void SetOriginalTerm()
		{
			OriginalTerm = new TermModel
			{
				TargetTerm = TargetTerm,
				TargetTermComment1 = TargetTermComment1,
				TargetTermComment2 = TargetTermComment2,
				SourceTerm = SourceTerm,
				SourceTermComment1 = SourceTermComment1,
				SourceTermComment2 = SourceTermComment2,
				CommentAll = CommentAll
			};
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;

			OnPropertyChanged(propertyName);
			OnPropertyChanged(nameof(Edited));
			return true;
		}

		private void Revert(object obj)
		{
			SourceTerm = OriginalTerm.SourceTerm;
			SourceTermComment1 = OriginalTerm.SourceTermComment1;
			SourceTermComment2 = OriginalTerm.SourceTermComment2;

			TargetTerm = OriginalTerm.TargetTerm;
			TargetTermComment1 = OriginalTerm.TargetTermComment1;
			TargetTermComment2 = OriginalTerm.TargetTermComment2;

			CommentAll = OriginalTerm.CommentAll;

			OnPropertyChanged(nameof(Edited));
		}
	}
}