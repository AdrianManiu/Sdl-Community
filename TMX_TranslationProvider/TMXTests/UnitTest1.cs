using TMX_TranslationProvider.TmxFormat;

namespace TMXTests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var parser = new TmxParser("..\\..\\..\\Samples\\#4.tmx");
			parser.LoadAsync().Wait();
			Assert.Equal(false, parser.HasError);
			Assert.Equal("INTRODUCTION", parser.TranslationUnits[0].SourceText());
			Assert.Equal("INTRODUCCI�N", parser.TranslationUnits[0].TargetText());

			Assert.Equal("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.", parser.TranslationUnits[1].SourceText());
			Assert.Equal("Este documento contiene el Interserve Construcci�n C�digo de Salud y Seguridad de los subcontratistas y la sostenibilidad C�digo de los subcontratistas.", parser.TranslationUnits[1].TargetText());
		}
	}
}