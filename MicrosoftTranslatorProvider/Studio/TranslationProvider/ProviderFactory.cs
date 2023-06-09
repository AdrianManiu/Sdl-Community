﻿using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	[TranslationProviderFactory(Id = "MicrosoftTranslatorProviderPlugin_Factory",
                                Name = "MicrosoftTranslatorProviderPlugin_Factory",
                                Description = "MicrosoftTranslatorProviderPlugin_Factory")]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			var credential = credentialStore.GetCredential(new Uri(Constants.MicrosoftProviderFullScheme)) ?? throw new TranslationProviderAuthenticationException();
			var options = JsonConvert.DeserializeObject<MTETranslationOptions>(translationProviderState);
			var privateHeaders = new List<UrlMetadata>();
			try
			{
				var genericCredentials = new GenericCredentials(credential.Credential);
				foreach (var credentialKey in genericCredentials.PropertyKeys)
				{
					if (!credentialKey.StartsWith("header_"))
					{
						continue;
					}

					privateHeaders.Add(new UrlMetadata()
					{
						Key = credentialKey.Replace("header_", string.Empty),
						Value = genericCredentials[credentialKey]
					});
				}
				options.ClientID = genericCredentials["API-Key"];
				options.PrivateEndpoint ??= genericCredentials["PrivateEndpoint"];
				bool.TryParse(genericCredentials["UseCategoryID"], out var useCategoryId);
				options.UseCategoryID = useCategoryId;
				options.CategoryID = useCategoryId ? genericCredentials["CategoryID"] : string.Empty;
				options.Region = genericCredentials["Region"];
			}
			catch { }

			return new Provider(options, new RegionsProvider(), new HtmlUtil()) { PrivateHeaders = privateHeaders };
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = MTETranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}