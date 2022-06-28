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
  public class DataManager : MonoBehaviour
  {
    public RenderData renderData; // Initialized by Unity
    public string DatabaseFilePath = ""; // Initialized by Unity or use default 

    private ThreadedDatabaseConnection threadedDatabaseConnection;
    
    void Start()
    {
      SubscribeToEvents();

      if (DatabaseFilePath == "")
      {
        DatabaseFilePath = Application.dataPath + "/example_db.db";
      }

      UnityEngine.Debug.Log($"Loading database: {DatabaseFilePath}");

      StartDatabaseConnectionThread();
      SetupDatabase();
      LoadInitialData();
    }

    private void SubscribeToEvents()
    {
      DatabaseEvents.LoadData.AddListener(OnLoadData);
    }

    private void StartDatabaseConnectionThread()
    {
      threadedDatabaseConnection = new ThreadedDatabaseConnection(DatabaseFilePath);
    }

    // Setup is performed by putting queries into a FIFO queue on a thread. 
    // Queries submitted are executed in order! It's threaded and scary, but you can trust it.
    private void SetupDatabase()
    {
      threadedDatabaseConnection.QueueQuery<string>(DatabaseQueries.SpeedupReadsQuery);
      threadedDatabaseConnection.QueueQuery<string>(DatabaseQueries.CreateInMemoryDbQuery);
      threadedDatabaseConnection.QueueQuery<string>(DatabaseQueries.CopyCollectionToInMemoryDbQuery);
    }

    private void LoadInitialData()
    {
      // Separate queries for counts works better for larger data sets and different in-memory paradigms.
      LoadCollectionCount();
    }

    private void LoadCollectionCount() 
    {
      threadedDatabaseConnection.QueueQuery<int>(DatabaseQueries.CollectionCountQuery, (NumberOfItemsInCollection) =>
      {
        renderData.NumberOfItemsInCollection = NumberOfItemsInCollection;
        UnityMainThreadDispatcher.Instance().Enqueue(() => DatabaseEvents.NumberOfItemsInCollectionLoaded.Invoke(NumberOfItemsInCollection));
      });
    }
    
    public void OnLoadData()
    {
      LoadData();
    }

    private void LoadData()
    {
      LoadCollection();
    }

    public void LoadCollection()
    {
      threadedDatabaseConnection.QueueQuery<List<CollectionDBSchema>>(DatabaseQueries.CollectionQuery, (Collection) =>
      {
        renderData.Collection = Collection;
        UnityMainThreadDispatcher.Instance().Enqueue(() => DatabaseEvents.CollectionLoaded.Invoke());
      });
    }

    void OnDisable()
    {
      UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
      DatabaseEvents.LoadData.RemoveListener(OnLoadData);
    }
  }
}
