﻿
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalDb.Entities
{
    public class ExternalCountry
    {
        /// <summary>
        /// Gets or sets the id for external country table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for external country table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of cities for external country table
        /// </summary>
        public IEnumerable<ExternalCity> Cities { get; set; } = new List<ExternalCity>();
    }
}
