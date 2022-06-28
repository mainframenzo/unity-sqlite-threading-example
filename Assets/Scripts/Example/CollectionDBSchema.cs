using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
  [Table("why_PM_if")]
  //[Dynamic] // TODO necessary? breaks query
  public partial class CollectionDBSchema 
  {
    [Column("id")]
    [PrimaryKey]
    public int Id { get; set; }
  }
}