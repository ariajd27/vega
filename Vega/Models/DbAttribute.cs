﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vega.Models;

namespace Vega.Models
{
    public class DbAttribute
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)] public string BothShort { get; set; } = null!;
        public string CategoryDescr { get; set; } = null!;
        public string ValueDescr { get; set; } = null!;
        public List<DbCourse> Courses { get; } = [];

        public override string ToString() => BothShort.Split('.')[0] + ": " + ValueDescr;
    }
}
