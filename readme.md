# unity-sqlite-threading-example
This source code demonstrates:
- Async, threaded SQLite queries in Unity `2021.3.0f1` using a queue
- Loading SQLite databases in-memory and querying those
- Object mapping from SQLite queries to C# classes
- Data callbacks using Unity Events

[sqlite-generate](https://github.com/simonw/sqlite-generate) was used to generate the example database with 1 table: `sqlite-generate example_db.db --pks=1 --rows=50 --columns=2 --tables=1 --fks=0`