# OpenTerminal Backend Server

## Building
`mvn package`

## Running
`java -jar target/OpenTerminalBackend-1.0-SNAPSHOT.jar`

## Runtime Commands
Type "help" to see them.

## API
The server listens for a `push-trades` SocketIO event. The data for this event should follow this format:
```json
{
  'ticker' : "AAPL",
  'trades' : [
    {
      'price' : 127.36,
      'volume' : 100,
      'date' : '28/05/2021 02:23:45 AM -05:00'
    },
    ...
  ]
}
```
The data is then deduped and sent to an event with the same name as the ticker.
Hence, if you want to listen for stock data on the AAPL ticker, simply add a listener to the AAPL event.

## TODO:

 - Sync data to PostgreSQL server
 - Schedule clients evenly along 15 second interval