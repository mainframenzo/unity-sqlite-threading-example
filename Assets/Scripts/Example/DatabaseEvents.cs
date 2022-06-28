using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Example
{ 
  public class UnityIntEvent : UnityEvent<int> { }

  public class DatabaseEvents
  {
    public static UnityIntEvent NumberOfItemsInCollectionLoaded = new UnityIntEvent();
    public static UnityEvent LoadData = new UnityEvent();
    public static UnityEvent CollectionLoaded = new UnityEvent();
  }
}
