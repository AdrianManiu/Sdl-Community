﻿using System;
using InterpretBank.GlossaryService;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	[TerminologyProviderFactory(Id = "My_Terminology_Provider_Id",
								Name = "My_Terminology_Provider_Name",
								Description = "My_Terminology_Provider_Description")]
	public class InterpretBankProviderFactory : ITerminologyProviderFactory
	{
		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			var settings = PersistenceService.PersistenceService.GetSettings(terminologyProviderUri.Scheme);

			var interpretBankDataContext = new InterpretBankDataContext();
			interpretBankDataContext.Setup(settings.DatabaseFilepath);

			var termSearchService = new TerminologyService.TerminologyService(interpretBankDataContext);

			var interpretBankProvider = new InterpretBankProvider(termSearchService, settings);

			return interpretBankProvider;
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri) =>
			PersistenceService.PersistenceService.GetSettings(terminologyProviderUri.Scheme) is not null;
	}
}