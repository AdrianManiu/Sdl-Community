﻿using System.ComponentModel.DataAnnotations.Schema;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.GlossaryService.DAL
{
	[System.Data.Linq.Mapping.Table(Name = "TagLink")]
	public class DbTagLink : IInterpretBankTable
	{
		[ForeignKey("GlossaryID")]
		[System.Data.Linq.Mapping.Column(Name = "GlossaryID")] public int GlossaryId { get; set; }

		[System.Data.Linq.Mapping.Column(Name = "TagID", IsPrimaryKey = true)] public int TagId { get; set; }

		[System.Data.Linq.Mapping.Column(Name = "TagName")] public string TagName { get; set; }
	}
}