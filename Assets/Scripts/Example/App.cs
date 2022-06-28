using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Example
{ 
  public class App : MonoBehaviour
  {
    public RenderData renderData; // Initialized by Unity

    void OnEnable()
    {
      SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
      DatabaseEvents.NumberOfItemsInCollectionLoaded.AddListener(OnNumberOfItemsInCollectionLoaded);
      DatabaseEvents.CollectionLoaded.AddListener(OnCollectionLoaded);
    }

    private void OnNumberOfItemsInCollectionLoaded(int NumberOfItemsInCollection) 
    {
      UnityEngine.Debug.Log($"OnNumberOfItemsInCollectionLoaded: {NumberOfItemsInCollection}");

      DatabaseEvents.LoadData.Invoke();
    }

    private void OnCollectionLoaded() 
    {
      UnityEngine.Debug.Log("OnCollectionLoaded");

      var ItemIds = renderData.Collection.Select(item => { return $"{item.Id}"; }).ToArray();
      var ItemIdsString = String.Join(", ", ItemIds);

      UnityEngine.Debug.Log($"ItemIds: {ItemIdsString}");
    }
    
    void Start()
    {
      UnityEngine.Debug.Log("App running");
    }

    void OnDisable()
    {
      UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
      DatabaseEvents.NumberOfItemsInCollectionLoaded.RemoveListener(OnNumberOfItemsInCollectionLoaded);
      DatabaseEvents.CollectionLoaded.RemoveListener(OnCollectionLoaded);
    }
  }
}
